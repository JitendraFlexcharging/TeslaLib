using System;
using System.Collections.Generic;
using System.Threading;
using TeslaLib;
using TeslaLib.Models;

namespace TeslaConsole
{
    public class Program
    {
        private const String TESLA_CLIENT_ID = "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384";
        private const String TESLA_CLIENT_SECRET = "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3";

        public static void Main(string[] args)
        {
            string clientId = TESLA_CLIENT_ID;
            string clientSecret = TESLA_CLIENT_SECRET;

            string email = "";

            string password = "";


            TeslaClient.TokenStoreForThisInstance = new FileBasedOAuthTokenStore();

            TeslaClient client = new TeslaClient(email, clientId, clientSecret);

            // If we have logged in previously with the same email address, then we can use this method and refresh tokens,
            // assuming the refresh token hasn't expired.
            //client.LoginUsingTokenStoreWithoutPasswordAsync().Wait();
            client.LoginUsingTokenStoreAsync(password).Wait();
            //client.LoginAsync(password).Wait();

            /*
            try
            {
                Console.WriteLine("Logging in without an MFA code for "+email);
                client.LoginAsync(password, null, TeslaAuth.TeslaAccountRegion.USA).Wait();
                Console.WriteLine("Succeeded without MFA code");
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught " + e);
                Console.Write("Enter Tesla multi-factor authentication code --> ");
                String mfaCode = Console.ReadLine().Trim();
                client.LoginAsync(password, mfaCode).Wait();
                Console.WriteLine("LoginAsync returned successfully");
            }
            */

            /*
            Console.Write("Enter Tesla multi-factor authentication code --> ");
            String mfaCode = Console.ReadLine().Trim();
            client.LoginAsync(password, mfaCode).Wait();
            */

            //client.GetAllProductsAsync(CancellationToken.None).Wait();
            bool printEnergySites = false;
            if (printEnergySites)
            {
                List<EnergySite> energySites = client.GetEnergySitesAsync(CancellationToken.None).Result;
                if (energySites != null && energySites.Count != 0)
                {
                    Console.WriteLine("Found {0} energy sites", energySites.Count);
                    foreach (EnergySite energySite in energySites)
                    {
                        Console.WriteLine($"Energy site name: {energySite.SiteName}  SoC: {energySite.StateOfCharge.ToString("0.0")}%  Power: {energySite.BatteryPower}  Energy left: {energySite.EnergyLeft.ToString("0")} / {energySite.TotalPackEnergy}");
                        //Console.WriteLine($"Site summary: {energySite.GetSiteSummary()}");
                        EnergySiteData liveStatus = energySite.GetLiveStatus();
                        Console.WriteLine($"Live status -  Home: {liveStatus.LoadPower}  Solar: {liveStatus.SolarPower}  Battery: {liveStatus.BatteryPower}  Grid: {liveStatus.GridPower}");
                        var configuration = energySite.GetSiteConfiguration();
                        Console.WriteLine($"Configuration - Mode: {configuration.DefaultRealMode}  Site Name: {configuration.SiteName}");
                        var settings = configuration.UserSettings;
                        Console.WriteLine($"User settings - Storm mode enabled: {settings.StormModeEnabled}  Breaker alert: {settings.BreakerAlertEnabled}  Sync grid alert: {settings.SyncGridAlertEnabled}");
                        var touSettings = configuration.TouSettings;
                        Console.WriteLine($"TOU settings - Optimization strategy: {touSettings.OptimizationStrategy}  Number of schedule items: {touSettings.Schedule.Count}");
                        Console.WriteLine($"Installation time zone: {configuration.InstallationTimeZone}");
                        Console.WriteLine($"Nameplate Energy: {configuration.NameplateEnergy}  Nameplate Power: {configuration.NameplatePower}");

                        Console.WriteLine("Energy history:");
                        var energyHistory = energySite.GetEnergyHistory();
                        foreach (var energyHistoryData in energyHistory.TimeSeries)
                            Console.WriteLine($"{energyHistoryData.TimeStamp}  Solar exported: {energyHistoryData.SolarEnergyExported.ToString("0")}  Powerwall exported: {energyHistoryData.BatteryEnergyExported.ToString("0")}  Solar -> Battery {energyHistoryData.BatteryEnergyImportedFromSolar.ToString("0")}");
                        Console.WriteLine();

                        Console.WriteLine($"Power History:");
                        var powerHistory = energySite.GetPowerHistory();
                        foreach (var data in powerHistory.TimeSeries)
                            Console.WriteLine($"  {data.TimeStamp}  Grid services: {data.GridServicesPower.ToString("0")}  Solar: {data.SolarPower.ToString("0")}  Powerwall: {data.BatteryPower.ToString("0")}  Grid: {data.GridPower.ToString("0")}");

                        /*  // Doesn't work, doesn't exist, or I don't know how to call it right.
                        Console.WriteLine("Calendar Energy history:");
                        var energyCalendarHistory = energySite.GetCalendarEnergyHistory(DateTimeOffset.Now);
                        foreach (var energyHistoryData in energyCalendarHistory.TimeSeries)
                            Console.WriteLine($"{energyHistoryData.TimeStamp}  Solar exported: {energyHistoryData.SolarEnergyExported.ToString("0")}  Powerwall exported: {energyHistoryData.BatteryEnergyExported.ToString("0")}  Solar -> Battery {energyHistoryData.BatteryEnergyImportedFromSolar.ToString("0")}");
                        Console.WriteLine();
                        */

                        Console.WriteLine($"Calendar Power History:");
                        var powerCalendarHistory = energySite.GetCalendarPowerHistory(DateTimeOffset.Now);
                        Console.WriteLine($"Calendar Power History has {powerCalendarHistory.TimeSeries.Length} items in the time series:");
                        foreach (var data in powerCalendarHistory.TimeSeries)
                            Console.WriteLine($"  {data.TimeStamp}  Grid services: {data.GridServicesPower.ToString("0")}  Solar: {data.SolarPower.ToString("0")}  Powerwall: {data.BatteryPower.ToString("0")}  Grid: {data.GridPower.ToString("0")}");
                    }
                }
            }

            var vehicles = client.LoadVehicles();

            foreach (TeslaVehicle car in vehicles)
            {
                Console.WriteLine(car.DisplayName + "   VIN: " + car.Vin + "  Model refresh number: "+car.Options.ModelRefreshNumber);
                Console.WriteLine("Car state: {0}", car.State);

                TimeSpan maxWakeupTime = TimeSpan.FromSeconds(30);
                DateTimeOffset startWaking = DateTimeOffset.Now;
                var newState = car.State;
                while (newState == TeslaLib.Models.VehicleState.Asleep || newState == TeslaLib.Models.VehicleState.Offline)
                {
                    Console.Write("Waking up...  ");
                    newState = car.WakeUp();
                    Console.WriteLine("WakeUp returned.  New vehicle state: {0}", newState);
                    Thread.Sleep(2000);
                    if (DateTimeOffset.Now - startWaking > maxWakeupTime)
                    {
                        Console.WriteLine("Giving up on waking up.");
                        break;
                    }
                }

                Console.WriteLine("Is mobile access enabled?  {0}", car.LoadMobileEnabledStatus());

                var vehicleState = car.LoadVehicleStateStatus();
                if (vehicleState == null)
                    Console.WriteLine("Vehicle state was null!  Is the car not awake?");
                else
                {
                    Console.WriteLine(" Roof state: {0}", vehicleState.PanoramicRoofState);
                    if (vehicleState.Odometer.HasValue)
                        Console.WriteLine(" Odometer: {0}", vehicleState.Odometer.Value);
                    else
                        Console.WriteLine(" Odometer has no value.");
                    Console.WriteLine(" Sentry Mode available: {0}  Sentry mode on: {1}", 
                        vehicleState.SentryModeAvailable, vehicleState.SentryMode);
                    Console.WriteLine("API version: {0}  Car version: {1}", vehicleState.ApiVersion.GetValueOrDefault(), vehicleState.CarVersion);
                }

                var vehicleConfig = car.LoadVehicleConfig();
                if (vehicleConfig == null)
                    Console.WriteLine("Couldn't get vehicle configuration");
                else
                {
                    Console.WriteLine("From VehicleConfig, Car type: {0}  special type: {1}  trim badging: {2}", vehicleConfig.CarType,
                        vehicleConfig.CarSpecialType, vehicleConfig.TrimBadging);
                    Console.WriteLine("Use range badging? {0}  Spoiler type: {1}", vehicleConfig.UseRangeBadging, vehicleConfig.SpoilerType);
                    Console.WriteLine("Color: {0}  Roof color: {1}  Has sunroof? {2}", vehicleConfig.ExteriorColor, vehicleConfig.RoofColor,
                        vehicleConfig.SunRoofInstalled.HasValue ? vehicleConfig.SunRoofInstalled.Value.ToString() : "false");
                    Console.WriteLine("Wheels: {0}", vehicleConfig.WheelType);
                }

                var chargeState = car.LoadChargeStateStatus();
                Console.WriteLine($" State of charge: {chargeState.BatteryLevel}%  Desired State of charge: {chargeState.ChargeLimitSoc}%");
                Console.WriteLine($" Charging state: {(chargeState.ChargingState.HasValue ? chargeState.ChargingState.Value.ToString() : "unknown")}");
                Console.WriteLine($"  Time until full charge: {chargeState.TimeUntilFullCharge} hours ({60*chargeState.TimeUntilFullCharge} minutes)  Usable battery level: {chargeState.UsableBatteryLevel}%");
                Console.WriteLine($" Charge current request in Amps: {chargeState.ChargeCurrentRequest}  Max charge current request: {chargeState.ChargeCurrentRequestMax}");
                Console.WriteLine($" Scheduled charging mode: {chargeState.ScheduledChargingMode}");
                Console.WriteLine($" Scheduled charging time: {chargeState.ScheduledChargingStartTime}  Minutes: {chargeState.ScheduledChargingStartTimeMinutes}");
                Console.WriteLine($"    Scheduled charging start time app: {chargeState.ScheduledChargingStartTimeApp}");
                Console.WriteLine($" Scheduled departure time: {chargeState.ScheduledDepartureTime}  Minutes: {chargeState.ScheduledDepartureTimeMinutes}");
                Console.WriteLine($" Scheduled charging pending? {chargeState.ScheduledChargingPending}");
                Console.WriteLine($" Managed charging active? {chargeState.ManagedChargingActive}  Managed charging start time? {chargeState.ManagedChargingStartTime}");
                Console.WriteLine($" Managed charging user canceled? {chargeState.ManagedChargingUserCanceled}");
                Console.WriteLine($" Off-peak charging enabled?  {chargeState.OffPeakChargingEnabled}");
                Console.WriteLine($" Off-peak charging times: {chargeState.OffPeakChargingTimes}  End time: {chargeState.OffPeakHoursEndTime}");
                Console.WriteLine($" Preconditioning enabled? {chargeState.PreconditioningEnabled}  Times: {chargeState.PreconditioningTimes}");

                var driveState = car.LoadDriveStateStatus();
                Console.WriteLine("  Shift state: {0}", driveState.ShiftState);
                var guiSettings = car.LoadGuiStateStatus();
                Console.WriteLine("  Units for distance: {0}   For temperature: {1}", guiSettings.DistanceUnits, guiSettings.TemperatureUnits);

                var options = car.Options;
                Console.WriteLine($"  Battery size: {options.BatterySize}  Has firmware limit? {(options.BatteryFirmwareLimit.HasValue ? options.BatteryFirmwareLimit.ToString() : false.ToString())}");
                // Note there is a BatteryRange and an EstimatedBatteryRange.  The BatteryRange seems to be about 4% higher on Brian's Model 3.  The Tesla app prints out BatteryRange.
                Console.WriteLine($"  Battery range: {chargeState.BatteryRange}  Estimated battery range: {chargeState.EstimatedBatteryRange}  Usable battery level: {chargeState.UsableBatteryLevel}");
                Console.WriteLine($"  Charger limit: {options.ChargerLimit}");
                Console.WriteLine($"Option Codes: {car.Options.RawOptionCodes}");

                var climate = car.LoadClimateStateStatus();
                Console.WriteLine("Climate:");
                Console.WriteLine($"  Driver temperature: {climate.DriverTemperatureSetting}  Passenger: {climate.PassengerTemperatureSetting}");
                Console.WriteLine($"  ClimateKeeperMode: {climate.ClimateKeeperMode}");

                var resultStatus = car.SetChargingAmps(23);
                if (resultStatus.Result)
                {
                    Console.WriteLine("SetChargingAmps succeeded");
                }
                else
                {
                    Console.WriteLine("SetChargingAmps failed.  Reason: " + resultStatus.Reason);
                }

            }

            client.RefreshLoginTokenAndUpdateTokenStoreAsync().Wait();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}

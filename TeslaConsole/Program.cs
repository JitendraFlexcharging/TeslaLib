using System;
using TeslaLib;

namespace TeslaConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string clientId = "";
            string clientSecret = "";

            string email = "";
            string password = "";

            TeslaClient.OAuthTokenStore = new FileBasedOAuthTokenStore();

            TeslaClient client = new TeslaClient(email, clientId, clientSecret);

            // If we have logged in previously with the same email address, then we can use this method and refresh tokens,
            // assuming the refresh token hasn't expired.
            //client.LoginUsingTokenStoreWithoutPasswordAsync().Wait();
            client.LoginUsingTokenStoreAsync(password).Wait();
            //client.LoginAsync(password).Wait();

            var vehicles = client.LoadVehicles();

            foreach (TeslaVehicle car in vehicles)
            {
                Console.WriteLine(car.DisplayName + "   VIN: " + car.Vin + "  Model refresh number: "+car.Options.ModelRefreshNumber);
                Console.WriteLine("Is mobile access enabled?  {0}", car.LoadMobileEnabledStatus());
                Console.WriteLine("Car state: {0}", car.State);
                if (car.State != TeslaLib.Models.VehicleState.Online)
                {
                    Console.Write("Waking up...  ");
                    var newState = car.WakeUp();
                    Console.WriteLine("WakeUp returned.  New vehicle state: {0}", newState);
                }

                var vehicleState = car.LoadVehicleStateStatus();
                if (vehicleState == null)
                    Console.WriteLine("Vehicle state was null!  Is the car not awake?");
                else
                {
                    Console.WriteLine(" Roof state: {0}", vehicleState.PanoramicRoofState);
                    Console.WriteLine(" Odometer: {0}", vehicleState.Odometer);
                    Console.WriteLine(" Sentry Mode available: {0}  Sentry mode on: {1}", 
                        vehicleState.SentryModeAvailable, vehicleState.SentryMode);
                }

                Console.WriteLine("API version: {0}  Car version: {1}", vehicleState.ApiVersion, vehicleState.CarVersion);

                var vehicleConfig = car.LoadVehicleConfig();
                Console.WriteLine("From VehicleConfig, Car type: {0}  special type: {1}  trim badging: {2}", vehicleConfig.CarType, 
                    vehicleConfig.CarSpecialType, vehicleConfig.TrimBadging);
                Console.WriteLine("Use range badging? {0}  Spoiler type: {1}", vehicleConfig.UseRangeBadging, vehicleConfig.SpoilerType);
                Console.WriteLine("Color: {0}  Roof color: {1}  Has sunroof? {2}", vehicleConfig.ExteriorColor, vehicleConfig.RoofColor, 
                    vehicleConfig.SunRoofInstalled.HasValue ? vehicleConfig.SunRoofInstalled.Value.ToString() : "false");
                Console.WriteLine("Wheels: {0}", vehicleConfig.WheelType);

                var chargeState = car.LoadChargeStateStatus();
                Console.WriteLine($" State of charge:  {chargeState.BatteryLevel}%  Desired State of charge: {chargeState.ChargeLimitSoc}%");
                Console.WriteLine($"  Time until full charge: {chargeState.TimeUntilFullCharge} hours ({60*chargeState.TimeUntilFullCharge} minutes)  Usable battery level: {chargeState.UsableBatteryLevel}%");
                Console.WriteLine($" Scheduled charging time: {chargeState.ScheduledChargingStartTime}");
                Console.WriteLine($" Scheduled departure time: {chargeState.ScheduledDepartureTime}");
                Console.WriteLine($" Scheduled charging pending? {chargeState.ScheduledChargingPending}");
                Console.WriteLine($" Managed charging active? {chargeState.ManagedChargingActive}  Managed charging start time? {chargeState.ManagedChargingStartTime}");
                Console.WriteLine($" Managed charging user canceled? {chargeState.ManagedChargingUserCanceled}");

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
            }

            client.RefreshLoginTokenAndUpdateTokenStoreAsync().Wait();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}

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
            client.LoginUsingTokenStoreWithoutPasswordAsync().Wait();
            //client.LoginUsingTokenStoreAsync(password).Wait();
            //client.LoginAsync(password).Wait();

            var vehicles = client.LoadVehicles();

            foreach (TeslaVehicle car in vehicles)
            {
                Console.WriteLine(car.DisplayName + "   VIN: " + car.Vin + "  Model year: "+car.Options.YearModel);
                Console.Write("Waking up...  ");
                car.WakeUp();
                Console.WriteLine("Done");

                var vehicleState = car.LoadVehicleStateStatus();
                if (vehicleState == null)
                    Console.WriteLine("Vehicle state was null!  Is the car not awake?");
                else
                {
                    Console.WriteLine(" Roof state: {0}", vehicleState.PanoramicRoofState);
                    Console.WriteLine(" Odometer: {0}", vehicleState.Odometer);
                }

                var chargeState = car.LoadChargeStateStatus();
                Console.WriteLine($" State of charge:  {chargeState.BatteryLevel}%  Desired State of charge: {chargeState.ChargeLimitSoc}%");
                Console.WriteLine($" Scheduled charging time: {chargeState.ScheduledChargingStartTime}");
                car.LoadDriveStateStatus();
                car.LoadClimateStateStatus();
                car.LoadGuiStateStatus();
                car.LoadMobileEnabledStatus();
                var options = car.Options;
                Console.WriteLine($"  Battery size: {options.BatterySize}  Has firmware limit? {(options.BatteryFirmwareLimit.HasValue ? options.BatteryFirmwareLimit.ToString() : false.ToString())}");
                // Note there is a BatteryRange and an EstimatedBatteryRange.  The BatteryRange seems to be about 4% higher on Brian's Model 3.  The Tesla app prints out BatteryRange.
                Console.WriteLine($"  Battery range: {chargeState.BatteryRange}  Estimated battery range: {chargeState.EstimatedBatteryRange}  Usable battery level: {chargeState.UsableBatteryLevel}");
                Console.WriteLine($"  Charger limit: {options.ChargerLimit}");
                Console.WriteLine("Climate:");
                var climate = car.LoadClimateStateStatus();
                Console.WriteLine($"  Driver temperature: {climate.DriverTemperatureSetting}  Passenger: {climate.PassengerTemperatureSetting}");
            }

            client.RefreshLoginTokenAndUpdateTokenStoreAsync().Wait();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}

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

            TeslaClient client = new TeslaClient(email, clientId, clientSecret);

            client.LoginAsync(password).Wait();

            var vehicles = client.LoadVehicles();

            foreach (TeslaVehicle car in vehicles)
            {
                Console.WriteLine(car.DisplayName + "   VIN: " + car.Vin);
                Console.Write("Waking up...  ");
                car.WakeUp();
                Console.WriteLine("Done");

                var vehicleState = car.LoadVehicleStateStatus();
                if (vehicleState == null)
                    Console.WriteLine("Vehicle state was null!  Is the car not awake?");
                else
                    Console.WriteLine(" Roof state: {0}", vehicleState.PanoramicRoofState);

                var chargeState = car.LoadChargeStateStatus();
                Console.WriteLine($" Desired State of charge: {chargeState.ChargeLimitSoc}%");
                car.LoadDriveStateStatus();
                car.LoadClimateStateStatus();
                car.LoadGuiStateStatus();
                car.LoadMobileEnabledStatus();
                var options = car.Options;
                Console.WriteLine($"  Battery size: {options.BatterySize}  Has firmware limit? {(options.BatteryFirmwareLimit.HasValue ? options.BatteryFirmwareLimit.ToString() : false.ToString())}");
                Console.WriteLine($"  Charger limit: {options.ChargerLimit}");
                Console.WriteLine("  Odometer: {0}", car.LoadVehicleStateStatus().Odometer);
                Console.WriteLine("Climate:");
                var climate = car.LoadClimateStateStatus();
                Console.WriteLine($"  Driver temperature: {climate.DriverTemperatureSetting}  Passenger: {climate.PassengerTemperatureSetting}");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}

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
                Console.WriteLine(car.Id + " " + car.Vin);
                var climateState = car.LoadClimateStateStatus();
                Console.WriteLine($"Climate State Inside: {climateState.InsideTemperature}  Outside: {climateState.OutsideTemperature}");
            }

            Console.ReadKey();
        }
    }
}

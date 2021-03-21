using System;
using Newtonsoft.Json;

namespace TeslaLib.Models
{
    public class EnergySiteUsageHistoryData
    {
        [JsonProperty(PropertyName = "solar_power")]
        public double SolarPower { get; set; }

        /// <summary>
        /// Looks like this is in Watts
        /// </summary>
        [JsonProperty(PropertyName = "battery_power")]
        public double BatteryPower { get; set; }

        /// <summary>
        /// Power in Watts(?)
        /// </summary>
        [JsonProperty(PropertyName = "grid_power")]
        public double GridPower { get; set; }

        [JsonProperty(PropertyName = "grid_services_power")]
        public double GridServicesPower { get; set; }

        [JsonProperty(PropertyName = "generator_power")]
        public double GeneratorPower { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
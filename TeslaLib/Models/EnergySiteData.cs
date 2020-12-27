using System;
using Newtonsoft.Json;

namespace TeslaLib.Models
{
    public class EnergySiteData
    {
        [JsonProperty(PropertyName = "solar_power")]
        public double SolarPower { get; set; }

        /// <summary>
        /// A site's energy stored in one or more Powerwalls in Watt-Hours
        /// </summary>
        [JsonProperty(PropertyName = "energy_left")]
        public double? EnergyLeft { get; set; }

        /// <summary>
        /// Max Powerwall capacity in Watt-Hours
        /// </summary>
        [JsonProperty(PropertyName = "total_pack_energy")]
        public double? TotalPackEnergy { get; set; }

        /// <summary>
        /// State of charge of the Powerwall
        /// </summary>
        [JsonProperty(PropertyName = "percentage_charged")]
        public double StateOfCharge { get; set; }

        [JsonProperty(PropertyName = "backup_capable")]
        public bool BackupCapable { get; set; }

        /// <summary>
        /// Looks like this is in Watts
        /// </summary>
        [JsonProperty(PropertyName = "battery_power")]
        public int BatteryPower { get; set; }

        /// <summary>
        /// Energy consumption at the site
        /// </summary>
        [JsonProperty(PropertyName = "load_power")]
        public double LoadPower { get; set; }

        /// <summary>
        /// A value like "Active"
        /// </summary>
        [JsonProperty(PropertyName = "grid_status")]
        public String GridStatus { get; set; }

        /// <summary>
        /// Might be whether the Powerwall is being used for frequency regulation, etc.
        /// </summary>
        [JsonProperty(PropertyName = "grid_services_active")]
        public bool GridServicesActive { get; set; }

        /// <summary>
        /// Power in Watts(?)
        /// </summary>
        [JsonProperty(PropertyName = "grid_power")]
        public int GridPower { get; set; }

        [JsonProperty(PropertyName = "grid_services_power")]
        public int GridServicesPower { get; set; }

        [JsonProperty(PropertyName = "generator_power")]
        public int GeneratorPower { get; set; }

        [JsonProperty(PropertyName = "storm_mode_active")]
        public bool StormModeActive { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
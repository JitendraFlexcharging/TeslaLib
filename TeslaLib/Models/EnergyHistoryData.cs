using System;
using Newtonsoft.Json;

namespace TeslaLib.Models
{
    // "timestamp":"2021-03-21T01:00:00-07:00","solar_energy_exported":4666.560277899727,"generator_energy_exported":0,"grid_energy_imported":26244.328333890997,
    // "grid_services_energy_imported":0,"grid_services_energy_exported":0,"grid_energy_exported_from_solar":49.51026646257378,
    // "grid_energy_exported_from_generator":0,"grid_energy_exported_from_battery":0,"battery_energy_exported":1170,
    // "battery_energy_imported_from_grid":41.45725615974516,"battery_energy_imported_from_solar":2018.5427438402548,
    // "battery_energy_imported_from_generator":0,"consumer_energy_imported_from_grid":26202.87107773125,
    // "consumer_energy_imported_from_solar":2598.5072675968986,"consumer_energy_imported_from_battery":1170,
    // "consumer_energy_imported_from_generator":0
    public class EnergyHistoryData
    {
        [JsonProperty(PropertyName = "solar_energy_exported")]
        public double SolarEnergyExported { get; set; }

        /// <summary>
        /// Looks like this is in Watts
        /// </summary>
        [JsonProperty(PropertyName = "generator_energy_exported")]
        public double GeneratorEnergyExported { get; set; }

        /// <summary>
        /// Power in Watts(?)
        /// </summary>
        [JsonProperty(PropertyName = "grid_energy_imported")]
        public double GridEnergyImported { get; set; }

        [JsonProperty(PropertyName = "grid_services_energy_imported")]
        public double GridServicesEnergyImported { get; set; }

        [JsonProperty(PropertyName = "grid_services_energy_exported")]
        public double GridServicesEnergyExported { get; set; }

        [JsonProperty(PropertyName = "grid_energy_exported_from_solar")]
        public double GridEnergyExportedFromSolar { get; set; }

        [JsonProperty(PropertyName = "grid_energy_exported_from_generator")]
        public double GridEnergyExportedFromGenerator { get; set; }

        [JsonProperty(PropertyName = "grid_energy_exported_from_battery")]
        public double GridEnergyExportedFromBattery { get; set; }

        [JsonProperty(PropertyName = "battery_energy_exported")]
        public double BatteryEnergyExported { get; set; }

        [JsonProperty(PropertyName = "battery_energy_imported_from_grid")]
        public double BatteryEnergyImportedFromGrid { get; set; }

        [JsonProperty(PropertyName = "battery_energy_imported_from_solar")]
        public double BatteryEnergyImportedFromSolar { get; set; }

        [JsonProperty(PropertyName = "battery_energy_imported_from_generator")]
        public double BatteryEnergyImportedFromGenerator { get; set; }

        [JsonProperty(PropertyName = "consumer_energy_imported_from_grid")]
        public double ConsumerEnergyImportedFromGrid{ get; set; }

        [JsonProperty(PropertyName = "consumer_energy_imported_from_solar")]
        public double ConsumerEnergyImportedFromSolar { get; set; }

        [JsonProperty(PropertyName = "consumer_energy_imported_from_battery")]
        public double ConsumerEnergyImportedFromBattery { get; set; }

        [JsonProperty(PropertyName = "consumer_energy_imported_from_generator")]
        public double ConsumerEnergyImportedFromGenerator { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
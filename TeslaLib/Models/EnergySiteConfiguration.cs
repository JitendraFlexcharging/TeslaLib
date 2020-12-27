using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TeslaLib.Models
{
    // {"response":{"id":"1118431-00-L--TG119288000A8V","site_name":"Brian's Lair","backup_reserve_percent":35,
    // "default_real_mode":"self_consumption","installation_date":"2020-01-29T12:20:00-08:00",
    // "user_settings":{"storm_mode_enabled":true,"sync_grid_alert_enabled":true,"breaker_alert_enabled":false},
    // "components":{"solar":true,"battery":true,"grid":true,"backup":true,"gateway":"teg","load_meter":true,"tou_capable":true,
    //              "storm_mode_capable":true,"flex_energy_request_capable":false,"car_charging_data_supported":false,
    //              "off_grid_vehicle_charging_reserve_supported":true,"vehicle_charging_performance_view_enabled":false,
    //              "vehicle_charging_solar_offset_view_enabled":false,"battery_solar_offset_view_enabled":true,"battery_type":"ac_powerwall",
    //              "configurable":true,"grid_services_enabled":false},
    // "version":"1.50.1","battery_count":2,
    // "tou_settings":{"optimization_strategy":"balanced",
    //                 "schedule":[{"target":"off_peak","week_days":[6,0],"start_seconds":0,"end_seconds":0}]},
    // "nameplate_power":10000,"nameplate_energy":27000,"installation_time_zone":"America/Los_Angeles",
    // "off_grid_vehicle_charging_reserve_percent":75}}
    public class EnergySiteUserSettings
    {
        [JsonProperty(PropertyName = "storm_mode_enabled")]
        public bool StormModeEnabled { get; set; }

        [JsonProperty(PropertyName = "sync_grid_alert_enabled")]
        public bool SyncGridAlertEnabled { get; set; }

        [JsonProperty(PropertyName = "breaker_alert_enabled")]
        public bool BreakerAlertEnabled { get; set; }
    }

    public class EnergySiteConfigurationComponents
    {
        [JsonProperty(PropertyName = "solar")]
        public bool Solar { get; set; }

        [JsonProperty(PropertyName = "battery")]
        public bool Battery { get; set; }

        [JsonProperty(PropertyName = "grid")]
        public bool Grid { get; set; }

        [JsonProperty(PropertyName = "backup")]
        public bool Backup { get; set; }

        [JsonProperty(PropertyName = "gateway")]
        public String Gateway { get; set; }

        [JsonProperty(PropertyName = "load_meter")]
        public bool LoadMeter { get; set; }

        [JsonProperty(PropertyName = "tou_capable")]
        public bool TouCapable { get; set; }

        [JsonProperty(PropertyName = "storm_mode_capable")]
        public bool StormModeCapable { get; set; }

        [JsonProperty(PropertyName = "flex_energy_request_capable")]
        public bool FlexEnergyRequestCapable { get; set; }

        [JsonProperty(PropertyName = "car_charging_data_supported")]
        public bool CarChargingDataSupported { get; set; }

        [JsonProperty(PropertyName = "off_grid_vehicle_charging_reserve_supported")]
        public bool OffGridVehicleChargingReserveSupported { get; set; }

        [JsonProperty(PropertyName = "vehicle_charging_performance_view_enabled")]
        public bool VehicleChargingPerformanceViewEnabled { get; set; }

        [JsonProperty(PropertyName = "vehicle_charging_solar_offset_view_enabled")]
        public bool VehicleChargingSolarOffsetViewEnabled { get; set; }

        [JsonProperty(PropertyName = "battery_solar_offset_view_enabled")]
        public bool BatterySolarOffsetViewEnabled { get; set; }

        [JsonProperty(PropertyName = "battery_type")]
        public String BatteryType { get; set; }

        [JsonProperty(PropertyName = "configurable")]
        public bool Configurable { get; set; }

        [JsonProperty(PropertyName = "grid_services_enabled")]
        public bool GridServicesEnabled { get; set; }
    }

    public class EnergySiteScheduleItem
    {
        /// <summary>
        /// A string like "off_peak"
        /// </summary>
        [JsonProperty(PropertyName = "target")]
        public String Target { get; set; }

        [JsonProperty(PropertyName = "week_days")]
        public List<int> WeekDays { get; set; }

        [JsonProperty(PropertyName = "start_seconds")]
        public int StartSeconds { get; set; }

        [JsonProperty(PropertyName = "end_seconds")]
        public int EndSeconds { get; set; }
    }

    public class EnergySiteTouSettings
    {
        /// <summary>
        /// A String like "balanced"
        /// </summary>
        [JsonProperty(PropertyName = "optimization_strategy")]
        public String OptimizationStrategy { get; set; }

        [JsonProperty(PropertyName = "schedule")]
        public List<EnergySiteScheduleItem> Schedule { get; set; }
    }

    public class EnergySiteConfiguration
    {
        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }

        /// <summary>
        /// A site's energy stored in one or more Powerwalls in Watt-Hours
        /// </summary>
        [JsonProperty(PropertyName = "site_name")]
        public String SiteName { get; set; }

        [JsonProperty(PropertyName = "backup_reserve_percent")]
        public int BackupReservePercent { get; set; }

        /// <summary>
        /// A value like "self_consumption"
        /// </summary>
        [JsonProperty(PropertyName = "default_real_mode")]
        public String DefaultRealMode { get; set; }

        [JsonProperty(PropertyName = "installation_date")]
        public DateTimeOffset InstallationDate { get; set; }

        [JsonProperty(PropertyName = "user_settings")]
        public EnergySiteUserSettings UserSettings { get; set; }

        [JsonProperty(PropertyName = "components")]
        public EnergySiteConfigurationComponents ConfigurationComponents { get; set; }

        [JsonProperty(PropertyName = "version")]
        public String Version { get; set; }

        [JsonProperty(PropertyName = "battery_count")]
        public int BatteryCount { get; set; }

        [JsonProperty(PropertyName = "tou_settings")]
        public EnergySiteTouSettings TouSettings { get; set; }

        [JsonProperty(PropertyName = "nameplate_power")]
        public int NameplatePower { get; set; }

        [JsonProperty(PropertyName = "nameplate_energy")]
        public int NameplateEnergy { get; set; }

        [JsonProperty(PropertyName = "installation_time_zone")]
        public String InstallationTimeZone { get; set; }

        [JsonProperty(PropertyName = "off_grid_vehicle_charging_reserve_percent")]
        public int OffGridVehicleChargingReservePower { get; set; }
    }
}
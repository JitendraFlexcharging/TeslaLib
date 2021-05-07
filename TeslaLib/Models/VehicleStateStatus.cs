using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TeslaLib.Models
{
    // As of August 2017, a 2014 Model S returns:
    // {"response":{"api_version":3,"autopark_state":"unavailable","autopark_state_v2":"unavailable","calendar_supported":true,
    // "car_type":"s","car_version":"2017.32 9ea02cb","center_display_state":0,"dark_rims":false,"df":0,"dr":0,
    // "exterior_color":"Red","ft":0,"has_spoiler":true,"locked":true,"notifications_supported":true,"odometer":32490.440953,
    // "parsed_calendar_supported":true,"perf_config":"P2","pf":0,"pr":0,"rear_seat_heaters":0,"rear_seat_type":0,
    // "remote_start":false,"remote_start_supported":true,"rhd":false,"roof_color":"None","rt":0,"seat_type":0,
    // "spoiler_type":"Passive","sun_roof_installed":1,"sun_roof_percent_open":0,"sun_roof_state":"unknown",
    // "third_row_seats":"None","timestamp":1503881911969,"valet_mode":false,"vehicle_name":"Hope Bringer","wheel_type":"Base19"}}
    //
    // As of Sept 2019, many fields moved to the VehicleConfig type.  The same 2014 Model S returns:
    // {"response":{"api_version":6,"autopark_state_v2":"unavailable","calendar_supported":true,
    // "car_version":"2019.32.2.1 9b8d6cd","center_display_state":0,"df":0,"dr":0,
    // "ft":0,"is_user_present":false,"locked":false,"media_state":{"remote_control_enabled":true},"notifications_supported":true,"odometer":42049.084757,
    // "parsed_calendar_supported":true,"pf":0,"pr":0,
    // "remote_start":false,"remote_start_enabled":true,"remote_start_supported":true,"rt":0,
    // "software_update":{"expected_duration_sec":2700,"status":""},
    // "speed_limit_mode":{"active":false,"current_limit_mph":85.0,"max_limit_mph":90,"min_limit_mph":55,"pin_code_set":false},
    // "sun_roof_percent_open":0,"sun_roof_state":"unknown",
    // "timestamp":1568998570067,"valet_mode":false,"valet_pin_needed":true,"vehicle_name":"Hope Bringer"}}
    //
    // Sept 2019, from a 2018 Model 3 Performance:
    // {"response":{"api_version":6,"autopark_state_v3":"ready","autopark_style":"dead_man","calendar_supported":true,
    // "car_version":"2019.32.2.1 9b8d6cd","center_display_state":0,"df":0,"dr":0,"ft":0,"homelink_nearby":false,
    // "is_user_present":false,"last_autopark_error":"no_error","locked":true,"media_state":{"remote_control_enabled":true},
    // "notifications_supported":true,"odometer":3505.813094,"parsed_calendar_supported":true,"pf":0,"pr":0,
    // "remote_start":false,"remote_start_enabled":true,"remote_start_supported":true,"rt":0,"sentry_mode":false,
    // "sentry_mode_available":true,"software_update":{"expected_duration_sec":2700,"status":""},
    // "speed_limit_mode":{"active":false,"current_limit_mph":90.0,"max_limit_mph":90,"min_limit_mph":50,
    // "pin_code_set":false},"sun_roof_percent_open":null,"sun_roof_state":"unknown",
    // "timestamp":1568999504491,"valet_mode":false,"valet_pin_needed":true,"vehicle_name":"Blue Lightning"}}
    //
    // As of March 2020, the software update data structure changed:
    // "software_update":{"download_perc":0,"expected_duration_sec":2700,"install_perc":1,"status":"","version":""},
    // 
    // API version 8, July 2020, Model X:
    // {"response":{"api_version":8,"autopark_state_v2":"standby","autopark_style":"standard","calendar_supported":true,
    // "car_version":"2020.20.12 d2c8a3e110f4","center_display_state":0,"df":0,"dr":0,"fd_window":2,"fp_window":0,"ft":16,
    // "homelink_device_count":null,"homelink_nearby":null,"is_user_present":false,"last_autopark_error":"no_error","locked":false,
    // "media_state":{"remote_control_enabled":true},"notifications_supported":true,"odometer":68727.222216,"parsed_calendar_supported":true,
    // "pf":0,"pr":0,"rd_window":0,"remote_start":null,"remote_start_enabled":true,"remote_start_supported":true,"rp_window":0,"rt":32,
    // "sentry_mode":false,"sentry_mode_available":true,"smart_summon_available":true,"software_update":{"download_perc":53,"expected_duration_sec":2700,
    // "install_perc":3,"status":"downloading","version":"2020.20.17"},"speed_limit_mode":{"active":false,"current_limit_mph":50.0,
    // "max_limit_mph":90,"min_limit_mph":50,"pin_code_set":false},"summon_standby_mode_enabled":true,"timestamp":1595713699831,"valet_mode":false,
    // "valet_pin_needed":true,"vehicle_name":"The X"}}
    // However Locked came back as null for some reason.  (Maybe a throttling related thing.)
    public class VehicleStateStatus
    {
        [JsonProperty(PropertyName = "df")]
        public bool IsDriverFrontDoorOpen { get; set; }

        [JsonProperty(PropertyName = "dr")]
        public bool IsDriverRearDoorOpen { get; set; }

        [JsonProperty(PropertyName = "pf")]
        public bool IsPassengerFrontDoorOpen { get; set; }

        [JsonProperty(PropertyName = "pr")]
        public bool IsPassengerRearDoorOpen { get; set; }

        [JsonProperty(PropertyName = "ft")]
        public bool IsFrontTrunkOpen { get; set; }

        [JsonProperty(PropertyName = "rt")]
        public bool IsRearTrunkOpen { get; set; }

        /// <summary>
        /// Car firmware version, like 2019.32.2.1 9b8d6cd
        /// </summary>
        [JsonProperty(PropertyName = "car_version")]
        public string CarVersion { get; set; }

        [JsonProperty(PropertyName = "locked")]
        public bool? IsLocked { get; set; }

        [JsonProperty(PropertyName = "sun_roof_installed")]
        public bool HasPanoramicRoof { get; set; }

        [JsonProperty(PropertyName = "sun_roof_state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PanoramicRoofState PanoramicRoofState { get; set; }

        [JsonProperty(PropertyName = "sun_roof_percent_open")]
        public int? PanoramicRoofPercentOpen { get; set; }


        // Fields that exist as of August 2017:

        [JsonProperty(PropertyName = "odometer")]
        public double? Odometer { get; set; }  // Value is in miles, regardless of the car's UI settings.

        // Newer fields documented in Tim Dorr's docs as of Jan 2019
        [JsonProperty(PropertyName = "api_version")]
        public int? ApiVersion { get; set; }

        [JsonProperty(PropertyName = "autopark_state_v2")]
        public string AutoparkStateV2 { get; set; }  // "standby"

        [JsonProperty(PropertyName = "autopark_state_v3")]
        public string AutoparkStateV3 { get; set; }  // "standby"

        [JsonProperty(PropertyName = "autopark_style")]
        public string AutoparkStyle { get; set; }  // "standard"

        [JsonProperty(PropertyName = "last_autopark_error")]
        public string LastAutoparkError { get; set; }  // "no_error"

        [JsonProperty(PropertyName = "calendar_supported")]
        public bool IsCalendarSupported { get; set; }

        [JsonProperty(PropertyName = "homelink_nearby")]
        public bool? IsHomeLinkNearby { get; set; }

        [JsonProperty(PropertyName = "is_user_present")]
        public bool IsUserPresent { get; set; }

        [JsonProperty(PropertyName = "remote_start")]
        public bool? RemoteStart { get; set; }

        [JsonProperty(PropertyName = "remote_start_supported")]
        public bool RemoteStartSupported { get; set; }

        [JsonProperty(PropertyName = "valet_mode")]
        public bool IsValetMode { get; set; }

        [JsonProperty(PropertyName = "valet_pin_needed")]
        public bool IsValetPinNeeded { get; set; }

        [JsonProperty(PropertyName = "sentry_mode")]
        public bool SentryMode { get; set; }

        [JsonProperty(PropertyName = "sentry_mode_available")]
        public bool SentryModeAvailable { get; set; }

        // There is a data structure for software_update
        // There is a data structure for speed_limit_mode
    }
}
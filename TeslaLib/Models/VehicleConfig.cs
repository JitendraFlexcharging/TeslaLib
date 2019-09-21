using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TeslaLib.Models
{
    // As of Sept 2019, a 2014 Model S returns
    //{"response":{"can_accept_navigation_requests":true,"can_actuate_trunks":true,"car_special_type":"base","car_type":"models","charge_port_type":"US",
    // "eu_vehicle":false,"exterior_color":"Red","has_air_suspension":true,"has_ludicrous_mode":false,"motorized_charge_port":false,"plg":true,
    // "rear_seat_heaters":0,"rear_seat_type":0,"rhd":false,"roof_color":"None","seat_type":0,"spoiler_type":"Passive","sun_roof_installed":1,"third_row_seats":"None",
    // "timestamp":1568998399078,"trim_badging":"p85","use_range_badging":false,"wheel_type":"Base19"}}
    //
    // Sept 2019, a 2018 Model 3 Performance
    // {"response":{"can_accept_navigation_requests":true,"can_actuate_trunks":true,"car_special_type":"base","car_type":"model3","charge_port_type":"US",
    // "eu_vehicle":false,"exterior_color":"DeepBlue","has_air_suspension":false,"has_ludicrous_mode":false,"key_version":2,"motorized_charge_port":true,"plg":false,
    // "rear_seat_heaters":1,"rear_seat_type":null,"rhd":false,"roof_color":"Glass","seat_type":null,"spoiler_type":"Passive","sun_roof_installed":null,"third_row_seats":"<invalid>",
    // "timestamp":1568999945749,"use_range_badging":true,"wheel_type":"Stiletto20"}}
    public class VehicleConfig
    {
        [JsonProperty(PropertyName = "can_accept_navigation_requests")]
        public bool CanAcceptNavigationRequests { get; set; }

        [JsonProperty(PropertyName = "can_actuate_trunks")]
        public bool CanActuateTrunks { get; set; }

        [JsonProperty(PropertyName = "car_special_type")]
        public String CarSpecialType { get; set; }

        /// <summary>
        /// Examples include "models" and "models2"
        /// </summary>
        [JsonProperty(PropertyName = "car_type")]
        public String CarType { get; set; }

        [JsonProperty(PropertyName = "charge_port_type")]
        public String ChargePortType{ get; set; }

        [JsonProperty(PropertyName = "eu_vehicle")]
        public bool EUVehicle { get; set; }

        // Note: We should use the TeslaColor enum here, but this returns values like "Red" vs. "MULTICOAT_RED"
        [JsonProperty(PropertyName = "exterior_color")]
        //[JsonConverter(typeof(StringEnumConverter))]
        public /*TeslaColor*/string ExteriorColor { get; set; }

        [JsonProperty(PropertyName = "has_air_suspension")]
        public bool HasAirSuspension{ get; set; }

        [JsonProperty(PropertyName = "has_ludicrous_mode")]
        public bool HasLudicrousMode { get; set; }

        /// <summary>
        /// There are at least two versions of key fobs.
        /// </summary>
        [JsonProperty(PropertyName = "key_version")]
        public int KeyVersion { get; set; }

        [JsonProperty(PropertyName = "motorized_charge_port")]
        public bool MotorizedChargePort { get; set; }

        [JsonProperty(PropertyName = "perf_config")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PerformanceConfiguration PerformanceConfiguration { get; set; }

        // ?
        [JsonProperty(PropertyName = "plg")]
        public bool Plg { get; set; }

        [JsonProperty(PropertyName = "rear_seat_heaters")]
        public int RearSeatHeaters { get; set; }

        [JsonProperty(PropertyName = "rear_seat_type")]
        public int? RearSeatType { get; set; }

        [JsonProperty(PropertyName = "rhd")]
        public bool RightHandDrive { get; set; }


        [JsonProperty(PropertyName = "roof_color")]
        [JsonConverter(typeof(StringEnumConverter))]
        public RoofType RoofColor { get; set; }

        [JsonProperty(PropertyName = "seat_type")]
        public int? SeatType { get; set; }

        /// <summary>
        /// Values can be "None" or "Passive", maybe more like the Model X's active?
        /// </summary>
        [JsonProperty(PropertyName = "spoiler_type")]
        public String SpoilerType { get; set; }

        /// <summary>
        /// Can be 2, or can be null on a Model 3
        /// </summary>
        [JsonProperty(PropertyName = "sun_roof_installed")]
        public int? SunRoofInstalled { get; set; }

        /// <summary>
        /// "None" and "<invalid>" have been observed.
        /// </summary>
        [JsonProperty(PropertyName = "third_row_seats")]
        public string ThirdRowSeats { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long TimeStamp { get; set; }

        /// <summary>
        /// Something like "p90d"
        /// </summary>
        [JsonProperty(PropertyName = "trim_badging")]
        public string TrimBadging { get; set; }

        [JsonProperty(PropertyName = "use_range_badging")]
        public bool UseRangeBadging { get; set; }

        // We have an enum for WheelType.  It may not be complete.  Tesla may introduce new wheel types at any time.
        // Let's try to be more resilient to that.  Sadly, that means using a String.  Maybe we could write our own StringEnumConverter
        // that converts to Unknown if it sees something novel.
        /*
        [JsonProperty(PropertyName = "wheel_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public WheelType WheelType { get; set; }
        */
        [JsonProperty(PropertyName = "wheel_type")]
        public String WheelType { get; set; }

    }
}
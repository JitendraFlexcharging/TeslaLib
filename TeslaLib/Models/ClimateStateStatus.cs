using Newtonsoft.Json;

namespace TeslaLib.Models
{
    // Response from a Model S in October 2017:
    // {"response":{"inside_temp":17.9,"outside_temp":13.5,"driver_temp_setting":22.2,"passenger_temp_setting":22.2,"left_temp_direction":196,
    //  "right_temp_direction":196,"is_auto_conditioning_on":false,"is_front_defroster_on":false,"is_rear_defroster_on":false,"fan_status":0,
    //  "is_climate_on":false,"min_avail_temp":15.0,"max_avail_temp":28.0,"seat_heater_left":2,"seat_heater_right":0,"seat_heater_rear_left":0,
    //  "seat_heater_rear_right":0,"seat_heater_rear_center":0,"seat_heater_rear_right_back":0,"seat_heater_rear_left_back":0,
    //  "smart_preconditioning":false,"timestamp":1509222155369}}
    public class ClimateStateStatus
    {

        public ClimateStateStatus()
        {

        }

        /// <summary>
        /// Degrees C inside the car
        /// </summary>
        [JsonProperty(PropertyName = "inside_temp")]
        public double? InsideTemperature { get; set; }

        /// <summary>
        /// Degrees C outside of the car
        /// </summary>
        [JsonProperty(PropertyName = "outside_temp")]
        public double? OutsideTemperature { get; set; }

        /// <summary>
        /// Degrees C of the driver temperature setpoint
        /// </summary>
        [JsonProperty(PropertyName = "driver_temp_setting")]
        public double DriverTemperatureSetting { get; set; }

        /// <summary>
        /// Degrees C of the passenger temperature setpoint
        /// </summary>
        [JsonProperty(PropertyName = "passenger_temp_setting")]
        public double PassengerTemperatureSetting { get; set; }

        /// <summary>
        /// Not clear what this is.  Automated vent control?
        /// </summary>
        [JsonProperty(PropertyName = "left_temp_direction")]
        public int LeftTemperatureDirection { get; set; }

        /// <summary>
        /// Not clear what this is.  Automated vent control?
        /// </summary>
        [JsonProperty(PropertyName = "right_temp_direction")]
        public int RightTemperatureDirection { get; set; }

        [JsonProperty(PropertyName = "is_auto_conditioning_on")]
        public bool? IsAutoAirConditioning { get; set; }

        [JsonProperty(PropertyName = "is_front_defroster_on")]
        public bool? IsFrontDefrosterOn { get; set; }

        [JsonProperty(PropertyName = "is_rear_defroster_on")]
        public bool? IsRearDefrosterOn { get; set; }

        /// <summary>
        /// Fan Speed
        /// 0-6 or null
        /// </summary>
        [JsonProperty(PropertyName = "fan_status")]
        public int? FanStatus { get; set; }

        //"is_climate_on":false,"min_avail_temp":15.0,"max_avail_temp":28.0,"seat_heater_left":2,"seat_heater_right":0,
        // "seat_heater_rear_left":0,"seat_heater_rear_right":0,"seat_heater_rear_center":0,"seat_heater_rear_right_back":0,
        //"seat_heater_rear_left_back":0,"smart_preconditioning":false

        [JsonProperty(PropertyName = "is_climate_on")]
        public bool? IsClimateOn { get; set; }

        // Not clear what this is.  Max and min air conditioning setting values?
        [JsonProperty(PropertyName = "min_avail_temp")]
        public double? MinAvailTemperature { get; set; }

        // Not clear what this is.  Max and min air conditioning setting values?
        [JsonProperty(PropertyName = "max_avail_temp")]
        public double? MaxAvailTemperature { get; set; }

        /// <summary>
        /// Seat heater setting for left seat, 0 - 3
        /// </summary>
        [JsonProperty(PropertyName = "seat_heater_left")]
        public int SeatHeaterLeft { get; set; }

        /// <summary>
        /// Seat heater setting for right seat, 0 - 3
        /// </summary>
        [JsonProperty(PropertyName = "seat_heater_right")]
        public int SeatHeaterRight { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_left")]
        public int SeatHeaterRearLeft { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_right")]
        public int SeatHeaterRearRight { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_center")]
        public int SeatHeaterRearCenter { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_left_back")]
        public int SeatHeaterRearLeftBack { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_right_back")]
        public int SeatHeaterRearRightBack { get; set; }

        /// <summary>
        /// Whether to start warming up the car before predicted use.
        /// </summary>
        [JsonProperty(PropertyName = "smart_preconditioning")]
        public bool SmartPreconditioning { get; set; }
    }
}
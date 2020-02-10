using Newtonsoft.Json;
using System;

namespace TeslaLib.Models
{
    // Response from a Model S in October 2017:
    // {"response":{"inside_temp":17.9,"outside_temp":13.5,"driver_temp_setting":22.2,"passenger_temp_setting":22.2,"left_temp_direction":196,
    //  "right_temp_direction":196,"is_auto_conditioning_on":false,"is_front_defroster_on":false,"is_rear_defroster_on":false,"fan_status":0,
    //  "is_climate_on":false,"min_avail_temp":15.0,"max_avail_temp":28.0,"seat_heater_left":2,"seat_heater_right":0,"seat_heater_rear_left":0,
    //  "seat_heater_rear_right":0,"seat_heater_rear_center":0,"seat_heater_rear_right_back":0,"seat_heater_rear_left_back":0,
    //  "smart_preconditioning":false,"timestamp":1509222155369}}
    //
    // Model S as of 2/15/2018, with a breaking change to seat_heater settings from int to bool.  Note that seat_heater_rear_right_back is still int.
    // {"response":{"inside_temp":13.0,"outside_temp":null,"driver_temp_setting":22.2,"passenger_temp_setting":22.2,"left_temp_direction":null,
    //  "right_temp_direction":null,"is_front_defroster_on":false,"is_rear_defroster_on":false,"fan_status":0,
    //  "is_climate_on":false,"min_avail_temp":15.0,"max_avail_temp":28.0,"seat_heater_left":false,"seat_heater_right":false,"seat_heater_rear_left":false,
    //  "seat_heater_rear_right":false,"seat_heater_rear_center":false,"seat_heater_rear_right_back":0,"seat_heater_rear_left_back":0,
    //  "battery_heater":false,"battery_heater_no_power":false,"steering_wheel_heater":false,"wiper_blade_heater":false,"side_mirror_heaters":false,
    //  "is_preconditioning":false,"smart_preconditioning":false,"is_auto_conditioning_on":null,"timestamp":1518691620545}}

    // 2014 Model S P85 as of Oct 2019
    // {"response":{"battery_heater":false,"battery_heater_no_power":false,"climate_keeper_mode":"off","defrost_mode":0,"driver_temp_setting":22.8,
    //  "fan_status":0,"inside_temp":17.4,"is_auto_conditioning_on":false,"is_climate_on":false,"is_front_defroster_on":false,"is_preconditioning":false,
    //  "is_rear_defroster_on":false,"left_temp_direction":268,"max_avail_temp":28.0,"min_avail_temp":15.0,"outside_temp":13.0,"passenger_temp_setting":22.8,
    //  "remote_heater_control_enabled":false,"right_temp_direction":268,"seat_heater_left":0,"seat_heater_right":0,"side_mirror_heaters":false,
    //  "smart_preconditioning":false,"timestamp":1571607890277,"wiper_blade_heater":false}}

    // {"response":{"battery_heater":false,"battery_heater_no_power":null,"climate_keeper_mode":"off","defrost_mode":0,"driver_temp_setting":21.7,
    //  "fan_status":0,"inside_temp":null,"is_auto_conditioning_on":null,"is_climate_on":false,"is_front_defroster_on":false,"is_preconditioning":false,
    //  "is_rear_defroster_on":false,"left_temp_direction":null,"max_avail_temp":28.0,"min_avail_temp":15.0,"outside_temp":null,"passenger_temp_setting":21.7,
    //  "remote_heater_control_enabled":false,"right_temp_direction":null,"seat_heater_left":0,"seat_heater_right":0,"side_mirror_heaters":false,
    //  "timestamp":1581305100019,"wiper_blade_heater":false}}



    // 2018 Model 3 Performance as of Oct 2019.
    // {"response":{"battery_heater":false,"battery_heater_no_power":null,"climate_keeper_mode":"off","defrost_mode":0,"driver_temp_setting":22.2,
    //  "fan_status":0,"inside_temp":13.1,"is_auto_conditioning_on":false,"is_climate_on":false,"is_front_defroster_on":false,"is_preconditioning":false,
    //  "is_rear_defroster_on":false,"left_temp_direction":764,"max_avail_temp":28.0,"min_avail_temp":15.0,"outside_temp":13.0,"passenger_temp_setting":22.2,
    //  "remote_heater_control_enabled":false,"right_temp_direction":764,"seat_heater_left":0,"seat_heater_rear_center":0,"seat_heater_rear_left":0,
    //  "seat_heater_rear_right":0,"seat_heater_right":0,"side_mirror_heaters":false,"smart_preconditioning":false,"timestamp":1571608022588,"wiper_blade_heater":false}}
    public class ClimateStateStatus
    {
        /// <summary>
        /// Degrees C inside the car
        /// </summary>
        [JsonProperty(PropertyName = "inside_temp")]
        public float? InsideTemperature { get; set; }

        /// <summary>
        /// Degrees C outside of the car
        /// </summary>
        [JsonProperty(PropertyName = "outside_temp")]
        public float? OutsideTemperature { get; set; }

        /// <summary>
        /// Degrees C of the driver temperature setpoint
        /// </summary>
        [JsonProperty(PropertyName = "driver_temp_setting")]
        public float DriverTemperatureSetting { get; set; }

        /// <summary>
        /// Degrees C of the passenger temperature setpoint
        /// </summary>
        [JsonProperty(PropertyName = "passenger_temp_setting")]
        public float PassengerTemperatureSetting { get; set; }

        /// <summary>
        /// Not clear what this is.  Automated vent control?
        /// </summary>
        [JsonProperty(PropertyName = "left_temp_direction")]
        public int? LeftTemperatureDirection { get; set; }

        /// <summary>
        /// Not clear what this is.  Automated vent control?
        /// </summary>
        [JsonProperty(PropertyName = "right_temp_direction")]
        public int? RightTemperatureDirection { get; set; }

        [JsonProperty(PropertyName = "is_auto_conditioning_on")]
        public bool? IsAutoAirConditioning { get; set; }

        [JsonProperty(PropertyName = "is_front_defroster_on")]
        public bool? IsFrontDefrosterOn { get; set; }

        [JsonProperty(PropertyName = "is_rear_defroster_on")]
        public bool? IsRearDefrosterOn { get; set; }

        [JsonProperty(PropertyName = "defrost_mode")]
        public int? DefrostMode { get; set; }   // Seen 0.

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
        public float? MinAvailTemperature { get; set; }

        // Not clear what this is.  Max and min air conditioning setting values?
        [JsonProperty(PropertyName = "max_avail_temp")]
        public float? MaxAvailTemperature { get; set; }

        /* Tesla went through a breaking change with a software update around Feb 13, 2018.  However it looks like they missed changing 
         * the type of two fields, and may update them in the future.  I don't need these at the moment, so I'll comment them out until
         * many cars get the update and I understand whether they will change the last two property's type.

        /// <summary>
        /// Seat heater setting for left seat, 0 - 3
        /// </summary>
        [JsonProperty(PropertyName = "seat_heater_left")]
        public bool SeatHeaterLeft { get; set; }

        /// <summary>
        /// Seat heater setting for right seat, 0 - 3
        /// </summary>
        [JsonProperty(PropertyName = "seat_heater_right")]
        public bool SeatHeaterRight { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_left")]
        public bool SeatHeaterRearLeft { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_right")]
        public bool SeatHeaterRearRight { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_center")]
        public bool SeatHeaterRearCenter { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_left_back")]
        public int SeatHeaterRearLeftBack { get; set; }

        [JsonProperty(PropertyName = "seat_heater_rear_right_back")]
        public int SeatHeaterRearRightBack { get; set; }
        */

        /* Added in ~2019, but gone by Feb 2020
        /// <summary>
        /// Whether to start warming up the car before predicted use.
        /// </summary>
        [JsonProperty(PropertyName = "smart_preconditioning")]
        public bool SmartPreconditioning { get; set; }
        */

        // New values as of Feb 15, 2018

        /// <summary>
        /// Whether the battery heater is on.
        /// </summary>
        [JsonProperty(PropertyName = "battery_heater")]
        public bool? BatteryHeater { get; set; }

        /// <summary>
        /// Not sure.
        /// </summary>
        [JsonProperty(PropertyName = "battery_heater_no_power")]
        public bool? BatteryHeaterNoPower { get; set; }

        /// <summary>
        /// Whether the steering wheel heater is on.
        /// </summary>
        [JsonProperty(PropertyName = "steering_wheel_heater")]
        public bool? SteeringWheelHeater { get; set; }

        /// <summary>
        /// Whether the wiper blade heater is on.
        /// </summary>
        [JsonProperty(PropertyName = "wiper_blade_heater")]
        public bool? WiperBladeHeater { get; set; }

        /// <summary>
        /// Whether the wiper blade heater is on.
        /// </summary>
        [JsonProperty(PropertyName = "side_mirror_heaters")]
        public bool? SideMirrorHeaters { get; set; }
        
        /// <summary>
        /// Whether the car is preconditioning.
        /// </summary>
        [JsonProperty(PropertyName = "is_preconditioning")]
        public bool? IsPreconditioning { get; set; }

        /// <summary>
        /// Climate keeper mode.  Values are "off" and "dog".  Maybe one day, baby?
        /// </summary>
        [JsonProperty(PropertyName = "climate_keeper_mode")]
        public String ClimateKeeperMode { get; set; }
    }
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TeslaLib.Converters;

namespace TeslaLib.Models
{
    /*  Old Values:
    * {"response":
    * {"charging_state":"Charging",
    * "charge_limit_soc":81,
    * "charge_limit_soc_std":90,
    * "charge_limit_soc_min":50,
    * "charge_limit_soc_max":100,
    * "charge_to_max_range":false,
    * "battery_heater_on":false,
    * "not_enough_power_to_heat":false,
    * "max_range_charge_counter":0,
    * "fast_charger_present":false,
    * "fast_charger_type":"<invalid>",
    * "battery_range":212.05,
    * "est_battery_range":222.92,
    * "ideal_battery_range":245.31,
    * "battery_level":80,
    * "usable_battery_level":80,
    * "battery_current":9.2,
    * "charge_energy_added":11.36,
    * "charge_miles_added_rated":38.5,
    * "charge_miles_added_ideal":44.5,
    * "charger_voltage":242,
    * "charger_pilot_current":40,
    * "charger_actual_current":17,
    * "charger_power":4,
    * "time_to_full_charge":0.08,
    * "trip_charging":false,
    * "charge_rate":9.1,
    * "charge_port_door_open":true,
    * "motorized_charge_port":false,
    * "scheduled_charging_start_time":null,
    * "scheduled_charging_pending":false,
    * "user_charge_enable_request":null,
    * "charge_enable_request":true,
    * "eu_vehicle":false,
    * "charger_phases":1,
    * "charge_port_latch":"Engaged",
    * "charge_current_request":40,
    * "charge_current_request_max":40,
    * "managed_charging_active":false,
    * "managed_charging_user_canceled":false,
    * "managed_charging_start_time":null}}
    * 
    * 
    * // While SuperCharging
    *     "charging_state": "Charging",
    "battery_current": 206.7,
    "battery_heater_on": false,
    "battery_level": 47,
    "battery_range": 125.08,
    "charge_enable_request": true,
    "charge_limit_soc": 95,
    "charge_limit_soc_max": 100,
    "charge_limit_soc_min": 50,
    "charge_limit_soc_std": 90,
    "charge_port_door_open": true,
    "charge_rate": 342.6,
    "charge_to_max_range": true,
    "charger_actual_current": 0,
    "charger_pilot_current": 0,
    "charger_power": 78,
    "charger_voltage": 377,
    "est_battery_range": 117.77,
    "fast_charger_present": true,
    "ideal_battery_range": 144.71,
    "max_range_charge_counter": 0,
    "not_enough_power_to_heat": false,
    "scheduled_charging_pending": false,
    "scheduled_charging_start_time": null,
    "time_to_full_charge": 0.83,
    "user_charge_enable_request": null,
    "trip_charging": false,
    "charger_phases": null,
    "motorized_charge_port": false,
    "fast_charger_type": "Tesla",
    "usable_battery_level": 47,
    "charge_energy_added": 29.44,
    "charge_miles_added_rated": 100.0,
    "charge_miles_added_ideal": 115.5,
    "eu_vehicle": false,
    "charge_port_latch": "Engaged",
    "charge_current_request": 80,
    "charge_current_request_max": 80,
    "managed_charging_active": false,
    "managed_charging_user_canceled": false,
    "managed_charging_start_time": null
    */

    /* Oct 2019
     * 2014 Model S, not running Tesla v10 software:
     * {"response":{
     * "battery_heater_on":false,"battery_level":46,"battery_range":118.12,
     * "charge_current_request":80,"charge_current_request_max":80,"charge_enable_request":true,"charge_energy_added":0.0,
     * "charge_limit_soc":80,"charge_limit_soc_max":100,"charge_limit_soc_min":50,"charge_limit_soc_std":90,
     * "charge_miles_added_ideal":0.0,"charge_miles_added_rated":0.0,"charge_port_cold_weather_mode":null,
     * "charge_port_door_open":false,"charge_port_latch":"Engaged","charge_rate":0.0,
     * "charge_to_max_range":false,"charger_actual_current":0,"charger_phases":null,
     * "charger_pilot_current":80,"charger_power":0,"charger_voltage":0,"charging_state":"Disconnected",
     * "conn_charge_cable":"<invalid>","est_battery_range":104.06,"fast_charger_brand":"<invalid>","fast_charger_present":false,
     * "fast_charger_type":"<invalid>","ideal_battery_range":136.65,
     * "managed_charging_active":false,"managed_charging_start_time":null,"managed_charging_user_canceled":false,
     * "max_range_charge_counter":0,"minutes_to_full_charge":0,"not_enough_power_to_heat":false,
     * "scheduled_charging_pending":false, "scheduled_charging_start_time":null,
     * "time_to_full_charge":0.0,
     * "timestamp":1570005110407,"trip_charging":false,"usable_battery_level":46,"user_charge_enable_request":null}}
    */
    /* Nov 2019
     * 2018 Model 3, managed charging options enabled & scheduled:
     * "managed_charging_active":false,"managed_charging_start_time":null,"managed_charging_user_canceled":false,
     * "scheduled_charging_pending":true,"scheduled_charging_start_time":1573649400,"scheduled_departure_time":1573671600,
     */
    /* Mar 2022
     * 2018 Model 3, scheduled charging enabled.
     * "managed_charging_active":false,"managed_charging_start_time":null,"managed_charging_user_canceled":false,
     * "scheduled_charging_mode":"StartAt","scheduled_charging_pending":true,"scheduled_charging_start_time":1646897400,
     * "scheduled_charging_start_time_app":1410,"scheduled_charging_start_time_minutes":1410,
     * "scheduled_departure_time":1646161200,"scheduled_departure_time_minutes":660,
     * 
     * Scheduled departure time enabled with off-peak charging:
     * "off_peak_charging_enabled":true,"off_peak_charging_times":"all_week","off_peak_hours_end_time":360,
     * "preconditioning_enabled":false,"preconditioning_times":"all_week",
     * A consistent set of values where the app was set for scheduled charge start time at 11:30 PM, then
     * we disabled that feature and set up a scheduled departure time.
     * Scheduled charging mode: DepartBy
     * Scheduled charging time: 3/10/2022 3:25:00 AM  Minutes: 205
     * Scheduled charging start time app: 1410  (11:30 PM in minutes)
     */
    /* July 2022
     * 2018 Model 3, with setting the max charging amperage to 19 amps.
     * Look at these:  "charge_amps":19,"charge_current_request":19,"charge_current_request_max":40
     * 
     * {"response":{"battery_heater_on":false,"battery_level":53,"battery_range":156.47,
     * "charge_amps":19,"charge_current_request":19,"charge_current_request_max":40,"charge_enable_request":true,
     * "charge_energy_added":0.21,"charge_limit_soc":71,"charge_limit_soc_max":100,"charge_limit_soc_min":50,"charge_limit_soc_std":90,
     * "charge_miles_added_ideal":1.0,"charge_miles_added_rated":1.0,
     * "charge_port_cold_weather_mode":false,"charge_port_color":"<invalid>","charge_port_door_open":true,"charge_port_latch":"Engaged",
     * "charge_rate":17.2,"charge_to_max_range":false,
     * "charger_actual_current":19,
     * "charger_phases":1,"charger_pilot_current":40,"charger_power":5,"charger_voltage":245,"charging_state":"Charging",
     * "conn_charge_cable":"SAE","est_battery_range":181.57,"fast_charger_brand":"<invalid>","fast_charger_present":false,
     * "fast_charger_type":"<invalid>","ideal_battery_range":156.47,
     * "managed_charging_active":false,"managed_charging_start_time":null,"managed_charging_user_canceled":false,"max_range_charge_counter":0,
     * "minutes_to_full_charge":185,"not_enough_power_to_heat":null,
     * "off_peak_charging_enabled":false,"off_peak_charging_times":"all_week","off_peak_hours_end_time":360,
     * "preconditioning_enabled":false,"preconditioning_times":"all_week",
     * "scheduled_charging_mode":"Off","scheduled_charging_pending":false,"scheduled_charging_start_time":null,"scheduled_charging_start_time_app":1020,
     * "scheduled_departure_time":1649008800,"scheduled_departure_time_minutes":660,"supercharger_session_trip_planner":false,
     * "time_to_full_charge":3.08,"timestamp":1657869650448,"trip_charging":false,"usable_battery_level":53,"user_charge_enable_request":null}}
     */
    public class ChargeStateStatus
{
    // Note: the ChargingState started coming back as null around June 2017, coinciding with a significant
    // Tesla software update.  They apparently upgraded from Linux kernel 2.6.36 to 4.4.35.  They may have changed
    // a lot of Tesla's software stack too.
    [JsonProperty(PropertyName = "charging_state")]
    [JsonConverter(typeof(StringEnumConverter))]
    public ChargingState? ChargingState { get; set; }

    [JsonProperty(PropertyName = "battery_heater_on")]
    public bool? IsBatteryHeaterOn { get; set; }

    [JsonProperty(PropertyName = "battery_level")]
    public int? BatteryLevel { get; set; }

    [JsonProperty(PropertyName = "battery_range")]
    public double? BatteryRange { get; set; }

    [JsonProperty(PropertyName = "charge_enable_request")]
    public bool IsChargeEnableRequest { get; set; }

    [JsonProperty(PropertyName = "charge_limit_soc")]
    public int ChargeLimitSoc { get; set; }

    [JsonProperty(PropertyName = "charge_limit_soc_max")]
    public int ChargeLimitSocMax { get; set; }

    [JsonProperty(PropertyName = "charge_limit_soc_min")]
    public int ChargeLimitSocMin { get; set; }

    [JsonProperty(PropertyName = "charge_limit_soc_std")]
    public int ChargeLimitSocStd { get; set; }

    [JsonProperty(PropertyName = "charge_port_door_open")]
    public bool? IsChargePortDoorOpen { get; set; }

    [JsonProperty(PropertyName = "charge_rate")]
    public double ChargeRate { get; set; }

    // No longer returned as of Jan 2016.
    //[JsonProperty(PropertyName = "charge_starting_range")]
    //public int? ChargeStartingRange { get; set; }

    //[JsonProperty(PropertyName = "charge_starting_soc")]
    //public int? ChargeStartingSoc { get; set; }

    [JsonProperty(PropertyName = "charge_to_max_range")]
    public bool IsChargeToMaxRange { get; set; }

    [JsonProperty(PropertyName = "charger_actual_current")]
    public int? ChargerActualCurrent { get; set; }

    [JsonProperty(PropertyName = "charger_pilot_current")]
    public int? ChargerPilotCurrent { get; set; }

    [JsonProperty(PropertyName = "charger_power")]
    public int? ChargerPower { get; set; }

    [JsonProperty(PropertyName = "charger_voltage")]
    public int? ChargerVoltage { get; set; }           // null when a car is starting to charge.

    [JsonProperty(PropertyName = "est_battery_range")]
    public double? EstimatedBatteryRange { get; set; }

    [JsonProperty(PropertyName = "fast_charger_present")]
    public bool? IsUsingSupercharger { get; set; }

    [JsonProperty(PropertyName = "ideal_battery_range")]
    public double? IdealBatteryRange { get; set; }

    [JsonProperty(PropertyName = "max_range_charge_counter")]
    public int? MaxRangeChargeCounter { get; set; }

    [JsonProperty(PropertyName = "not_enough_power_to_heat")]
    public bool? IsNotEnoughPowerToHeat { get; set; }

    [JsonProperty(PropertyName = "scheduled_charging_pending")]
    public bool ScheduledChargingPending { get; set; }

    // This is a Unix time value in seconds from 1970 in UTC.
    // We need to use a JsonConverter to make this work.
    [JsonProperty(PropertyName = "scheduled_charging_start_time")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? ScheduledChargingStartTime { get; set; }

    // This is a Unix time value in seconds from 1970 in UTC.
    // We need to use a JsonConverter to make this work.
    [JsonProperty(PropertyName = "scheduled_departure_time")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? ScheduledDepartureTime { get; set; }

    // This is a time of day in minutes.  IE, 660 is 11:00 AM
    // Seems like this is redundant with scheduled_charging_start_time_minutes.
    [JsonProperty(PropertyName = "scheduled_departure_time_minutes")]
    public int? ScheduledDepartureTimeMinutes { get; set; }

    // This is a time of day in minutes.  IE, 1410 is 11:30 PM (23.5 hours)
    // Seems like this is redundant with scheduled_charging_start_time_minutes.
    [JsonProperty(PropertyName = "scheduled_charging_start_time_app")]
    public int? ScheduledChargingStartTimeApp { get; set; }

    // This is a time of day in minutes.  IE, 1410 is 11:20 PM (23.5 hours)
    [JsonProperty(PropertyName = "scheduled_charging_start_time_minutes")]
    public int? ScheduledChargingStartTimeMinutes { get; set; }

    [JsonProperty(PropertyName = "scheduled_charging_mode")]
    [JsonConverter(typeof(StringEnumConverter))]
    public ScheduledChargingMode ScheduledChargingMode { get; set; }


    // Hours
    [JsonProperty(PropertyName = "time_to_full_charge")]
    public double? TimeUntilFullCharge { get; set; }

    [JsonProperty(PropertyName = "minutes_to_full_charge")]
    public int MinutesUntilFullCharge { get; set; }

    [JsonProperty(PropertyName = "user_charge_enable_request")]
    public bool? IsUserChargeEnableRequest { get; set; }

    // Updates to Tesla API's
    // Updated at an unknown time

    [JsonProperty(PropertyName = "trip_charging")]
    public bool? IsTripCharging { get; set; }

    [JsonProperty(PropertyName = "charger_phases")]
    public int? ChargerPhases { get; set; }

    [JsonProperty(PropertyName = "motorized_charge_port")]
    public bool? IsMotorizedChargePort { get; set; }

    // Seen values "\u003Cinvalid\u003E"
    [JsonProperty(PropertyName = "fast_charger_type")]
    public string FastChargerType { get; set; }

    [JsonProperty(PropertyName = "usable_battery_level")]
    public int? UsableBatteryLevel { get; set; }

    [JsonProperty(PropertyName = "charge_energy_added")]
    public double? ChargeEnergyAdded { get; set; }

    [JsonProperty(PropertyName = "charge_miles_added_rated")]
    public double? ChargeMilesAddedRated { get; set; }

    [JsonProperty(PropertyName = "charge_miles_added_ideal")]
    public double? ChargeMilesAddedIdeal { get; set; }

    [JsonProperty(PropertyName = "eu_vehicle")]
    public bool IsEUVehicle { get; set; }

    // Updates to Tesla API's around December 2015:
    // Updated firmware from v7.0 (2.7.56) to v7(2.9.12) Some new fields added:

    [JsonProperty(PropertyName = "charge_port_latch")]
    public string ChargePortLatch { get; set; }  // "Engaged"

    [JsonProperty(PropertyName = "charge_current_request")]
    public int? ChargeCurrentRequest { get; set; }  // amps

    [JsonProperty(PropertyName = "charge_current_request_max")]
    public int? ChargeCurrentRequestMax { get; set; }  // amps

    [JsonProperty(PropertyName = "managed_charging_active")]
    public bool? ManagedChargingActive { get; set; }

    [JsonProperty(PropertyName = "managed_charging_user_canceled")]
    public bool? ManagedChargingUserCanceled { get; set; }

    [JsonProperty(PropertyName = "managed_charging_start_time")]
    public DateTime? ManagedChargingStartTime { get; set; }

    // Updates as of January 2019

    [JsonProperty(PropertyName = "conn_charge_cable")]
    public string ConnChargeCable { get; set; } // "<invalid>", "SAE"

    [JsonProperty(PropertyName = "fast_charger_brand")]
    public string FastChargerBrand { get; set; }  // "<invalid>"

    [JsonProperty(PropertyName = "off_peak_charging_enabled")]
    public bool OffPeakChargingEnabled { get; set; }

    [JsonProperty(PropertyName = "off_peak_charging_times")]
    [JsonConverter(typeof(StringEnumConverter))]
    public WeekTimes OffPeakChargingTimes { get; set; }

    // Time of day in minutes
    [JsonProperty(PropertyName = "off_peak_hours_end_time")]
    public int? OffPeakHoursEndTime { get; set; }

    [JsonProperty(PropertyName = "preconditioning_enabled")]
    public bool PreconditioningEnabled { get; set; }

    [JsonProperty(PropertyName = "preconditioning_times")]
    public WeekTimes PreconditioningTimes { get; set; }
}
}

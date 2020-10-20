using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeslaLib.Models;

namespace TeslaLibTests
{
    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void DeserializeNullsInChargeStateStatus()
        {
            // We got back some charge state status values with mostly null in them.  Let's make sure we don't fail as a result.
            string jsonWithNulls = "{\"response\":{ \"battery_heater_on\":false,\"battery_level\":null,\"battery_range\":null,\"charge_current_request\":32,\"charge_current_request_max\":32," +
                "\"charge_enable_request\":true,\"charge_energy_added\":23.06,\"charge_limit_soc\":90,\"charge_limit_soc_max\":100,\"charge_limit_soc_min\":50,\"charge_limit_soc_std\":90," +
                "\"charge_miles_added_ideal\":90.5,\"charge_miles_added_rated\":78.0,\"charge_port_cold_weather_mode\":null,\"charge_port_door_open\":null,\"charge_port_latch\":\"<invalid>\"," +
                "\"charge_rate\":0.0,\"charge_to_max_range\":false,\"charger_actual_current\":null,\"charger_phases\":null,\"charger_pilot_current\":null,\"charger_power\":null," +
                "\"charger_voltage\":null,\"charging_state\":\"Complete\",\"conn_charge_cable\":\"<invalid>\",\"est_battery_range\":null,\"fast_charger_brand\":\"<invalid>\"," +
                "\"fast_charger_present\":null,\"fast_charger_type\":\"<invalid>\",\"ideal_battery_range\":null,\"managed_charging_active\":false,\"managed_charging_start_time\":null," +
                "\"managed_charging_user_canceled\":false,\"max_range_charge_counter\":0,\"minutes_to_full_charge\":0,\"not_enough_power_to_heat\":null,\"scheduled_charging_pending\":false," +
                "\"scheduled_charging_start_time\":null,\"time_to_full_charge\":0.0,\"timestamp\":1603190060833,\"trip_charging\":null,\"usable_battery_level\":null,\"user_charge_enable_request\":null}}";

            var json = JObject.Parse(jsonWithNulls)["response"];
            var data = JsonConvert.DeserializeObject<ChargeStateStatus>(json.ToString());
            // If we got here, great.
        }
    }
}

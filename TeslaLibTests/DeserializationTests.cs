using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TeslaLib;
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
        [TestMethod]
        public void DeserializeNullInTeslaVehicle()
        {
            //This response was throwing an error due to null option_codes in seconds device
            string jsonWithNull = "{\"response\":[{\"id\":1492932705572515,\"vehicle_id\":520539554,\"vin\":\"5YJ3E1EA1LF736733\",\"display_name\":\"Chugster\",\"option_codes\":\"AD15,AF00,APFB,APH4,AU3D,BC3B,BT35,RNG0,CDM0,CH05,COUS,DRLH,DV2W,FC01,FG30,FM3S,GLFR,HL31,HM30,ID3W,IL31,LTSB,MDL3,MR30,PMNG,PC30,RENA,RF3G,RS3H,S3PB,SA3P,SC04,STCP,SU3C,T3MA,TM00,TW00,UT3P,W38B,WR00,ZINV,MI01,PL30,SLR0,ST30,BG30,I36M,USSB,AUF2,RSF0,ILF0,FGF0,CPF0,P3WS,HP30,PT00\",\"color\":null,\"access_type\":\"OWNER\",\"tokens\":[\"e4251a6f8e7ad39f\",\"d0f179461e40fc5c\"],\"state\":\"asleep\",\"in_service\":false,\"id_s\":\"1492932705572515\",\"calendar_enabled\":true,\"api_version\":40,\"backseat_token\":null,\"backseat_token_updated_at\":null},{\"id\":3744493103938796,\"vehicle_id\":2252112704759789,\"vin\":\"7SAYGDEE5NF445694\",\"display_name\":\"PandaSpots\",\"option_codes\":null,\"color\":null,\"access_type\":\"DRIVER\",\"tokens\":[\"a8cc08db4c2f4abd\",\"3c0c906bd39f0e13\"],\"state\":\"online\",\"in_service\":false,\"id_s\":\"3744493103938796\",\"calendar_enabled\":true,\"api_version\":40,\"backseat_token\":null,\"backseat_token_updated_at\":null}],\"count\":2}";
            var json = JObject.Parse(jsonWithNull)["response"];
            var d = json.ToString();
            var data = JsonConvert.DeserializeObject<List<TeslaVehicle>>(d);
        }
    }
}

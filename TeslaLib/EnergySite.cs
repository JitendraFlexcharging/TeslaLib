using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using TeslaLib.Converters;
using TeslaLib.Models;

namespace TeslaLib
{
    public class EnergySite
    {

        #region Properties

        [JsonProperty(PropertyName = "energy_site_id")]
        public string EnergySiteId { get; set; }

        [JsonProperty(PropertyName = "resource_type")]
        public string ResourceType { get; set; }

        [JsonProperty(PropertyName = "site_name")]
        public string SiteName { get; set; }

        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "gateway_id")]
        public String GatewayId { get; set; }

        [JsonProperty(PropertyName = "energy_left")]
        public double EnergyLeft { get; set; }

        [JsonProperty(PropertyName = "total_pack_energy")]
        public double TotalPackEnergy { get; set; }

        [JsonProperty(PropertyName = "percentage_charged")]
        public double StateOfCharge { get; set; }

        [JsonProperty(PropertyName = "battery_type")]
        public string BatteryType { get; set; }

        [JsonProperty(PropertyName = "backup_capable")]
        public bool BackupCapable { get; set; }

        [JsonProperty(PropertyName = "battery_power")]
        public double BatteryPower { get; set; }

        [JsonProperty(PropertyName = "sync_grid_alert_enabled")]
        public bool SyncGridAlertEnabled { get; set; }

        [JsonProperty(PropertyName = "breaker_alert_enabled")]
        public bool BreakerAlertEnabled { get; set; }


        [JsonIgnore]
        public RestClient Client { get; set; }

        #endregion Properties

        #region State and Settings

        public String GetEnergySiteStatus()
        {
            var request = new RestRequest("energy_sites/{site_id}/status");
            request.AddParameter("site_id", EnergySiteId, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<String>(response);
        }

        public String GetSiteData()
        {
            var request = new RestRequest("energy_sites/{site_id}/live_status");
            request.AddParameter("site_id", EnergySiteId, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<String>(response);
        }

        public String GetSiteConfiguration()
        {
            var request = new RestRequest("energy_sites/{site_id}/site_info");
            request.AddParameter("site_id", EnergySiteId, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<String>(response);
        }

        public String GetSiteHistory()
        {
            var request = new RestRequest("energy_sites/{site_id}/history");
            request.AddParameter("site_id", EnergySiteId, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<String>(response);
        }

        #endregion State and Settings

        #region Commands

        private T ParseResult<T>(IRestResponse response)
        {
            if (response.Content.Length == 0)
                throw new FormatException("Tesla's response was empty.");

            try
            {
                var json = JObject.Parse(response.Content)["response"];
                var data = JsonConvert.DeserializeObject<T>(json.ToString());
                return data;
            }
            catch (JsonSerializationException e)
            {
                e.Data["SerializedResponse"] = response.Content;

                // Hack - if we have an enum we can't deal with, print something out...  But we also can't not fail.
                if (e.Message.StartsWith("Error converting value "))
                {
                    TeslaClient.Logger.WriteLine("TeslaVehicle failed to deserialize something.  Need to add new enum value?  "+e);
                }
                throw;
            }
            catch (Exception e)
            {
                if (response.Content.Contains(TeslaClient.InternalServerErrorMessage))
                    throw new TeslaServerException();

                e.Data["SerializedResponse"] = response.Content;
                throw;
            }
        }

        #endregion Commands

    }
}
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    public class VehicleData
    {
        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public String UserId { get; set; }

        [JsonProperty(PropertyName = "vehicle_id")]
        public String VehicleId { get; set; }

        [JsonProperty(PropertyName = "vin")]
        public String VIN { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public String DisplayName { get; set; }

        // Comes back as null now
        [JsonProperty(PropertyName = "color")]
        public String Color { get; set; }

        // Can return OWNER
        [JsonProperty(PropertyName = "access_type")]
        public String AccessType { get; set; }

        [JsonProperty(PropertyName = "tokens")]
        public String[] Tokens { get; set; }

        [JsonProperty(PropertyName = "state")]
        public String State { get; set; }

        [JsonProperty(PropertyName = "in_service")]
        public bool InService { get; set; }

        [JsonProperty(PropertyName = "id_s")]
        public String Id_S { get; set; }

        [JsonProperty(PropertyName = "calendar_enabled")]
        public bool CalendarEnabled { get; set; }

        [JsonProperty(PropertyName = "api_version")]
        public int ApiVersion { get; set; }

        [JsonProperty(PropertyName = "backseat_token")]
        public String BackseatToken { get; set; }

        [JsonProperty(PropertyName = "backseat_token_updated_at")]
        public String BackseatTokenUpdatedAt { get; set; }

        [JsonProperty(PropertyName = "drive_state")]
        public DriveStateStatus DriveState { get; set; }

        [JsonProperty(PropertyName = "climate_state")]
        public ClimateStateStatus ClimateState { get; set; }

        [JsonProperty(PropertyName = "charge_state")]
        public ChargeStateStatus ChargeState { get; set; }

        [JsonProperty(PropertyName = "gui_settings")]
        public GuiSettingsStatus GuiSettings { get; set; }

        [JsonProperty(PropertyName = "vehicle_state")]
        public VehicleStateStatus VehicleState { get; set; }

        [JsonProperty(PropertyName = "vehicle_config")]
        public VehicleConfig VehicleConfig { get; set; }
    }
}

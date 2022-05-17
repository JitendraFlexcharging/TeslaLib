using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using TeslaLib.Converters;
using TeslaLib.Models;

namespace TeslaLib
{
    /* Oct 2019, from a 2018 Model 3:
     * {"response":[{"id":26396141727078272,"vehicle_id":1502282633,"vin":"5YJ3E1EBXJF080943","display_name":"Blue Lightning",
     * "option_codes":"AD15,MDL3,PBSB,RENA,BT37,ID3W,RF3G,S3PB,DRLH,DV2W,W39B,APF0,COUS,BC3B,CH07,PC30,FC3P,FG31,GLFR,HL31,HM31,IL31,LTPB,MR31,FM3B,RS3H,SA3P,STCP,SC04,SU3C,T3CA,TW00,TM00,UT3P,WR00,AU3P,APH3,AF00,ZCST,MI00,CDM0",
     * "color":null,"tokens":["4327e0dd67dab91b","73c94aaacdd9ecc4"],"state":"online","in_service":false,
     * "id_s":"26396141727078272","calendar_enabled":true,"api_version":6,
     * "backseat_token":null,"backseat_token_updated_at":null}],"count":1}
     */
    public class TeslaVehicle : ITeslaVehicle
    {

        #region Properties

        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "option_codes")]
        [JsonConverter(typeof(VehicleOptionsConverter))]
        public VehicleOptions Options { get; set; }

        /*
        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }
        */

        [JsonProperty(PropertyName = "vehicle_id")]
        public long VehicleId { get; set; }

        [JsonProperty(PropertyName = "vin")]
        public string Vin { get; set; }

        [JsonProperty(PropertyName = "tokens")]
        public List<string> Tokens { get; set; }

        [JsonProperty(PropertyName = "state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public VehicleState State { get; set; }

        [JsonProperty(PropertyName = "in_service")]
        public bool InService { get; set; }

        [JsonProperty(PropertyName = "calendar_enabled")]
        public bool CalendarEnabled { get; set; }

        // As of June ~29th-ish of 2020, this started returning null temporarily.  Bug in Tesla's servers?  But we shouldn't fail as a result.
        [JsonProperty(PropertyName = "api_version")]
        public float? ApiVersion { get; set; }

        [JsonIgnore]
        public RestClient Client { get; set; }

        #endregion Properties

        #region State and Settings

        public bool LoadMobileEnabledStatus()
        {
            var request = new RestRequest("vehicles/{id}/mobile_enabled");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Get(request);
            var json = JObject.Parse(response.Content)["response"];
            bool data = true;
            try
            {
                data = bool.Parse(json.ToString());
            }
            catch (FormatException e)
            {
                ReportKnownErrors(response);
                e.Data["SerializedResponse"] = response.Content;
                TeslaClient.Logger.WriteLine("LoadMobileEnabledStatus failed to parse results.  JSON: \"" + json.ToString() + "\"");
                throw;
            }

            return data;
        }

        private void ReportKnownErrors(IRestResponse response)
        {
            // Success is not a failure.
            if (response.StatusCode == HttpStatusCode.OK)
                return;

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                TeslaClient.ReportUnauthorizedAccess(response, false, null);

            if (response.StatusCode == HttpStatusCode.RequestTimeout)
            {
                // Multiple examples, including one that looks just messed up from Tesla's side
                // {"error": "timeout"}    // Timeout accessing a Tesla vehicle: {"error": "timeout"}
                // "error":"vehicle unavailable: {:error=>\"vehicle unavailable:\"}"    <----  Likely the Tesla is asleep.
                String errorMessage = ParseErrorFromJson(response);
                //TeslaClient.Logger.WriteLine("TeslaLib debug: RequestTimeout handling.  Error: {0}  Content: {1}", errorMessage, response.Content);
                String timeoutMessage = "Timeout accessing a Tesla vehicle";
                if (!String.IsNullOrWhiteSpace(errorMessage) && errorMessage != "{\"error\": \"timeout\"}")
                    timeoutMessage += ": " + errorMessage;
                throw new TimeoutException(timeoutMessage);
            }

            if (response.Content == TeslaClient.ThrottlingMessage || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var throttled = new TeslaThrottlingException();
                throttled.Data["StatusCode"] = response.StatusCode;
                TeslaClient.Logger.WriteLine("Tesla account for car {0} named {1} was throttled by Tesla.  StatusCode: {2}", Vin, DisplayName, response.StatusCode);
                throw throttled;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests && response.Content.Trim() == TeslaClient.RetryLaterMessage)
            {
                var throttled = new TeslaThrottlingException();
                throttled.Data["StatusCode"] = response.StatusCode;
                TeslaClient.Logger.WriteLine("Tesla account for car {0} named {1} said we should retry later.  StatusCode: {2}", Vin, DisplayName, response.StatusCode);
                throw throttled;
            }

            if (response.StatusCode == (HttpStatusCode)444 && response.Content == TeslaClient.BlockedMessage)
            {
                var blocked = new TeslaBlockedException();
                blocked.Data["StatusCode"] = response.StatusCode;
                TeslaClient.Logger.WriteLine("Tesla server blocked access to your machine when asking for car {0} named {1}.  Retry in an hour with the correct password?  StatusCode: {2}", Vin, DisplayName, response.StatusCode);
                throw blocked;
            }

            // Saw this once.
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                var serviceDownOrAggressivelyDisconnectingUs = new Exception("Tesla's service is unavailable");
                serviceDownOrAggressivelyDisconnectingUs.Data["StatusCode"] = response.StatusCode;
                serviceDownOrAggressivelyDisconnectingUs.Data["Error"] = response.Content;
                TeslaClient.Logger.WriteLine("Tesla server is either down or aggressively disconnecting us.  Message: {0}", response.Content);
                throw serviceDownOrAggressivelyDisconnectingUs;
            }

            if (response.StatusCode == HttpStatusCode.MethodNotAllowed)
            {
                // {"response":null,"error":"vehicle is currently in service","error_description":""}
                String errorMessage = ParseErrorFromJson(response);
                if (!String.IsNullOrWhiteSpace(errorMessage))
                    throw new VehicleNotAvailableException(errorMessage);
                else
                    throw new VehicleNotAvailableException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // {"response":null,"error":"not_found","error_description":""}
                String errorMessage = ParseErrorFromJson(response);
                if (!String.IsNullOrWhiteSpace(errorMessage) && errorMessage != "not_found")
                    throw new VehicleNotFoundException(errorMessage);
                else
                    throw new VehicleNotFoundException();
            }

            TeslaClient.Logger.WriteLine("Unrecognized TeslaLib error.  Status code: {0}  Response content: \"{1}\"  Content length: {2}", response.StatusCode, response.Content, response.Content.Length);
        }

        private static string ParseErrorFromJson(IRestResponse response)
        {
            var errorJson = JObject.Parse(response.Content)["error"];
            var error = errorJson.ToString();
            //TeslaClient.Logger.WriteLine("TeslaLib ParseErrorFromJson debugging: error: {0}", error);
            if (!String.IsNullOrWhiteSpace(error))
                return error;
            return null;
        }

        public ChargeStateStatus LoadChargeStateStatus()
        {
            var request = new RestRequest("vehicles/{id}/data_request/charge_state");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<ChargeStateStatus>(response, timeoutMeansReturnNull: false);
        }

        public ClimateStateStatus LoadClimateStateStatus()
        {
            var request = new RestRequest("vehicles/{id}/data_request/climate_state");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<ClimateStateStatus>(response, timeoutMeansReturnNull: true);
        }

        public DriveStateStatus LoadDriveStateStatus()
        {
            var request = new RestRequest("vehicles/{id}/data_request/drive_state");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<DriveStateStatus>(response, timeoutMeansReturnNull: true);
        }

        public GuiSettingsStatus LoadGuiStateStatus()
        {
            var request = new RestRequest("vehicles/{id}/data_request/gui_settings");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<GuiSettingsStatus>(response, timeoutMeansReturnNull: true);
        }

        public VehicleConfig LoadVehicleConfig()
        {
            var request = new RestRequest("vehicles/{id}/data_request/vehicle_config");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<VehicleConfig>(response, timeoutMeansReturnNull: true);
        }

        public VehicleStateStatus LoadVehicleStateStatus()
        {
            var request = new RestRequest("vehicles/{id}/data_request/vehicle_state");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Get(request);
            return ParseResult<VehicleStateStatus>(response, timeoutMeansReturnNull: true);
        }

        #endregion State and Settings

        #region Commands

        public VehicleState WakeUp()
        {
            var request = new RestRequest("vehicles/{id}/wake_up");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            // Commonly, we will get back that the vehicle is unavailable.
            if (response.StatusCode == HttpStatusCode.RequestTimeout)
                return VehicleState.Asleep;

            JToken json = null;
            try
            {
                json = JObject.Parse(response.Content)["response"];
            }
            catch (JsonReaderException e)
            {
                ReportKnownErrors(response);

                if (response.Content.Contains(TeslaClient.InternalServerErrorMessage))
                    throw new TeslaServerException();

                // Every once in a while, WakeUp will throw a JsonReaderException.  Let's see why.
                e.Data["SerializedResponse"] = response.Content;
                TeslaClient.Logger.WriteLine("Wakeup failed to parse results.  response: \"" + response.Content + "\"");
                throw;
            }

            TeslaVehicle data;
            try
            {
                data = JsonConvert.DeserializeObject<TeslaVehicle>(json.ToString());
            }
            catch (Exception e)
            {
                ReportKnownErrors(response);
                e.Data["SerializedResponse"] = response.Content;
                TeslaClient.Logger.WriteLine("Wakeup failed to deserialize.  JSON: \"" + json + "\"");
                throw;
            }
            return data?.State ?? VehicleState.Asleep;
        }

        public ResultStatus OpenChargePortDoor()
        {
            var request = new RestRequest("vehicles/{id}/command/charge_port_door_open");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus SetChargeLimitToStandard()
        {
            var request = new RestRequest("vehicles/{id}/command/charge_standard");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        // Don't use this very often as it damages the battery.
        public ResultStatus SetChargeLimitToMaxRange()
        {
            var request = new RestRequest("vehicles/{id}/command/charge_max_range");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus SetChargeLimit(int stateOfChargePercent)
        {
            var request = new RestRequest("vehicles/{id}/command/set_charge_limit", Method.POST);
            request.AddParameter("id", Id, ParameterType.UrlSegment);
            /*  This throws an exception - RestSharp serializes this out incorrectly, perhaps?  
            request.AddBody(new
            {
                state = "set",
                percent = socPercent
            });
            */
            request.AddParameter("state", "set");
            request.AddParameter("percent", stateOfChargePercent.ToString());

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus StartCharge()
        {
            var request = new RestRequest("vehicles/{id}/command/charge_start");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus StopCharge()
        {
            var request = new RestRequest("vehicles/{id}/command/charge_stop");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus FlashLights()
        {
            var request = new RestRequest("vehicles/{id}/command/flash_lights");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus HonkHorn()
        {
            var request = new RestRequest("vehicles/{id}/command/honk_horn");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus UnlockDoors()
        {
            var request = new RestRequest("vehicles/{id}/command/door_unlock");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus LockDoors()
        {
            var request = new RestRequest("vehicles/{id}/command/door_lock");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus SetTemperatureSettings(int driverTemp = 17, int passengerTemp = 17)
        {
            int TEMP_MAX = 32;
            int TEMP_MIN = 17;

            driverTemp = Math.Max(driverTemp, TEMP_MIN);
            driverTemp = Math.Min(driverTemp, TEMP_MAX);

            passengerTemp = Math.Max(passengerTemp, TEMP_MIN);
            passengerTemp = Math.Min(passengerTemp, TEMP_MAX);


            var request = new RestRequest("vehicles/{id}/command/set_temps?driver_temp={driver_degC}&passenger_temp={pass_degC}");
            request.AddParameter("id", Id, ParameterType.UrlSegment);
            request.AddParameter("driver_degC", driverTemp, ParameterType.UrlSegment);
            request.AddParameter("pass_degC", passengerTemp, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus StartHVAC()
        {
            var request = new RestRequest("vehicles/{id}/command/auto_conditioning_start");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus StopHVAC()
        {
            var request = new RestRequest("vehicles/{id}/command/auto_conditioning_stop");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus SetPanoramicRoofLevel(PanoramicRoofState roofState, int percentOpen = 0)
        {
            var request = new RestRequest("vehicles/{id}/command/sun_roof_control?state={state}&percent={percent}");
            request.AddParameter("id", Id, ParameterType.UrlSegment);
            request.AddParameter("state", roofState.GetEnumValue(), ParameterType.UrlSegment);

            if (roofState == PanoramicRoofState.Move)
            {
                request.AddParameter("percent", percentOpen, ParameterType.UrlSegment);
            }

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus RemoteStart(string password)
        {
            var request = new RestRequest("vehicles/{id}/command/remote_start_drive?password={password}");
            request.AddParameter("id", Id, ParameterType.UrlSegment);
            request.AddParameter("password", password, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus OpenFrontTrunk() => OpenTrunk("front");

        public ResultStatus OpenRearTrunk() => OpenTrunk("rear");

        public ResultStatus OpenTrunk(string trunkType)
        {
            var request = new RestRequest("vehicles/{id}/command/trunk_open");
            request.AddParameter("id", Id, ParameterType.UrlSegment);
            request.AddJsonBody(new
            {
                which_trunk = trunkType
            });

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus DisableValetMode() => SetValetMode(false);

        public ResultStatus EnableValetMode(int password) => SetValetMode(true, password);

        public ResultStatus SetValetMode(bool enabled, int password = 0)
        {
            var request = new RestRequest("vehicles/{id}/command/set_valet_mode");
            request.AddParameter("id", Id, ParameterType.UrlSegment);
            request.AddJsonBody(new
            {
                on = enabled,
                password
            });

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        public ResultStatus ResetValetPin()
        {
            var request = new RestRequest("vehicles/{id}/command/reset_valet_pin");
            request.AddParameter("id", Id, ParameterType.UrlSegment);

            var response = Client.Post(request);
            return ParseResult<ResultStatus>(response);
        }

        private T ParseResult<T>(IRestResponse response, bool timeoutMeansReturnNull = false)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                TeslaClient.ReportUnauthorizedAccess(response, false, null);

            if (!response.IsSuccessful)
            {
                if (timeoutMeansReturnNull && response.StatusCode == HttpStatusCode.RequestTimeout)
                    return default;
                ReportKnownErrors(response);
            }

            // This should never happen now that we are checking for known errors first, but in case we missed something...
            if (response.Content.Length == 0)
                throw new FormatException($"Tesla didn't provide data.  Error: {response.StatusCode}");

            try
            {
                var json = JObject.Parse(response.Content)["response"];
                var data = JsonConvert.DeserializeObject<T>(json.ToString());
                return data;
            }
            catch (JsonSerializationException e)
            {
                ReportKnownErrors(response);

                e.Data["SerializedResponse"] = response.Content;

                // Hack - if we have an enum we can't deal with, print something out...  But we also can't not fail.
                if (e.Message.StartsWith("Error converting value "))
                {
                    TeslaClient.Logger.WriteLine("TeslaVehicle failed to deserialize something.  Need to add new enum value?  " + e + "\r\nBad TeslaLib JSON content: " + response.Content);
                }
                throw;
            }
            catch (JsonReaderException e)
            {
                ReportKnownErrors(response);

                e.Data["SerializedResponse"] = response.Content;

                // Hack - if we have an enum we can't deal with, print something out...  But we also can't not fail.
                if (e.Message.StartsWith("Error converting value "))
                {
                    TeslaClient.Logger.WriteLine("TeslaVehicle failed to deserialize something.  Need to add new enum value?  " + e + "\r\nBad TeslaLib JSON content: " + response.Content);
                }
                throw;
            }
            catch (Exception e)
            {
                ReportKnownErrors(response);
                if (response.Content.Contains(TeslaClient.InternalServerErrorMessage))
                    throw new TeslaServerException();

                e.Data["SerializedResponse"] = response.Content;
                throw;
            }
        }

        #endregion Commands

        public string LoadStreamingValues(string values)
        {
            return null;

            //values = "speed,odometer,soc,elevation,est_heading,est_lat,est_lng,power,shift_state,range,est_range";
            //string response = webClient.DownloadString(Path.Combine(TESLA_SERVER, string.Format("stream/{0}/?values={1}", vehicle.VehicleId, values)));
            //return response;
        }
    }
}
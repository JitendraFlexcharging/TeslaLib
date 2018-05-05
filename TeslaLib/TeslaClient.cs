using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TeslaLib.Models;
using TeslaLib.Converters;
using System.Security;

namespace TeslaLib
{

    public class TeslaClient
    {
        public string Email { get; }
        public string TeslaClientID { get; }
        public string TeslaClientSecret { get; }
        public string AccessToken { get; private set; }

        public RestClient Client { get; set; }

        public static readonly string BASE_URL = "https://owner-api.teslamotors.com/api/1";
        public static readonly string VERSION = "1.1.0";

        internal const String InternalServerErrorMessage = "<title>We're sorry, but something went wrong (500)</title>";

        public TeslaClient(string email, string teslaClientId, string teslaClientSecret)
        {
            Email = email;
            TeslaClientID = teslaClientId;
            TeslaClientSecret = teslaClientSecret;

            Client = new RestClient(BASE_URL);
            Client.Authenticator = new TeslaAuthenticator();
        }

        public void LoginUsingCache(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            var token = LoginTokenCache.GetToken(Email);
            if (token != null)
            {
                SetToken(token);
            }
            else
            {
                token = GetLoginToken(password);
                SetToken(token);
                LoginTokenCache.AddToken(Email, token);
            }
        }

        public void Login(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            LoginToken token = GetLoginToken(password);
            SetToken(token);
        }

        private LoginToken GetLoginToken(string password)
        {
            var loginClient = new RestClient("https://owner-api.teslamotors.com/oauth");
            var request = new RestRequest("token")
            {
                RequestFormat = DataFormat.Json
            };

            request.AddBody(new
            {
                grant_type = "password",
                client_id = TeslaClientID,
                client_secret = TeslaClientSecret,
                email = Email,
                password = password
            });
            var response = loginClient.Post<LoginToken>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new SecurityException($"Logging in failed for account {Email}: {response.StatusDescription}.  Is your password correct?  Does your Tesla account allow mobile access?");
            }
            if (response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut ||
                response.ResponseStatus == ResponseStatus.Aborted)
            {
                throw response.ErrorException;
            }
            var token = response.Data;
            return token;
        }

        internal void SetToken(LoginToken token)
        {
            var auth = Client.Authenticator as TeslaAuthenticator;
            auth.Token = token.AccessToken;
            AccessToken = token.AccessToken;
        }

        public void ClearLoginTokenCache() => LoginTokenCache.ClearCache();

        public List<TeslaVehicle> LoadVehicles()
        {
            var request = new RestRequest("vehicles");
            var response = Client.Get(request);

            if (response.Content.Length == 0)
                throw new FormatException("Response was empty.");

            List<TeslaVehicle> data = null;
            try
            {
                var json = JObject.Parse(response.Content)["response"];
                data = JsonConvert.DeserializeObject<List<TeslaVehicle>>(json.ToString());
                data.ForEach(x => x.Client = Client);
            }
            catch(Exception e)
            {
                if (response.Content.Contains(InternalServerErrorMessage))
                    throw new TeslaServerException();
                Console.WriteLine("Bad content: " + response.Content);
                e.Data["SerializedResponse"] = response.Content;
                throw;
            }

            return data;
        }
    }
}

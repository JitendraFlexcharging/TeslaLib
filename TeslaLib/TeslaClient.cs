using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TeslaLib.Models;
using TeslaLib.Converters;
using System.Security;
using System.Threading.Tasks;

namespace TeslaLib
{

    public class TeslaClient
    {
        public string Email { get; }
        public string TeslaClientId { get; }
        public string TeslaClientSecret { get; }
        public string AccessToken { get; private set; }

        public RestClient Client { get; set; }

        public const string LoginUrl = "https://owner-api.teslamotors.com/oauth/";
        public const string BaseUrl = "https://owner-api.teslamotors.com/api/1/";
        public const string Version = "1.1.0";

        private static IOAuthTokenStore TokenStore = null;

        internal const String InternalServerErrorMessage = "<title>We're sorry, but something went wrong (500)</title>";

        public TeslaClient(string email, string teslaClientId, string teslaClientSecret)
        {
            Email = email;
            TeslaClientId = teslaClientId;
            TeslaClientSecret = teslaClientSecret;

            Client = new RestClient(BaseUrl);
            Client.Authenticator = new TeslaAuthenticator();
        }

        public static void SetOAuthTokenStore(IOAuthTokenStore tokenStore)
        {
            TokenStore = tokenStore;
        }

        public async Task LoginUsingCacheAsync(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            LoginToken token = null;
            if (TokenStore != null)
            {
                token = await TokenStore.GetTokenAsync(Email);
            }

            if (token != null)
            {
                SetToken(token);
            }
            else
            {
                token = await GetLoginTokenAsync(password).ConfigureAwait(false);
                SetToken(token);
                if (TokenStore != null)
                    await TokenStore.AddTokenAsync(Email, token);
            }
        }

        public async Task LoginAsync(string password) => SetToken(await GetLoginTokenAsync(password).ConfigureAwait(false));

        private async Task<LoginToken> GetLoginTokenAsync(string password)
        {
            var loginClient = new RestClient(LoginUrl);
			
            var request = new RestRequest("token")
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(new
            {
                grant_type = "password",
                client_id = TeslaClientId,
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

        public void ClearLoginTokenCache()
        {
            if (TokenStore != null)
                TokenStore.ClearCache();
        }

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

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
        // For refresh token.
        private LoginToken _token;

        public RestClient Client { get; set; }

        public const string LoginUrl = "https://owner-api.teslamotors.com/oauth/";
        public const string BaseUrl = "https://owner-api.teslamotors.com/api/1/";
        public const string Version = "1.1.0";

        private static IOAuthTokenStore TokenStore = null;
        // If we are within perhaps two weeks of our OAuth2 token expiring, renew the token.
        private static readonly TimeSpan TokenExpirationRenewalWindow = TimeSpan.FromDays(14);

        internal const String InternalServerErrorMessage = "<title>We're sorry, but something went wrong (500)</title>";

        public TeslaClient(string email, string teslaClientId, string teslaClientSecret)
        {
            Email = email;
            TeslaClientId = teslaClientId;
            TeslaClientSecret = teslaClientSecret;

            Client = new RestClient(BaseUrl);
            Client.Authenticator = new TeslaAuthenticator();
        }

        public static IOAuthTokenStore OAuthTokenStore {
            get { return TokenStore; }
            set { TokenStore = value; }
        }

        public async Task LoginUsingTokenStoreAsync(string password)
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
                // Check expiration.  If we're within a few days of expiration, refresh it.
                TimeSpan expirationTimeFromNow = token.ExpiresUtc - DateTime.UtcNow;
                if (expirationTimeFromNow.TotalSeconds < 0)
                {
                    // If it expired, we need a new token.  Not clear that the refresh token will work.
                    token = null;
                }
                else if (expirationTimeFromNow < TokenExpirationRenewalWindow)
                {
                    // We have a valid refresh token, but it's close to expiry.  Try getting a new one, but don't block if that fails.
                    var newToken = await RefreshLoginTokenAsync(token);
                    if (TokenStore != null)
                    {
                        if (newToken == null)
                        {
                            // Try again next time, and hope we can refresh the token at that point.
                            // For transient errors maybe this operation will work the next day.  Could be network problems,
                            // or servers being down, or someone running the app while on an airplane.
                            // But don't delete the refresh token.
                            // Also, if someone changes their password, our refresh token may be invalid.
                        }
                        else
                        {
                            await TokenStore.UpdateTokenAsync(Email, newToken);
                            token = newToken;
                        }
                    }
                    SetToken(token);
                }
                else
                {
                    SetToken(token);
                }
            }

            if (token == null)
            {
                token = await GetLoginTokenAsync(password).ConfigureAwait(false);
                SetToken(token);
                if (TokenStore != null)
                    await TokenStore.AddTokenAsync(Email, token);
            }
        }

        // This method relies solely on the IOAuthTokenStore to recover a valid OAuth2 token, using that and if needed refreshing it.
        // It will throw a SecurityException if the token is expired and we have no alternatives.
        public async Task LoginUsingTokenStoreWithoutPasswordAsync()
        {
            LoginToken token = null;
            if (TokenStore != null)
            {
                token = await TokenStore.GetTokenAsync(Email);
            }

            if (token != null)
            {
                // Check expiration.  If we're within a few days of expiration, refresh it.
                TimeSpan expirationTimeFromNow = token.ExpiresUtc - DateTime.UtcNow;
                if (expirationTimeFromNow.TotalSeconds < 0)
                {
                    // If it expired, we need a new token.  Not clear whether the refresh token will work.
                    // Try using the refresh token anyways?  This should fail, but maybe it will work?
                    // Note:  This error code path has not been tested.
                    var newToken = await RefreshLoginTokenAsync(token);
                    if (newToken == null)
                    {
                        await TokenStore.UpdateTokenAsync(Email, newToken);
                        token = newToken;
                    }
                }
                else if (expirationTimeFromNow < TokenExpirationRenewalWindow)
                {
                    // We have a valid refresh token, but it's close to expiry.  Try getting a new one, but don't block if that fails.
                    var newToken = await RefreshLoginTokenAsync(token);
                    if (TokenStore != null)
                    {
                        if (newToken == null)
                        {
                            // Try again next time, and hope we can refresh the token at that point.
                            // For transient errors maybe this operation will work the next day.  Could be network problems,
                            // or servers being down, or someone running the app while on an airplane.
                            // But don't delete the refresh token.
                            // Also, if someone changes their password, our refresh token may be invalid.
                        }
                        else
                        {
                            await TokenStore.UpdateTokenAsync(Email, newToken);
                            token = newToken;
                        }
                    }
                    SetToken(token);
                }
                else
                {
                    SetToken(token);
                }
            }

            if (token == null)
                throw new SecurityException($"Cannot get a LoginToken for {Email}");
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
            _token = token;
        }

        public LoginToken GetTeslaLoginToken()
        {
            return _token;
        }

        public void ClearLoginTokenStore()
        {
            if (TokenStore != null)
                TokenStore.ClearCache();
        }

        // For testing purposes.
        public async Task RefreshLoginTokenAndUpdateTokenStoreAsync()
        {
            LoginToken newToken = await RefreshLoginTokenAsync(_token);
            if (newToken == null)
            {
                Console.WriteLine("Refreshing the login token failed for {0}", Email);
                return;
            }

            Console.WriteLine($"RefreshLoginTokenAsync: Old access token: {_token.AccessToken}\r\nOld refresh token: {_token.RefreshToken}");
            Console.WriteLine($"Old expiry time: {_token.ExpiresUtc}");
            Console.WriteLine($"New access token: {newToken.AccessToken}\r\nNew refresh token: {newToken.RefreshToken}");
            Console.WriteLine($"New expiry time: {newToken.ExpiresUtc}");
            SetToken(newToken);

            if (TokenStore != null)
            {
                await TokenStore.UpdateTokenAsync(Email, newToken);
            }
        }

        // For a LoginToken that is close to expiry, this method will refresh the OAuth2 access token.  Returns a new LoginToken.
        internal async Task<LoginToken> RefreshLoginTokenAsync(LoginToken loginToken)
        {
            var loginClient = new RestClient(LoginUrl);

            var request = new RestRequest("token")
            {
                RequestFormat = DataFormat.Json
            };

            request.AddJsonBody(new
            {
                grant_type = "refresh_token",
                refresh_token = loginToken.RefreshToken
            });

            var newToken = await loginClient.PostAsync<LoginToken>(request);

            return newToken;
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

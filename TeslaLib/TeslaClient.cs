using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using TeslaLib.Models;
using TeslaLib.Converters;
using System.Security;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net;
using System.Linq;

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

        public static TextWriter Logger = TextWriter.Null;

        private static IOAuthTokenStore TokenStore = null;
        // If we are within perhaps two weeks of our OAuth2 token expiring, renew the token.
        private static readonly TimeSpan TokenExpirationRenewalWindow = TimeSpan.FromDays(14);

        internal const String InternalServerErrorMessage = "<title>We're sorry, but something went wrong (500)</title>";
        internal const String ThrottlingMessage = "You have been temporarily blocked for making too many requests!";

        public TeslaClient(string email, string teslaClientId, string teslaClientSecret)
        {
            Email = email;
            TeslaClientId = teslaClientId;
            TeslaClientSecret = teslaClientSecret;

            Client = new RestClient(BaseUrl);
            Client.Authenticator = new TeslaAuthenticator();
        }

        public static IOAuthTokenStore OAuthTokenStore
        {
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

            if (token != null)
            {
                // Now see if the login worked.  If so, great.  If not, try logging in with the password provided.
                try
                {
                    var vehicles = await LoadVehiclesAsync(CancellationToken.None);
                }
                catch (Exception e)
                {
                    token = null;
                }
            }

            if (token == null)
            {
                try
                {
                    token = await GetLoginTokenAsync(password).ConfigureAwait(false);
                }
                catch(SecurityException)
                {
                    if (TokenStore != null)
                        await TokenStore.DeleteTokenAsync(Email);
                    throw;
                }

                // Successfully obtained a new token.
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
                throw new SecurityException($"Logging in failed for Tesla account {Email}: {response.StatusDescription}.  Is your password correct?  Does your Tesla account allow mobile access?");
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

        internal bool IsLoggedIn()
        {
            if (Client == null)
                return false;
            var auth = Client.Authenticator as TeslaAuthenticator;
            if (auth == null)
                return false;
            return _token != null && auth.Token != null && AccessToken != null;
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
        public async Task<bool> RefreshLoginTokenAndUpdateTokenStoreAsync()
        {
            LoginToken newToken = await RefreshLoginTokenAsync(_token);
            if (newToken == null || newToken.AccessToken == null)
            {
                Console.WriteLine("Refreshing the login token failed for {0}", Email);
                return false;
            }

            Console.WriteLine($"TeslaClient.RefreshLoginTokenAsync: Old access token: {_token.AccessToken}\r\nOld refresh token: {_token.RefreshToken}");
            Console.WriteLine($"Old expiry time: {_token.ExpiresUtc}");
            Console.WriteLine($"New access token: {newToken.AccessToken}\r\nNew refresh token: {newToken.RefreshToken}");
            Console.WriteLine($"New expiry time: {newToken.ExpiresUtc}");
            SetToken(newToken);

            if (TokenStore != null)
            {
                await TokenStore.UpdateTokenAsync(Email, newToken);
            }
            return true;
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
            if (!IsLoggedIn())
                throw new InvalidOperationException("Log in to your Tesla account first.");

            var request = new RestRequest("vehicles");
            var response = Client.Get(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                UnauthorizedHandler(response);

            if (response.Content.Length == 0)
                throw new FormatException("Tesla's response was empty.");

            List<TeslaVehicle> data = null;
            try
            {
                var json = JObject.Parse(response.Content)["response"];
                data = JsonConvert.DeserializeObject<List<TeslaVehicle>>(json.ToString());
                data.ForEach(x => x.Client = Client);
            }
            catch (Exception e)
            {
                if (response.Content == ThrottlingMessage)
                {
                    var throttled = new TeslaThrottlingException();
                    throttled.Data["StatusCode"] = response.StatusCode;
                    TeslaClient.Logger.WriteLine("Tesla account {0} was throttled by Tesla.  StatusCode: {1}", this.Email, response.StatusCode);
                    throw throttled;
                }

                TeslaClient.Logger.WriteLine("TeslaClient.LoadVehicles failed to parse and deserialize contents: \"" + response.Content + "\"");
                if (response.Content.Contains(InternalServerErrorMessage))
                {
                    var tse = new TeslaServerException();
                    tse.Data["SerializedResponse"] = response.Content;
                    throw tse;
                }
                e.Data["SerializedResponse"] = response.Content;
                throw;
            }

            return data;
        }

        public async Task<List<TeslaVehicle>> LoadVehiclesAsync(CancellationToken cancellationToken)
        {
            if (!IsLoggedIn())
                throw new InvalidOperationException("Log in to your Tesla account first.");

            var request = new RestRequest("vehicles");
            var response = await Client.ExecuteGetTaskAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                UnauthorizedHandler(response);

            if (response.Content.Length == 0)
                throw new FormatException("Tesla's response was empty.");

            List<TeslaVehicle> data = null;
            try
            {
                var json = JObject.Parse(response.Content)["response"];
                data = JsonConvert.DeserializeObject<List<TeslaVehicle>>(json.ToString());
                data.ForEach(x => x.Client = Client);
            }
            catch (Exception e)
            {
                if (response.Content == ThrottlingMessage)
                {
                    var throttled = new TeslaThrottlingException();
                    throttled.Data["StatusCode"] = response.StatusCode;
                    TeslaClient.Logger.WriteLine("Tesla account {0} was throttled by Tesla.  StatusCode: {1}", this.Email, response.StatusCode);
                    throw throttled;
                }

                TeslaClient.Logger.WriteLine("TeslaClient.LoadVehiclesAsync failed to parse and deserialize contents: \"" + response.Content + "\"");
                if (response.Content.Contains(InternalServerErrorMessage))
                {
                    var tse = new TeslaServerException();
                    tse.Data["SerializedResponse"] = response.Content;
                    throw tse;
                }
                e.Data["SerializedResponse"] = response.Content;
                throw;
            }

            return data;
        }

        /*
        public async Task<List<String>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            if (!IsLoggedIn())
                throw new InvalidOperationException("Log in to your Tesla account first.");

            var request = new RestRequest("products");
            var response = await Client.ExecuteGetTaskAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                UnauthorizedHandler(response);

            if (response.Content.Length == 0)
                throw new FormatException("Tesla's response was empty.");

            List<String> data = null;
            try
            {
                var json = JObject.Parse(response.Content)["response"];
                String str = json.ToString();
                data = JsonConvert.DeserializeObject<List<String>>(str);
                //data.ForEach(x => x.Client = Client);
            }
            catch (Exception e)
            {
                if (response.Content == ThrottlingMessage)
                {
                    var throttled = new TeslaThrottlingException();
                    throttled.Data["StatusCode"] = response.StatusCode;
                    TeslaClient.Logger.WriteLine("Tesla account {0} was throttled by Tesla.  StatusCode: {1}", this.Email, response.StatusCode);
                    throw throttled;
                }

                TeslaClient.Logger.WriteLine("TeslaClient.GetAllProductsAsync failed to parse and deserialize contents: \"" + response.Content + "\"");
                if (response.Content.Contains(InternalServerErrorMessage))
                {
                    var tse = new TeslaServerException();
                    tse.Data["SerializedResponse"] = response.Content;
                    throw tse;
                }
                e.Data["SerializedResponse"] = response.Content;
                throw;
            }

            return data;
        }
        */

        public async Task<List<EnergySite>> GetEnergySitesAsync(CancellationToken cancellationToken)
        {
            if (!IsLoggedIn())
                throw new InvalidOperationException("Log in to your Tesla account first.");

            var request = new RestRequest("products");
            var response = await Client.ExecuteGetTaskAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                UnauthorizedHandler(response);

            // I think almost all of these were actually returning unauthorized.  RestSharp may have been hiding these due to an internal NullReferenceException.
            if (response.Content.Length == 0)
                throw new FormatException("Tesla's response was empty.");

            List<EnergySite> data = null;
            try
            {
                var json = JObject.Parse(response.Content)["response"];
                String str = json.ToString();
                // We get an array of cars, energy sites, and maybe powerwalls in JSON.  Gotta parse these separately though,
                // because I don't think we can deserialize a List<A | B>.  
                data = new List<EnergySite>();
                String[] sections = JsonSplit(str);
                foreach(var section in sections)
                {
                    if (section.Contains("energy_site_id"))
                    {
                        EnergySite energySite = JsonConvert.DeserializeObject<EnergySite>(section);
                        data.Add(energySite);
                    }
                }
                data.ForEach(x => x.Client = Client);
            }
            catch (Exception e)
            {
                if (response.Content == ThrottlingMessage)
                {
                    var throttled = new TeslaThrottlingException();
                    throttled.Data["StatusCode"] = response.StatusCode;
                    TeslaClient.Logger.WriteLine("Tesla account {0} was throttled by Tesla.  StatusCode: {1}", this.Email, response.StatusCode);
                    throw throttled;
                }

                TeslaClient.Logger.WriteLine("TeslaClient.GetEnergySitesAsync failed to parse and deserialize contents: \"" + response.Content + "\"");
                if (response.Content.Contains(InternalServerErrorMessage))
                {
                    var tse = new TeslaServerException();
                    tse.Data["SerializedResponse"] = response.Content;
                    throw tse;
                }
                e.Data["SerializedResponse"] = response.Content;
                throw;
            }

            return data;
        }

        private static String[] JsonSplit(String str)
        {
            List<String> sections = new List<String>();
            int i = 0;
            while (i<str.Length)
            {
                while (i < str.Length && str[i] != '{') i++;
                if (i == str.Length)
                    break;
                int startIndex = i;
                i++;
                int depth = 0;
                while (i < str.Length)
                {
                    if (str[i] == '{')
                        depth++;
                    if (str[i] == '}')
                    {
                        depth--;
                        if (depth < 0)
                        {
                            int lastIndex = i;
                            String section = str.Substring(startIndex, lastIndex - startIndex + 1);
                            sections.Add(section);
                            break;
                        }
                    }
                    i++;
                }
            }
            return sections.ToArray();
        }

        internal void UnauthorizedHandler(IRestResponse response)
        {
            bool successfullyRefreshedToken = false;
            try
            {
                successfullyRefreshedToken = RefreshLoginTokenAndUpdateTokenStoreAsync().Result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Refreshing Tesla login token and updating token store threw: " + e);
            }

            Logger.WriteLine("Tesla login failed for account {0}. Did we successfully update the login token? {1}", Email, successfullyRefreshedToken);

            ReportUnauthorizedAccess(response, successfullyRefreshedToken);
        }

        internal static void ReportUnauthorizedAccess(IRestResponse response, bool successfullyRefreshedToken)
        {
            var errorParameter = response.Headers.Where(p => p.Value != null && p.Value.ToString().Contains("error_description")).FirstOrDefault();
            String errorDescription = errorParameter.Value.ToString();
            String errorMsg = null;
            if (errorParameter != null)
            {
                int errDescIndex = errorDescription.IndexOf("error_description");
                if (errDescIndex > 0)
                {
                    int firstQuote = errorDescription.IndexOf('\"', errDescIndex + 10);
                    int secondQuote = errorDescription.IndexOf('\"', firstQuote + 1);
                    errorMsg = errorDescription.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
                }
            }

            String msg = "Tesla authorization error.  ";
            if (successfullyRefreshedToken)
                msg += "Try again.";
            else {
                msg += "Did your password change?";
                if (errorMsg != null)
                    msg += "  " + errorMsg;
            }

            throw new SecurityException(msg);
        }
    }
}

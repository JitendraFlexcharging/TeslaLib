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
using System.Text;
using System.Security.Cryptography;
using TeslaAuth;

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
        private TeslaAuthHelper _teslaAuthHelper;

        public const string LoginUrl = "https://owner-api.teslamotors.com/oauth/";
        public const string BaseUrl = "https://owner-api.teslamotors.com/api/1/";
        public const string StreamingUrl = "wss://streaming.vn.teslamotors.com/streaming/";

        // Multi-factor Authentication enabled URLs
        // Read documentation here:  https://tesla-api.timdorr.com/api-basics/authentication
        public const string OAuthAuthorizeUrlMFA = "https://auth.tesla.com/oauth2/v3/authorize";
        public const string OAuthBaseUrl = "https://auth.tesla.com/oauth2/v3";   // add "token" to the end
        public const string TeslaRedirectUrl = "https://auth.tesla.com/void/callback";  // Not what we want, but...

        public const string Version = "1.1.0";

        public static TextWriter Logger = TextWriter.Null;

        private static IOAuthTokenStore TokenStore = null;
        // If we are within perhaps two weeks of our OAuth2 token expiring, renew the token.
        // Tokens may last for 45 days.
        private static readonly TimeSpan TokenExpirationRenewalWindow = TimeSpan.FromDays(14);

        internal const String InternalServerErrorMessage = "<title>We're sorry, but something went wrong (500)</title>";
        internal const String ThrottlingMessage = "You have been temporarily blocked for making too many requests!";
        internal const String RetryLaterMessage = "Retry later";
        internal const String BlockedMessage = "Blocked";


        private static void MFACodeInvalidHandler(object sender, MFACodeInvalidEventArgs args)
        {
            throw new MultiFactorAuthenticationException(String.Format("MFA problem: {0}", args.Message), args.Username);
        }

        private static void MFACodeRequiredHandler(object sender, MFACodeRequiredEventArgs args)
        {
            throw new MultiFactorAuthenticationException(String.Format("Multi-factor authentication is required for user {0}", args.Username), args.Username);
        }

        public TeslaClient(string email, string teslaClientId, string teslaClientSecret)
        {
            Email = email;
            TeslaClientId = teslaClientId;
            TeslaClientSecret = teslaClientSecret;

            Client = new RestClient(BaseUrl);
            Client.Authenticator = new TeslaAuthenticator();

            String userAgent = "FlexCharging.com";  // This works with a '.' in the name, but requests hang without the '.'!
            _teslaAuthHelper = new TeslaAuthHelper(userAgent);
            _teslaAuthHelper.MFACodeRequired += MFACodeRequiredHandler;
            _teslaAuthHelper.MFACodeInvalid += MFACodeInvalidHandler;
        }

        public static IOAuthTokenStore OAuthTokenStore
        {
            get { return TokenStore; }
            set { TokenStore = value; }
        }

        public async Task LoginUsingTokenStoreAsync(string password, string mfaCode = null, TeslaAccountRegion region = TeslaAccountRegion.Unknown, 
            bool forceRefreshOlderThanToday = false)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            LoginToken token = null;
            if (TokenStore != null)
            {
                token = await TokenStore.GetTokenAsync(Email);
            }

            bool refreshingTokenFailed = false;
            if (token != null)
            {
                // Check expiration.  If we're within a few days of expiration, refresh it.
                // If the access token expired, we might still be able to use the refresh token, maybe.
                TimeSpan expirationTimeFromNow = token.ExpiresUtc - DateTime.UtcNow;

                /*  // This code was throwing away the token without trying the refresh token.  Seems silly.
                if (expirationTimeFromNow.TotalSeconds < 0)
                {
                    // If it expired, we need a new token.  Not clear that the refresh token will work.
                    Logger.WriteLine("TeslaLib login token for {0} expired.  UTC expiry time: {1}", Email, token.ExpiresUtc);
                    token = null;
                }
                */

                if (expirationTimeFromNow < TokenExpirationRenewalWindow || (forceRefreshOlderThanToday && token.CreatedUtc < DateTime.Now.Date))
                {
                    // We have a valid refresh token, but it's close to expiry.  Try getting a new one, but don't block if that fails.
                    LoginToken newToken = null;
                    try
                    {
                        newToken = await RefreshLoginTokenAsync(token, region);
                        expirationTimeFromNow = token.ExpiresUtc - DateTime.UtcNow;
                    }
                    catch(Exception e)
                    {
                        refreshingTokenFailed = true;
                        Object serializedResponse = e.Data["SerializedResponse"];
                        String errMsg = String.Format("TeslaLib couldn't refresh a login token while logging in for account {5}.  Will try to log in again.  Token created at: {0}  Expires: {1}  {2}: {3}{4}",
                            token.CreatedUtc, token.ExpiresUtc, e.GetType().Name, e.Message, serializedResponse == null ? String.Empty : "  Serialized response: " + serializedResponse, Email);
                        Console.WriteLine(errMsg);
                        Logger.WriteLine(errMsg);

                        if (forceRefreshOlderThanToday)
                            token = null;
                    }

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

                    if (token != null)
                        SetToken(token);
                }
                else
                {
                    SetToken(token);
                }

                if (expirationTimeFromNow.TotalSeconds < 0)
                {
                    // If it expired, we need a new token.  Not clear that the refresh token will work.
                    Logger.WriteLine("TeslaLib login token for {0} expired.  UTC expiry time: {1}  Created at: {2}", Email, token.ExpiresUtc, token.CreatedUtc);
                    token = null;
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
                    Logger.WriteLine("TeslaLib LoginUsingTokenStoreAsync hit an exception when trying to load vehicles, in an effort to verify our LoginToken was good.  " + e);
                    token = null;
                }
            }

            if (token == null)
            {
                try
                {
                    token = await GetLoginTokenAsync(password, mfaCode, region).ConfigureAwait(false);
                }
                catch(SecurityException)
                {
                    String getLoginTokenFailed = $"TeslaLib GetLoginTokenAsync failed for account {Email}";
                    if (refreshingTokenFailed)
                        getLoginTokenFailed += "  Refreshing the login token had failed previously.";
                    Console.WriteLine(getLoginTokenFailed);
                    Logger.WriteLine(getLoginTokenFailed);
                    if (TokenStore != null)
                        await TokenStore.DeleteTokenAsync(Email);
                    throw;
                }

                if (token == null)
                    throw new SecurityException(String.Format("TeslaLib couldn't log in for user {0}", Email));

                // Successfully obtained a new token.
                SetToken(token);
                if (TokenStore != null)
                    await TokenStore.AddTokenAsync(Email, token);
            }

            if (!IsLoggedIn())
            {
                // We don't expect to hit this, unless we messed up error handling somewhere along the way, or Tesla returns a new error code we don't recognize.
                TeslaAuthenticator auth = (Client != null) ? Client.Authenticator as TeslaAuthenticator : null;
                String loginProblem = String.Format("TeslaLib LoginUsingTokenStoreAsync completed, did not throw, but IsLoggedIn returns false.  Email: {0}  Is client null? {1}  Is auth null? {2}  Access token: {3}  Was LoginToken null? {4}",
                    Email, Client == null, auth == null, AccessToken, token == null);
                Logger.WriteLine(loginProblem);
                Console.WriteLine(loginProblem);
            }
        }

        // This method relies solely on the IOAuthTokenStore to recover a valid OAuth2 token, using that and if needed refreshing it.
        // It will throw a SecurityException if the token is expired and we have no alternatives.
        public async Task LoginUsingTokenStoreWithoutPasswordAsync(TeslaAccountRegion region = TeslaAccountRegion.Unknown)
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
                    var newToken = await RefreshLoginTokenAsync(token, region);
                    if (newToken == null)
                    {
                        Logger.WriteLine("TeslaLib had an expired login token, tried refreshing it, and failed for account {0}", Email);
                        await TokenStore.DeleteTokenAsync(Email);
                        token = null;
                    }
                }
                else if (expirationTimeFromNow < TokenExpirationRenewalWindow)
                {
                    // We have a valid refresh token, but it's close to expiry.  Try getting a new one, but don't block if that fails.
                    var newToken = await RefreshLoginTokenAsync(token, region);
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

            if (!IsLoggedIn())
            {
                // We don't expect to hit this, unless we messed up error handling somewhere along the way, or Tesla returns a new error code we don't recognize.
                TeslaAuthenticator auth = (Client != null) ? Client.Authenticator as TeslaAuthenticator : null;
                String loginProblem = String.Format("TeslaLib LoginUsingTokenStoreWithoutPassword says IsLoggedIn returns false.  Email: {0}  Is client null? {1}  Is auth null? {2}  Access token: {3}  Was LoginToken null? {4}",
                    Email, Client == null, auth == null, AccessToken, token == null);
                Logger.WriteLine(loginProblem);
                Console.WriteLine(loginProblem);
            }

            if (token == null)
            {
                throw new SecurityException($"Cannot get a LoginToken for {Email}");
            }
        }

        public async Task LoginAsync(string password, String mfaCode = null, TeslaAccountRegion region = TeslaAccountRegion.Unknown) =>
            SetToken(await GetLoginTokenAsync(password, mfaCode, region).ConfigureAwait(false));

        private async Task<LoginToken> GetLoginTokenAsync(string password, string mfaCode, TeslaAccountRegion region)
        {
            LoginToken loginToken = null;
            try
            {
                Tokens tokens = await _teslaAuthHelper.AuthenticateAsync(Email, password, mfaCode, region);

                loginToken = ConvertTeslaAuthTokensToLoginToken(tokens);
            }
            catch(Exception e)
            {
                Object serializedResponse = e.Data["SerializedResponse"];
                String responseStr = serializedResponse == null ? String.Empty : "\r\nSerialized response: " + serializedResponse;
                Logger.WriteLine("TeslaClient GetLoginToken failed for user {0}.  {1}{2}", Email, e, responseStr);
                Console.WriteLine("TeslaClient GetLoginToken failed for user {0}.  {1}{2}", Email, e, responseStr);
                throw e;
            }

            // @TODO: Verify that the TeslaAuthHelper code verifies the code challenge.

            return loginToken;
        }

        private static LoginToken ConvertTeslaAuthTokensToLoginToken(Tokens tokens)
        {
            LoginToken loginToken = new LoginToken();
            loginToken.AccessToken = tokens.AccessToken;
            loginToken.RefreshToken = tokens.RefreshToken;
            loginToken.CreatedUtc = tokens.CreatedAt.UtcDateTime;
            loginToken.ExpiresInTimespan = tokens.ExpiresIn;
            return loginToken;
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
        public async Task<bool> RefreshLoginTokenAndUpdateTokenStoreAsync(TeslaAccountRegion region = TeslaAccountRegion.Unknown)
        {
            LoginToken newToken = await RefreshLoginTokenAsync(_token, region);
            if (newToken == null || newToken.AccessToken == null)
            {
                Logger.WriteLine("TeslaLib: Refreshing the login token failed for {0}", Email);
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
        internal async Task<LoginToken> RefreshLoginTokenAsync(LoginToken loginToken, TeslaAccountRegion region)
        {
            var tokens = await _teslaAuthHelper.RefreshTokenAsync(loginToken.RefreshToken, region);

            LoginToken newTokens = ConvertTeslaAuthTokensToLoginToken(tokens);
            return newTokens;
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
                HandleKnownFailures(response);

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
            var response = await Client.ExecuteGetAsync(request, cancellationToken);

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
                HandleKnownFailures(response);

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
                HandleKnownFailures(response);

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
            var response = await Client.ExecuteGetAsync(request, cancellationToken);

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
                    else if (section.Contains("\"vin\":"))
                    {
                        // This is a car.  Skip
                    }
                    else
                    {
                        Logger.WriteLine("Got a section that wasn't a vehicle nor an energy site.  Section: {0}", section);
                        Console.WriteLine("Got a section that wasn't a vehicle nor an energy site.  Section: {0}", section);
                    }
                }
                data.ForEach(x => x.Client = Client);
            }
            catch (Exception e)
            {
                HandleKnownFailures(response);

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

        private void HandleKnownFailures(IRestResponse response)
        {
            if (response.Content == ThrottlingMessage || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var throttled = new TeslaThrottlingException();
                throttled.Data["StatusCode"] = response.StatusCode;
                TeslaClient.Logger.WriteLine("Tesla account {0} was throttled by Tesla.  StatusCode: {1}", this.Email, response.StatusCode);
                throw throttled;
            }

            if (response.StatusCode == (HttpStatusCode)444 && response.Content == TeslaClient.BlockedMessage)
            {
                var blocked = new TeslaBlockedException();
                blocked.Data["StatusCode"] = response.StatusCode;
                TeslaClient.Logger.WriteLine("Tesla server blocked access from your machine when accessing Tesla account {0}.  Retry later, maybe?  StatusCode: {1}", this.Email, response.StatusCode);
                throw blocked;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var badRequest = new InvalidOperationException("Tesla said this was a bad request");
                badRequest.Data["StatusCode"] = response.StatusCode;
                badRequest.Data["Response"] = response.Content;
                TeslaClient.Logger.WriteLine("Tesla server said we made a bad request.  Response: {0}", response.Content);
                throw badRequest;
            }
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

            ReportUnauthorizedAccess(response, successfullyRefreshedToken, Email);
        }

        internal static void ReportUnauthorizedAccess(IRestResponse response, bool successfullyRefreshedToken, String accountName)
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

            String msg = (accountName == null) ? "Tesla authorization error.  " : "Tesla authorization error for account " + accountName + ".  ";
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

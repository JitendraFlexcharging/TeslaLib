using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeslaAuth;
using TeslaLib.Models;

namespace TeslaLib
{
    public interface ITeslaClient
    {
        string AccessToken { get; }
        RestClient Client { get; set; }
        string Email { get; }  
        ITeslaAuthHelper TeslaAuthHelper { get; }

        [Obsolete("Please use IOAuthTokenDataBase OAuthTokenStoreDataBase instead.")]
        IOAuthTokenStore OAuthTokenStore { get; set; }
        string TeslaClientId { get; }
        string TeslaClientSecret { get; }
        void ClearLoginTokenStore();
        Task<List<EnergySite>> GetEnergySitesAsync(CancellationToken cancellationToken);
        LoginToken GetTeslaLoginToken();
        List<TeslaVehicle> LoadVehicles();
        Task<List<TeslaVehicle>> LoadVehiclesAsync(CancellationToken cancellationToken);
        Task LoginAsync(string password, string mfaCode = null);
        Task LoginUsingTokenStoreAsync(string password, string mfaCode = null, bool forceRefresh = false);
        Task LoginUsingTokenStoreWithoutPasswordAsync();
        Task LoginWithExistingToken(LoginToken loginToken);
        Task<bool> RefreshLoginTokenAndUpdateTokenStoreAsync();
        Task<LoginToken> RefreshLoginTokenAsync(LoginToken loginToken); 
        IOAuthTokenDataBase OAuthTokenStoreDataBase { get; set; }
    }
}
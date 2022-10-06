using RestSharp;
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
        string TeslaClientId { get; }
        string TeslaClientSecret { get; }
        void ClearLoginTokenStore();
        Task<List<EnergySite>> GetEnergySitesAsync(CancellationToken cancellationToken);
        LoginToken GetTeslaLoginToken();
        List<TeslaVehicle> LoadVehicles();
        Task<List<TeslaVehicle>> LoadVehiclesAsync(CancellationToken cancellationToken);
        Task LoginAsync(string email, string password, string mfaCode = null);
        Task LoginUsingTokenStoreAsync(string email, string password, string mfaCode = null, bool forceRefreshOlderThanToday = false);
        Task LoginUsingTokenStoreWithoutPasswordAsync();
        Task LoginWithExistingToken(LoginToken loginToken);
        Task<bool> RefreshLoginTokenAndUpdateTokenStoreAsync();
        Task<LoginToken> RefreshLoginTokenAsync(LoginToken loginToken);
    }
}
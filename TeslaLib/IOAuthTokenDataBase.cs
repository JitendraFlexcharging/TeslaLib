using System.Threading.Tasks;
using TeslaLib.Models;

namespace TeslaLib
{
    public interface IOAuthTokenDataBase : IOAuthTokenStore
    {
        /// <summary>
        /// Add Tesla OAuth token into DB
        /// </summary>
        /// <param name="flexEmailAddress">Flexcharging email address</param>
        /// <param name="teslaEmailAddress">Tesla account email address</param>
        /// <param name="token">Tesla OAuth token</param> 
        Task AddOAuthTokenAsync(LoginToken token, string teslaEmailAddress , string flexEmailAddress);

        /// <summary>
        /// Update Tesla OAuth token into DB
        /// </summary>
        /// <param name="flexEmailAddress">Flexcharging email address</param>
        /// <param name="teslaEmailAddress">Tesla account email address</param>
        /// <param name="token">Tesla OAuth token</param> 
        Task UpdateOAuthTokenAsync(LoginToken token, string teslaEmailAddress, string flexEmailAddress);
    }
}

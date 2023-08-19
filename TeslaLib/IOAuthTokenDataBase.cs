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
        /// <param name="loginToken">Tesla OAuth token</param> 
        Task AddOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress , string flexEmailAddress);

        /// <summary>
        /// Update Tesla OAuth token into DB
        /// </summary>
        /// <param name="flexEmailAddress">Flexcharging email address</param>
        /// <param name="teslaEmailAddress">Tesla account email address</param>
        /// <param name="loginToken">Tesla OAuth token</param> 
        Task UpdateOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress, string flexEmailAddress);
    }
}

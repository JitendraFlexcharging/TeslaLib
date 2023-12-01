using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;
using TeslaLib.Models;

namespace TeslaLib
{
    public interface IOAuthTokenDataBase
    {
        /// <summary>
        /// Clear tokens caches.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Get all the tokens from caches.
        /// </summary>
        /// <returns>Return tokens list.</returns>
        Task<ReadOnlyDictionary<String, LoginToken>> GetAllTokensAsync();

        /// <summary>
        /// Get Tesla token.
        /// </summary>
        /// <param name="flexEmailAddress">Flexcharging email address</param>
        /// <param name="teslaEmailAddress">Tesla account email address</param>
        /// <returns>Return Tesla token.</returns>
        Task<LoginToken> GetTokenAsync(string teslaEmailAddress, string flexEmailAddress);

        /// <summary>
        /// Add Tesla OAuth token.
        /// </summary>
        /// <param name="flexEmailAddress">Flexcharging email address</param>
        /// <param name="teslaEmailAddress">Tesla account email address</param>
        /// <param name="loginToken">Tesla OAuth token</param> 
        Task AddOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress, string flexEmailAddress);

        /// <summary>
        /// Update Tesla OAuth token.
        /// </summary>
        /// <param name="flexEmailAddress">Flexcharging email address</param>
        /// <param name="teslaEmailAddress">Tesla account email address</param>
        /// <param name="loginToken">Tesla OAuth token</param> 
        Task UpdateOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress, string flexEmailAddress);

        /// <summary>
        /// Delete specific Tesla token.
        /// </summary>
        /// <param name="teslaEmailAddress">Tesla account email address.</param>
        /// <param name="flexEmailAddress">Flexcharging email address.</param>
        /// <returns>Execute method asynchronously and return boolean result.</returns>
        Task<bool> DeleteTokenAsync(string teslaEmailAddress, string flexEmailAddress);
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using TeslaLib.Models;

namespace TeslaLib
{
    /// <summary>
    /// A token storage mechanism.  Some implementations may allow storing multiple tokens per user.  Others only one.
    /// </summary>
    public interface IOAuthTokenStore
    {
        Task<LoginToken> GetTokenAsync(string emailAddress);

        Task AddTokenAsync(string emailAddress, LoginToken token);

        Task UpdateTokenAsync(string emailAddress, LoginToken token);

        Task DeleteTokenAsync(string emailAddress);

        /// <summary>
        /// Delete a token but only if the access token matches what we find in our store.
        /// </summary>
        /// <param name="emailAddress">Which user's tokens to look at</param>
        /// <param name="token">A token to delete from the store</param>
        /// <returns>True if deleted successfully, false if otherwise</returns>
        Task<bool> DeleteSpecificTokenAsync(string emailAddress, LoginToken token);

        Task<ReadOnlyDictionary<String, LoginToken>> GetAllTokensAsync();

        Task<IReadOnlyList<LoginToken>> GetAllTokensForUserAsync(String emailAddress);

        void ClearCache();
    }
}

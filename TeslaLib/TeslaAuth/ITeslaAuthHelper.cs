// Helper library to authenticate to Tesla Owner API 
// Includes support for MFA.

using System.Threading;
using System.Threading.Tasks;

namespace TeslaAuth
{
    public interface ITeslaAuthHelper
    {
        Task<Tokens> AuthenticateAsync(string username, string password, string mfaCode = null, CancellationToken cancellationToken = default);
        string GetLoginUrlForBrowser();
        Task<Tokens> GetTokenAfterLoginAsync(string redirectUrl, CancellationToken cancellationToken = default);
        Task<Tokens> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    }
}
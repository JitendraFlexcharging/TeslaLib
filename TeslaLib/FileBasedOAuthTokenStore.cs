using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using TeslaLib.Models;

namespace TeslaLib
{
    // Note: This is not a secure storage mechanism for access tokens nor refresh tokens.
    // Ideally this file would be encrypted.  Or, we store the contents in a secure storage mechanism, not a file on disk.
    public class FileBasedOAuthTokenStore : IOAuthTokenDataBase
    {
        private const string CacheFileName = "TeslaLoginTokenCache.cache";
         
        private static readonly Dictionary<string, LoginToken> Tokens = new Dictionary<string, LoginToken>();
        private static readonly object CacheLock = new object();
        private static volatile bool _haveReadCacheFile;

        // On iOS, Environment.OSVersion.Platform returns Unix.
        public static readonly bool OSSupportsTokenCache = Environment.OSVersion.Platform != PlatformID.Unix;

        private void ReadCacheFile()
        {
            Tokens.Clear();

            if (!File.Exists(CacheFileName))
            {
                return;
            }

            try
            {
                // The file format here doesn't fit well with Newtonsoft's JSON.NET, unless we serialize a List of Tuples of Email, LoginTokens.
                // We can't change the LoginToken type - that's Tesla's specification.  And we need to correctly handle
                // files longer than 1K in data (about 6 users).  Yes, this code allocates a lot after these changes.
                var serializer = new JsonSerializer();
                using (var reader = File.OpenText(CacheFileName))
                {
                    while (!reader.EndOfStream)
                    {
                        var emailAddress = reader.ReadLine();
                        String serializedToken = reader.ReadLine();
                        var jsonReader = new JsonTextReader(new StringReader(serializedToken));
                        var token = serializer.Deserialize<LoginToken>(jsonReader);
                        Tokens.Add(emailAddress, token);
                    }
                }
            }
            catch (Exception e)
            {
                TeslaClient.Logger.WriteLine("Exception while reading Tesla LoginTokenCache file: {0}: {1}.  Deleting cache file.", e.GetType().Name, e.Message);
                File.Delete(CacheFileName);
            }
        }

        private void WriteCacheFile()
        {
            // Note: On an Android device, we either don't have a legal path or we simply can't write anything.
            // The same behavior happens with an IPhone (UnauthorizedAccessException).  Can we use isolated storage on Xamarin?
            try
            {
                using (var writer = File.CreateText(CacheFileName))
                {
                    var serializer = new JsonSerializer();

                    foreach (var pair in Tokens)
                    {
                        writer.WriteLine(pair.Key);
                        serializer.Serialize(writer, pair.Value);
                        writer.WriteLine();
                    }
                }
            }
            catch (UnauthorizedAccessException)
            { }
        }

        public Task AddTokenAsync(string emailAddress, LoginToken token)
        {
            lock (CacheLock)
            {
                Tokens[emailAddress] = token;
                if (OSSupportsTokenCache)
                    WriteCacheFile();
            }
            return Task.CompletedTask;
        }

        public Task UpdateTokenAsync(string emailAddress, LoginToken token)
        {
            lock (CacheLock)
            {
                Tokens[emailAddress] = token;
                if (OSSupportsTokenCache)
                    WriteCacheFile();
            }
            return Task.CompletedTask;
        }

        public Task DeleteTokenAsync(string emailAddress)
        {
            lock (CacheLock)
            {
                Tokens.Remove(emailAddress);
                if (OSSupportsTokenCache)
                {
                    WriteCacheFile();
                }
            }
            return Task.CompletedTask;
        }

        public Task<bool> DeleteSpecificTokenAsync(string emailAddress, LoginToken token)
        {
            bool deleted = false;
            lock (CacheLock)
            {
                if (Tokens.TryGetValue(emailAddress, out var foundToken))
                {
                    if (token.AccessToken == foundToken.AccessToken)
                    {
                        deleted = Tokens.Remove(emailAddress);
                    }

                    if (OSSupportsTokenCache)
                        WriteCacheFile();
                }
            }
            return Task.FromResult(deleted);
        }

        public void ClearCache()
        {
            lock (CacheLock)
            {
                Tokens.Clear();
                File.Delete(CacheFileName);
            }
        }

        public Task<LoginToken> GetTokenAsync(string emailAddress)
        {
            LoginToken token = null;
            lock (CacheLock)
            {
                if (!_haveReadCacheFile && OSSupportsTokenCache)
                {
                    ReadCacheFile();
                    _haveReadCacheFile = true;
                }

                if (!Tokens.TryGetValue(emailAddress, out token))
                {
                    // Return a task with null
                    return Task.FromResult<LoginToken>(null);
                }

                // The LoginToken's access token may have expired.  However, if it did we can simply retrieve the refresh token and try using that.
                // Do not remove it from our cache file!
                /*
                // Ensure the LoginToken is still valid.
                var expirationTime = token.CreatedUtc.ToLocalTime() + UnixTimeConverter.FromUnixTimeSpan(token.ExpiresIn);

                if (DateTime.Now + ExpirationTimeWindow >= expirationTime)
                {
                    Tokens.Remove(emailAddress);
                    if (OSSupportsTokenCache)
                        WriteCacheFile();
                    token = null;
                }
                */
            }
            return Task.FromResult(token);
        }

        public Task<ReadOnlyDictionary<String, LoginToken>> GetAllTokensAsync()
        {
            lock (CacheLock)
            {
                if (!_haveReadCacheFile && OSSupportsTokenCache)
                {
                    ReadCacheFile();
                    _haveReadCacheFile = true;
                }

                var readOnlyWrapper = new ReadOnlyDictionary<String, LoginToken>(Tokens);
                return Task.FromResult(readOnlyWrapper);
            }
        }

        public Task<IReadOnlyList<LoginToken>> GetAllTokensForUserAsync(string emailAddress)
        {
            lock (CacheLock)
            {
                if (!_haveReadCacheFile && OSSupportsTokenCache)
                {
                    ReadCacheFile();
                    _haveReadCacheFile = true;
                }

                if (Tokens.TryGetValue(emailAddress, out var loginToken))
                {
                    IReadOnlyList<LoginToken> readOnlyWrapper = new List<LoginToken>() { loginToken };
                    return Task.FromResult(readOnlyWrapper);
                }
                else
                    return Task.FromResult((IReadOnlyList<LoginToken>) new List<LoginToken>());
            }
        } 
        public Task AddOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress, string flexEmailAddress)
        {
            lock (CacheLock)
            {
                Tokens[teslaEmailAddress] = loginToken;
               
                if (OSSupportsTokenCache)
                {
                    WriteCacheFile();
                }
            }
            return Task.CompletedTask;
        }

        public Task UpdateOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress, string flexEmailAddress)
        {
            lock (CacheLock)
            {
                Tokens[teslaEmailAddress] = loginToken;
                if (OSSupportsTokenCache)
                {
                    WriteCacheFile();
                }
            }
            return Task.CompletedTask;
        } 
        public Task<LoginToken> GetTokenAsync(string teslaEmailAddress, string flexEmailAddress)
        {
            throw new NotImplementedException();
        } 
        public Task<bool> DeleteTokenAsync(string teslaEmailAddress, string flexEmailAddress)
        {
            throw new NotImplementedException();
        }
    }
}

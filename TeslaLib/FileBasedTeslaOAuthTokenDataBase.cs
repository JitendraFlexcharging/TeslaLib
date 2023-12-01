using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using TeslaLib.Models;
namespace TeslaLib
{
    public class FileBasedTeslaOAuthTokenDataBase : IOAuthTokenDataBase
    {
        private static volatile bool _haveReadCacheFile;
        private static readonly object CacheLock = new object();
        private const string CacheFileName = "TeslaLoginTokenCache.cache";
        public static readonly bool OSSupportsTokenCache = Environment.OSVersion.Platform != PlatformID.Unix;
        private static readonly IDictionary<string, LoginToken> Tokens = new Dictionary<string, LoginToken>();

        /// <inheritdoc/>
        public void ClearCache()
        {
            lock (CacheLock)
            {
                Tokens.Clear();

                File.Delete(CacheFileName);
            }
        }

        /// <inheritdoc/>
        public Task<ReadOnlyDictionary<string, LoginToken>> GetAllTokensAsync()
        {
            lock (CacheLock)
            {
                if (!_haveReadCacheFile && OSSupportsTokenCache)
                {
                    ReadCacheFile();
                    _haveReadCacheFile = true;
                }
                return Task.FromResult(new ReadOnlyDictionary<String, LoginToken>(Tokens));
            }
        }

        /// <inheritdoc/>
        public Task<LoginToken> GetTokenAsync(string teslaEmailAddress, string flexEmailAddress)
        {
            LoginToken token = null;

            lock (CacheLock)
            {
                if (string.IsNullOrWhiteSpace(teslaEmailAddress))
                {
                    throw new ArgumentException(nameof(teslaEmailAddress));
                }
                if (string.IsNullOrWhiteSpace(flexEmailAddress))
                {
                    throw new ArgumentException(nameof(flexEmailAddress));
                }

                if (!_haveReadCacheFile && OSSupportsTokenCache)
                {
                    ReadCacheFile();

                    _haveReadCacheFile = true;
                }
                var emailAddress = $"Tesla:{teslaEmailAddress}, FlexCharging:{flexEmailAddress}";
                if (!Tokens.TryGetValue(emailAddress, out token))
                {
                    return Task.FromResult<LoginToken>(null);
                }
            }
            return Task.FromResult(token);
        }

        /// <inheritdoc/>
        public Task<bool> DeleteTokenAsync(string teslaEmailAddress, string flexEmailAddress)
        {
            lock (CacheLock)
            {
                if (string.IsNullOrWhiteSpace(teslaEmailAddress))
                {
                    throw new ArgumentException(nameof(teslaEmailAddress));
                }
                if (string.IsNullOrWhiteSpace(flexEmailAddress))
                {
                    throw new ArgumentException(nameof(flexEmailAddress));
                }
                var emailAddress = $"Tesla:{teslaEmailAddress}, FlexCharging:{flexEmailAddress}";

                Tokens.Remove(emailAddress);

                if (OSSupportsTokenCache)
                {
                    WriteCacheFile();
                }
            }
            return Task.FromResult<bool>(true);
        }

        /// <inheritdoc/>
        public Task AddOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress, string flexEmailAddress)
        {
            lock (CacheLock)
            {
                if (string.IsNullOrWhiteSpace(teslaEmailAddress))
                {
                    throw new ArgumentException(nameof(teslaEmailAddress));
                }
                if (string.IsNullOrWhiteSpace(flexEmailAddress))
                {
                    throw new ArgumentException(nameof(flexEmailAddress));
                }
                if (loginToken == null)
                {
                    throw new ArgumentNullException(nameof(loginToken));
                }

                var emailAddress = $"Tesla:{teslaEmailAddress}, FlexCharging:{flexEmailAddress}";

                Tokens[emailAddress] = loginToken;

                if (OSSupportsTokenCache)
                {
                    WriteCacheFile();
                }
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task UpdateOAuthTokenAsync(LoginToken loginToken, string teslaEmailAddress, string flexEmailAddress)
        {
            lock (CacheLock)
            {
                if (string.IsNullOrWhiteSpace(teslaEmailAddress))
                {
                    throw new ArgumentException(nameof(teslaEmailAddress));
                }

                if (string.IsNullOrWhiteSpace(flexEmailAddress))
                {
                    throw new ArgumentException(nameof(flexEmailAddress));
                }

                if (loginToken == null)
                {
                    throw new ArgumentNullException(nameof(loginToken));
                }

                var emailAddress = $"Tesla:{teslaEmailAddress}, FlexCharging:{flexEmailAddress}";

                Tokens[emailAddress] = loginToken;

                if (OSSupportsTokenCache)
                {
                    WriteCacheFile();
                }
            }
            return Task.CompletedTask;
        }
        private void WriteCacheFile()
        {
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
            {
            }
        }
        private void ReadCacheFile()
        {
            Tokens.Clear();

            if (!File.Exists(CacheFileName))
            {
                return;
            }

            try
            {
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
    }
}

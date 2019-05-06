using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TeslaLib.Converters;
using TeslaLib.Models;

namespace TeslaLib
{
    internal static class LoginTokenCache
    {
        private const string CacheFileName = "TeslaLoginTokenCache.cache";

        private static readonly TimeSpan ExpirationTimeWindow = TimeSpan.FromDays(1);
        private static readonly Dictionary<string, LoginToken> Tokens = new Dictionary<string, LoginToken>();
        private static readonly object CacheLock = new object();
        private static volatile bool _haveReadCacheFile;
		
        // On iOS, Environment.OSVersion.Platform returns Unix.
        public static readonly bool OSSupportsTokenCache = Environment.OSVersion.Platform != PlatformID.Unix;

        private static void ReadCacheFile()
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
            catch(Exception e)
            {
                Console.WriteLine("Exception while reading Tesla LoginTokenCache file: {0}: {1}.  Deleting cache file.", e.GetType().Name, e.Message);
                File.Delete(CacheFileName);
            }
        }

        private static void WriteCacheFile()
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

        public static LoginToken GetToken(string emailAddress)
        {
            lock (CacheLock)
            {
                if (!_haveReadCacheFile && OSSupportsTokenCache)
                {
                    ReadCacheFile();
                    _haveReadCacheFile = true;
                }

                if (!Tokens.TryGetValue(emailAddress, out LoginToken token))
                {
                    return null;
                }

                // Ensure the LoginToken is still valid.
                var expirationTime = token.CreatedUtc.ToLocalTime() + UnixTimeConverter.FromUnixTimeSpan(token.ExpiresIn);

                if (DateTime.Now + ExpirationTimeWindow >= expirationTime)
                {
                    Tokens.Remove(emailAddress);
                    if (OSSupportsTokenCache)
                        WriteCacheFile();
                    token = null;
                }
                return token;
            }
        }

        public static void AddToken(string emailAddress, LoginToken token)
        {
            lock (CacheLock)
            {
                Tokens[emailAddress] = token;
                if (OSSupportsTokenCache)
                    WriteCacheFile();
            }
        }

        public static void ClearCache()
        {
            lock (CacheLock)
            {
                Tokens.Clear();
                File.Delete(CacheFileName);
            }
        }
    }
}

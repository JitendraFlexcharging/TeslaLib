// Code from https://github.com/briangru/TeslaAuth, with work from bassmaster187, Tom Hollander, briangru, and Ramon Smits
using System;

namespace TeslaAuth
{

    public class Tokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public TimeSpan ExpiresIn { get; set; }
    }
}
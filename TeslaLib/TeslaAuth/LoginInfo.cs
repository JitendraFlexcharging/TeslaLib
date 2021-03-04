// Code from https://github.com/briangru/TeslaAuth, with work from bassmaster187, Tom Hollander, briangru, and Ramon Smits
using System.Collections.Generic;

namespace TeslaAuth 
{
    internal class LoginInfo
    {
        public string CodeVerifier { get; set;}
        public string CodeChallenge { get; set;}
        public string State { get; set;}
        public Dictionary<string, string> FormFields { get; set;}
        public string UserName { get; set; }  // For error reporting
    }
}

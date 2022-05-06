using System;
using System.Collections.Generic;
using System.Text;
using TeslaAuth;

namespace TeslaLib
{
    public class TeslaLoad : ITeslaLoad
    {

        private const String TESLA_CLIENT_ID = "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384";
        private const String TESLA_CLIENT_SECRET = "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3";

        public TeslaLoad()
        {

        }

        public TeslaClient Client { get; private set; }

        public void LoadVehicle(string email, string password, string MFA = null, TeslaAuthHelper teslaAuthHelper = null)
        {
            Client = new TeslaClient(email, TESLA_CLIENT_ID, TESLA_CLIENT_SECRET, authHelper: teslaAuthHelper);
        }
    }
}

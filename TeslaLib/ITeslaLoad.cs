using TeslaAuth;

namespace TeslaLib
{
    public interface ITeslaLoad
    {
        TeslaClient Client { get; }

        void LoadVehicle(string email, string password, string MFA = null, TeslaAuthHelper teslaAuthHelper = null);
    }
}
using TeslaAuth;

namespace TeslaLib
{
    public interface ITeslaLoadVehicle
    {
        TeslaClient Client { get; }

        void LoadVehicle(string email, string password, string MFA = null, TeslaAuthHelper teslaAuthHelper = null);
    }
}
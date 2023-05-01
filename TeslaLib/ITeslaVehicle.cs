using RestSharp;
using System;
using System.Collections.Generic;
using TeslaLib.Models;

namespace TeslaLib
{
    public interface ITeslaVehicle
    {
        float? ApiVersion { get; set; }
        bool CalendarEnabled { get; set; }
        RestClient Client { get; set; }
        string Color { get; set; }
        string DisplayName { get; set; }
        long Id { get; set; }
        bool InService { get; set; }
        VehicleOptions Options { get; set; }
        VehicleState State { get; set; }
        List<string> Tokens { get; set; }
        long VehicleId { get; set; }
        string Vin { get; set; }

        ResultStatus DisableValetMode();
        ResultStatus EnableValetMode(int password);
        ResultStatus FlashLights();
        ResultStatus HonkHorn();

        VehicleData LoadVehicleData();

        [Obsolete("Use LoadVehicleData")]
        ChargeStateStatus LoadChargeStateStatus();

        [Obsolete("Use LoadVehicleData")]
        ClimateStateStatus LoadClimateStateStatus();

        [Obsolete("Use LoadVehicleData")]
        DriveStateStatus LoadDriveStateStatus();

        [Obsolete("Use LoadVehicleData")]
        GuiSettingsStatus LoadGuiStateStatus();
        bool LoadMobileEnabledStatus();
        string LoadStreamingValues(string values);

        [Obsolete("Use LoadVehicleData")]
        VehicleConfig LoadVehicleConfig();

        [Obsolete("Use LoadVehicleData")]
        VehicleStateStatus LoadVehicleStateStatus();

        ResultStatus LockDoors();
        ResultStatus OpenChargePortDoor();
        ResultStatus OpenFrontTrunk();
        ResultStatus OpenRearTrunk();
        ResultStatus OpenTrunk(string trunkType);
        ResultStatus RemoteStart(string password);
        ResultStatus ResetValetPin();
        ResultStatus SetChargeLimit(int stateOfChargePercent);
        ResultStatus SetChargeLimitToMaxRange();
        ResultStatus SetChargeLimitToStandard();
        ResultStatus SetPanoramicRoofLevel(PanoramicRoofState roofState, int percentOpen = 0);
        ResultStatus SetTemperatureSettings(int driverTemp = 17, int passengerTemp = 17);
        ResultStatus SetValetMode(bool enabled, int password = 0);
        ResultStatus StartCharge();
        ResultStatus StartHVAC();
        ResultStatus StopCharge();
        ResultStatus StopHVAC();
        ResultStatus UnlockDoors();
        VehicleState WakeUp();
    }
}
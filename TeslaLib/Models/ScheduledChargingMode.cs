using System;
using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    [Obsolete("Please use TeslaScheduledChargingMode instead.")]
    public enum ScheduledChargingMode
    {
        [EnumMember(Value = "Off")]
        Off,

        [EnumMember(Value = "StartAt")]
        StartAt,

        [EnumMember(Value = "DepartBy")]
        DepartBy,
    }
}

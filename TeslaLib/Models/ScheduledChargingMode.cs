using System.Runtime.Serialization;

namespace TeslaLib.Models
{
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

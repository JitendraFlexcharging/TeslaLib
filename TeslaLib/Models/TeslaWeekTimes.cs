using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    public enum TeslaWeekTimes
    {
        [EnumMember(Value = "")]  // ?
        Unknown,

        [EnumMember(Value = "all_week")]
        AllWeek,

        [EnumMember(Value = "weekdays")]
        Weekdays,
    }
}

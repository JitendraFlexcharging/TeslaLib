using System;
using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    [Obsolete("Please use TeslaWeekTimes instead.")]
    public enum WeekTimes
    {
        [EnumMember(Value = "")]  // ?
        Unknown,

        [EnumMember(Value = "all_week")]
        AllWeek,

        [EnumMember(Value = "weekdays")]
        Weekdays,
    }
}

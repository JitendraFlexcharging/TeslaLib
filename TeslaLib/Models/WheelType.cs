using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    public enum WheelType
    {
        Unknown = 0,

        [EnumMember(Value = "Base19")]
        Base19,

        [EnumMember(Value = "Silver21")]
        Silver21,

        [EnumMember(Value = "Charcoal21")]
        Charcoal21,

        CharcoalPerformance21,

        [EnumMember(Value = "AeroTurbine19")]
        AeroTurbine19,

        [EnumMember(Value = "AeroTurbine20")]
        AeroTurbine20,

        [EnumMember(Value = "Pinwheel18")]
        Pinwheel18,

        [EnumMember(Value = "Stiletto19")]
        Stiletto19,

        [EnumMember(Value = "Stiletto20")]
        Stiletto20,

        [EnumMember(Value = "Super21Silver")]
        Super21Silver,
    }
}

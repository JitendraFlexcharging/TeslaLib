using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    public enum TrimLevel
    {
        [EnumMember(Value = "00")]
        Standard,

        //[EnumMember(Value = "01")]
        //Performance,

        [EnumMember(Value = "02")]
        SignaturePerformance,   // General production signature trim?

        [EnumMember(Value = "0A")]
        AlphaPreproduction,

        [EnumMember(Value = "0B")]
        BetaPreproduction,

        [EnumMember(Value = "0C")]
        Preproduction,
    }
}

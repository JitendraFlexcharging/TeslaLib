using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    public enum InteriorDecor
    {
        [EnumMember(Value = "CF")]
        CarbonFiber,

        [EnumMember(Value = "LW")]
        Lacewood,

        [EnumMember(Value = "OM")]
        ObecheWoodMatte,

        [EnumMember(Value = "OG")]
        ObecheWoodGloss,

        [EnumMember(Value = "PB")]
        PianoBlack,

        [EnumMember(Value = "3W")]
        Model3Wood,

        [EnumMember(Value = "BA")]
        DarkAshWood,

        [EnumMember(Value = "BO")]
        FiguredAshWood
    }
}

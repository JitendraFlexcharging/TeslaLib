using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    // Model S paint codes:  https://teslamotorsclub.com/tmc/threads/model-s-paint-codes.25297/
    public enum TeslaColor
    {
        [EnumMember(Value = "BSB")]
        Black,

        [EnumMember(Value = "BCW")]
        White,

        [EnumMember(Value = "MSS")]
        Silver,  // Metallic silver

        [EnumMember(Value = "MTG")]
        MetallicDolphinGrey,

        [EnumMember(Value = "MAB")]
        MetallicBrown,

        [EnumMember(Value = "MBL")]
        MetallicBlack,  // Obsidian Black

        [EnumMember(Value = "MMB")]
        MetallicBlue,

        [EnumMember(Value = "MSG")]
        MetallicGreen,

        [EnumMember(Value = "MNG")]
        SteelGrey,

        [EnumMember(Value = "PSW")]
        PearlWhite,

        [EnumMember(Value = "PMR")]
        MulticoatRed,
        //Red = MULTICOAT_RED,

        // Not clear whether this exists.
        [EnumMember(Value = "MMR")]
        MulticoatRed2,

        [EnumMember(Value = "PSB")]
        DeepBlueMetallic,  // Originally called Ocean Blue

        [EnumMember(Value = "PSR")]
        SignatureRed,  // "Sunset Red"

        [EnumMember(Value = "PTI")]
        Titanium,  // Titanium metallic
    }
}

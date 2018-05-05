using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    // Model S paint codes:  https://teslamotorsclub.com/tmc/threads/model-s-paint-codes.25297/
    public enum TeslaColor
    {
        [EnumMember(Value = "BSB")]
        BLACK,

        [EnumMember(Value = "BCW")]
        WHITE,

        [EnumMember(Value = "MSS")]
        SILVER,  // Metallic silver

        [EnumMember(Value = "MTG")]
        METALLIC_DOLPHIN_GREY,

        [EnumMember(Value = "MAB")]
        METALLIC_BROWN,

        [EnumMember(Value = "MBL")]
        METALLIC_BLACK,  // Obsidian Black

        [EnumMember(Value = "MMB")]
        METALLIC_BLUE,

        [EnumMember(Value = "MSG")]
        METALLIC_GREEN,

        [EnumMember(Value = "MNG")]
        STEEL_GREY,

        [EnumMember(Value = "PSW")]
        PEARL_WHITE,

        [EnumMember(Value = "PMR")]
        MULTICOAT_RED,
        //Red = MULTICOAT_RED,

        // Not clear whether this exists.
        //[EnumMember(Value = "MMR")]
        //MULTICOAT_RED_2,

        [EnumMember(Value = "PSB")]
        DEEP_BLUE_METALLIC,  // Originally called Ocean Blue

        [EnumMember(Value = "PSR")]
        SIGNATURE_RED,  // "Sunset Red"

        [EnumMember(Value = "PTI")]
        TITANIUM,  // Titanium metallic
    }
}

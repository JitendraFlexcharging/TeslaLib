using System.Runtime.Serialization;

namespace TeslaLib.Models
{
    public enum RoofType
    {
        [EnumMember(Value = "Colored")]
        Colored,

        [EnumMember(Value = "None")]
        None,

        [EnumMember(Value = "Black")]
        Black,

        [EnumMember(Value = "Glass")]
        Glass,

        [EnumMember(Value = "AllGlassPanoramic")]
        AllGlassPanoramic,

        [EnumMember(Value = "ModelX")]
        ModelX,

        [EnumMember(Value = "Sunroof")]
        Sunroof,

        [EnumMember(Value = "RoofColorGlass")]
        RoofColorGlass,

        [EnumMember(Value = "FixedGlassRoof")]
        FixedGlassRoof, // Compatible with roofracks

        // Possible future roof glass types, based on reverse engineering Tesla software:/
        // https://twitter.com/greentheonly/status/1208160416305688576
        // TSA3_PET  (PET is a type of lamination)
        // TSA5_NOPET
    }
}

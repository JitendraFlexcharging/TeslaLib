using System;

namespace TeslaLib.Models
{
    [Obsolete("Please use VehicleModel instead.")]
    public enum Model
    {
        Unknown,
        S,
        X,
        Three,
        Y,
        Semi,
        Roadster,  // V2 of course
        Cybertruck
    }
}

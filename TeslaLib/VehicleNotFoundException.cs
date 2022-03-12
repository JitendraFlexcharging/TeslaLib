using System;
using System.Text;

namespace TeslaLib
{
    /// <summary>
    /// For when we receive a response saying "not found"
    /// </summary>
    public class VehicleNotFoundException : Exception
    {
        public VehicleNotFoundException() : base("Tesla says this vehicle could not be found.")
        {
        }

        public VehicleNotFoundException(String message) : base(message)
        {
        }
    }
}

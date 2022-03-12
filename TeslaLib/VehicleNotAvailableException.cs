using System;
using System.Text;

namespace TeslaLib
{
    /// <summary>
    /// For when we receive a response saying "vehicle is currently in service"
    /// </summary>
    public class VehicleNotAvailableException : Exception
    {
        public VehicleNotAvailableException() : base("Tesla vehicle is not currently available.")
        {
        }

        public VehicleNotAvailableException(String message) : base(message)
        {
        }
    }
}

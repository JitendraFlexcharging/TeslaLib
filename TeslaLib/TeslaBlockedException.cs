using System;
using System.Collections.Generic;
using System.Text;

namespace TeslaLib
{
    /// <summary>
    /// For when we receive a response saying "Blocked."
    /// </summary>
    public class TeslaBlockedException : Exception
    {
        public TeslaBlockedException() : base("Tesla server blocked this request.")
        {
        }
    }
}

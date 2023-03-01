using System;
using System.Collections.Generic;
using System.Text;

namespace TeslaLib
{
    /// <summary>
    /// For when we receive an Internal Server Error (HTTP code 500) from Tesla's servers.
    /// </summary>
    public class TeslaServerException : Exception
    {
        public TeslaServerException() : this("Tesla's server had an internal server error.")
        {
        }

        public TeslaServerException(string message) : base(message)
        {
        }
    }
}

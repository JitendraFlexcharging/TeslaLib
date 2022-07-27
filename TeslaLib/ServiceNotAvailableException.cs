using System;
using System.Collections.Generic;
using System.Text;

namespace TeslaLib
{
    public class ServiceNotAvailableException : Exception
    {
        public ServiceNotAvailableException() : base("Tesla's service is unavailable")
        {
        }

        public ServiceNotAvailableException(String message) : base(message)
        {
        }
    }
}

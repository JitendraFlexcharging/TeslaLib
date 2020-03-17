using System;
using System.Collections.Generic;
using System.Text;

namespace TeslaLib
{
    /// <summary>
    /// For when we receive a response saying "You have been temporarily blocked for making too many requests!"
    /// </summary>
    public class TeslaThrottlingException : Exception
    {
        public TeslaThrottlingException() : base("Tesla throttled this request - your account is too busy.")
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TeslaLib
{
    /// <summary>
    /// For when we can't log in due to likely expired password or Re-CAPTCHA issues
    /// </summary>
    public class TeslaAuthenticationException : Exception
    {
        public TeslaAuthenticationException() : base("Please re-enter your Tesla account credentials.")
        {
        }

        public TeslaAuthenticationException(String message) : base(message)
        {
        }
    }
}

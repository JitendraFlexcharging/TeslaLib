using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace TeslaLib
{
    /// <summary>
    /// Some accounts have MFA enabled.
    /// </summary>
    public class MultiFactorAuthenticationException : Exception
    {
        public MultiFactorAuthenticationException() : base("Multi-factor authentication is required for this account")
        {
        }

        public MultiFactorAuthenticationException(String message) : base(message)
        {
        }

        public MultiFactorAuthenticationException(String message, String userName) : base(message)
        {
            UserName = userName;
        }

        public String UserName { get; set; }
    }
}

using System;

namespace HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedUsername
{
    /// <summary>
    /// Exception to signal known Errors, that should abort the current checking mechanism
    /// </summary>
    public class HaveIBeenPwnedAbortException : Exception
    {
        public HaveIBeenPwnedAbortException(string message)
        :base(message)
        {
        }
    }
}
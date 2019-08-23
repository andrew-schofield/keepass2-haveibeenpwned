using System;

namespace HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedUsername
{
    public class ApiKeyException :Exception
    {
        public ApiKeyException(string message)
            :base(message)
        {
        }
    }
}
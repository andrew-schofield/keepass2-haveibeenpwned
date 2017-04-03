using System;

namespace HaveIBeenPwned
{
    public interface IBreach
    {
        string Name { get; }

        DateTime BreachDate { get; }
    }
}
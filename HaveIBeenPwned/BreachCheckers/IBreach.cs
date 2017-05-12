using System;

namespace HaveIBeenPwned.BreachCheckers
{
    public interface IBreach
    {
        string Title { get; }

        DateTime BreachDate { get; }
    }
}
using System;

namespace HaveIBeenPwned.BreachCheckers
{
    public interface IBreach
    {
        string Title { get; }

        DateTime BreachDate { get; }

        string Domain { get; }

        string Username { get; }
    }
}
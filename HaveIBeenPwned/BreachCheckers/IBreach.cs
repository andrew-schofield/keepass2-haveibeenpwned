using System;

namespace HaveIBeenPwned.BreachCheckers
{
    public interface IBreach
    {
        string Title { get; }

        DateTime BreachDate { get; }

        string[] DataClasses { get; }

        string Domain { get; }

        string Username { get; }

        string Description { get; }
    }
}
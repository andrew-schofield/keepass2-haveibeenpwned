using System;

namespace HaveIBeenPwned
{
    public interface IBreach
    {
        string Title { get; }

        DateTime BreachDate { get; }
    }
}
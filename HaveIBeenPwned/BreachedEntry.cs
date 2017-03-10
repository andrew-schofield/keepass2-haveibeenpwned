using KeePassLib;
using System;

namespace HaveIBeenPwned
{
    public class BreachedEntry
    {
        public PwEntry Entry { get; private set; }

        public DateTime BreachDate { get; private set; }

        public BreachedEntry(PwEntry entry, DateTime breachDate)
        {
            Entry = entry;
            BreachDate = breachDate;
        }
    }
}

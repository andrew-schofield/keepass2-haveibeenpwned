using KeePassLib;
using System;

namespace HaveIBeenPwned
{
    public class BreachedEntry
    {
        private IBreach breach;

        public PwEntry Entry { get; private set; }

        public DateTime BreachDate
        {
            get
            {
                return breach.BreachDate;
            }
        }

        public string BreachName
        {
            get
            {
                return breach.Name;
            }
        }

        public BreachedEntry(PwEntry entry, IBreach breach)
        {
            Entry = entry;
            this.breach = breach;
        }
    }
}

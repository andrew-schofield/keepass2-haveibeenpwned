using KeePassLib;
using System;

namespace HaveIBeenPwned.BreachCheckers
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
                return breach.Title;
            }
        }

        public string BreachUrl
        {
            get
            {
                return breach.Domain;
            }
        }

        public string BreachUsername
        {
            get
            {
                return breach.Username;
            }
        }

        public string Description
        {
            get
            {
                return breach.Description;
            }
        }
        
        public string[] DataClasses
        {
            get
            {
                return breach.DataClasses
                       ?? new string[0];
            }
        }

        public BreachedEntry(PwEntry entry, IBreach breach)
        {
            Entry = entry;
            this.breach = breach;
        }
    }
}

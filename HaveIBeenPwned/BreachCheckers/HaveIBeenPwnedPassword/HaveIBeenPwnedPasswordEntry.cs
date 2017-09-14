using KeePassLib;
using System;

namespace HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedPassword
{
    public class HaveIBeenPwnedPasswordEntry : IBreach
    {
        private string username;
        private string domain;

        public HaveIBeenPwnedPasswordEntry(string username, string domain, PwEntry entry)
        {
            this.username = username;
            this.domain = domain;
            Entry = entry;
        }

        public string Title {
            get
            {
                return "HIBP Password Breach";
            }
        }

        public DateTime BreachDate {
            get
            {
                return DateTime.Now;
            }
        }

        public string Domain {
            get
            {
                return domain;
            }
        }

        public string Username {
            get
            {
                return username;
            }
        }

        public PwEntry Entry { get; set; }
    }
}

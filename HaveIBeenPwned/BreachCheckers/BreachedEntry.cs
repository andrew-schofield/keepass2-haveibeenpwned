using KeePass.Plugins;
using KeePassLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HaveIBeenPwned.BreachCheckers
{
    public class BreachedEntry
    {
        private IBreach breach;
        private IPluginHost pluginHost;

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

        public BreachedEntry(IPluginHost pluginHost, PwEntry entry, IBreach breach)
        {
            Entry = entry;
            this.breach = breach;
            this.pluginHost = pluginHost;
        }

        public bool IsIgnored
        {
            get
            {
                if (Entry == null)
                {
                    // At the moment the only way a breach can have no entry is if it's a breached username
                    return GetIgnoredBreachForUserName().Contains(BreachName);
                }

                // Otherwise, see if this particular breach is ignored for this entry
                return GetIngoredBreachesForThisEntry().Contains(BreachName);
            }
        }

        private string[] GetIngoredBreachesForThisEntry()
        {
            if (Entry == null)
            {
                return new string[] { };
            }

            var items = Entry.CustomData.Get<string[]>(Resources.FieldNameIgnoredBreachs);
            return items == null ? new string[] { } : items;
        }

        private void SetIngoredBreachForThisEntry(string[] value)
        {
            if (Entry == null)
            {
                throw new NotSupportedException();
            }

            var arrayValue = value.OrderBy(x => x).ToArray();
            var modified = Entry.CustomData.Set(Resources.FieldNameIgnoredBreachs, arrayValue.Length > 0 ? arrayValue : null);
            if (modified)
            {
                pluginHost.MarkAsModified();
            }
        }

        public void IgnoreForThisEntry()
        {
            var items = GetIngoredBreachesForThisEntry().ToList();
            items.Add(BreachName);
            SetIngoredBreachForThisEntry(items.Distinct().ToArray());
        }

        public void StopIgnoringBreachForThisEntry()
        {
            var items = GetIngoredBreachesForThisEntry().Where(x => x != BreachName).ToArray();
            SetIngoredBreachForThisEntry(items);
        }

        public Dictionary<string, string[]> GetUserIgnoreList()
        {
            var items = pluginHost.Database.RootGroup.CustomData.Get<Dictionary<string, string[]>>(Resources.FieldNameIgnoredBreachs);
            return items == null ? new Dictionary<string, string[]>() { } : items;
        }

        private void SetGloballyIgnoredBreachesByUser(string[] breaches)
        {

            var currentIgnores = GetUserIgnoreList();
            if (breaches != null && breaches.Length > 0)
            {
                currentIgnores[BreachUsername] = breaches;
            }
            else
            {
                currentIgnores.Remove(BreachUsername);
            }

            var entry = pluginHost.Database.RootGroup;
            var modified = entry.CustomData.Set(Resources.FieldNameIgnoredBreachs, currentIgnores.Count > 0 ? currentIgnores : null);
            if (modified)
            {
                pluginHost.MarkAsModified();
            }
        }

        public string[] GetIgnoredBreachForUserName()
        {
            string[] value;
            if (!GetUserIgnoreList().TryGetValue(BreachUsername, out value))
            {
                value = new string[] { };
            }

            return value;
        }

        public void AddBreachToUserIgnoreList()
        {
            var value = GetIgnoredBreachForUserName().Concat(new[] { BreachName }).OrderBy(x => x).ToArray();
            SetGloballyIgnoredBreachesByUser(value);
        }

        public void ClearUserIgnoreList()
        {
            SetGloballyIgnoredBreachesByUser(null);
        }
    }
}

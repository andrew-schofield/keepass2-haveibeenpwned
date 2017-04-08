using KeePass.Plugins;
using KeePassLib;
using System;
using System.Linq;

namespace HaveIBeenPwned
{
    public static class PwEntryExtension
    {
        public static DateTime GetPasswordLastModified(this PwEntry entry)
        {
            if(entry.History != null && entry.History.Any())
            {
                var sortedEntries = entry.History.OrderByDescending(h => h.LastModificationTime);
                foreach(var historyEntry in sortedEntries)
                {
                    if(entry.Strings.GetSafe(PwDefs.PasswordField).ReadString() != historyEntry.Strings.GetSafe(PwDefs.PasswordField).ReadString())
                    {
                        return historyEntry.LastModificationTime;
                    }
                }
                return sortedEntries.Last().LastModificationTime;
            }

            return entry.LastModificationTime;
        }

        public static bool IsDeleted(this PwEntry entry, IPluginHost pluginHost)
        {
            return entry.ParentGroup.Uuid.CompareTo(pluginHost.Database.RecycleBinUuid) == 0;
        }
    }
}

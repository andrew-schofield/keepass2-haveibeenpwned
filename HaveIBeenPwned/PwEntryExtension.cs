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
            if (entry.History != null && entry.History.Any())
            {
                var sortedEntries = entry.History.OrderByDescending(h => h.LastModificationTime);
                foreach (var historyEntry in sortedEntries)
                {
                    if (entry.Strings.GetSafe(PwDefs.PasswordField).ReadString() != historyEntry.Strings.GetSafe(PwDefs.PasswordField).ReadString())
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

        public static string GetUrlDomain(this PwEntry entry)
        {
            var url = entry.Strings.ReadSafe(PwDefs.UrlField).ToLower();
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            if (!url.Contains("://"))
            {
                // add http as a fallback protocol
                url = string.Format("http://{0}", url);
            }

            try
            {
                var domain = new Uri(url).DnsSafeHost;
                if (domain.StartsWith("www."))
                {
                    domain = domain.Substring(4);
                }
                return domain;
            }
            catch (UriFormatException) { return string.Empty; }
        }
    }
}

using KeePass.Plugins;
using KeePassLib;
using System;
using System.Drawing;
using System.Linq;

namespace HaveIBeenPwned.Extensions
{
    public static class PwEntryExtensions
    {
        public static DateTime GetPasswordLastModified(this PwEntry entry)
        {
            if (entry.History != null && entry.History.Any())
            {
                var sortedEntries = entry.History.OrderByDescending(h => h.LastModificationTime);
                foreach (var historyEntry in sortedEntries)
                {
                    if(!entry.Strings.GetSafe(PwDefs.PasswordField).Equals(historyEntry.Strings.GetSafe(PwDefs.PasswordField)))
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

        public static Image GetIcon(this PwEntry entry, IPluginHost pluginHost)
        {
            var entryIcon = entry.IconId;
            var customIcon = entry.CustomIconUuid;

            if (!customIcon.Equals(PwUuid.Zero))
            {
                int w = KeePass.UI.DpiUtil.ScaleIntX(16);
                int h = KeePass.UI.DpiUtil.ScaleIntY(16);

                var imgCustom = pluginHost.Database.GetCustomIcon(customIcon, w, h);
                return (imgCustom ?? pluginHost.MainWindow.ClientIcons.Images[(int)entryIcon]);
            }
            else
            {
                return pluginHost.MainWindow.ClientIcons.Images[(int)entryIcon];
            }
        }
    }
}

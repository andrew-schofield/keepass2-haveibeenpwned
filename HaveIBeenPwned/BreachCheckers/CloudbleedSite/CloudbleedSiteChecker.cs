using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.IO;
using System.Windows.Forms;
using KeePass.Plugins;
using System.Threading.Tasks;
using System.Drawing;
using KeePassExtensions;
using KeePassLib;

namespace HaveIBeenPwned.BreachCheckers.CloudbleedSite
{
    public class CloudbleedSiteChecker : BaseChecker
    {
        public CloudbleedSiteChecker(HttpClient httpClient, IPluginHost pluginHost)
            : base(httpClient, pluginHost)
        {
        }

        public override Image BreachLogo
        {
            get { return Resources.cloudbleed.ToBitmap(); }
        }

        public override string BreachTitle
        {
            get { return "Cloudbleed Vulnerability"; }
        }

        public async override Task<List<BreachedEntry>> CheckGroup(PwGroup group, bool expireEntries, bool oldEntriesOnly, bool ignoreDeleted, IProgress<ProgressItem> progressIndicator)
        {
            progressIndicator.Report(new ProgressItem(0, "Getting Cloudbleed breach list..."));
            var breaches = await GetBreaches(progressIndicator);
            var entries = group.GetEntries(true).Where(e => !ignoreDeleted || !e.IsDeleted(pluginHost));
            var breachedEntries = new List<BreachedEntry>();

            uint counter = 0;
            var entryCount = entries.Count();
            await Task.Run(() =>
            {
                foreach (var entry in entries)
                {
                    var url = entry.GetUrlDomain();

                    if (!string.IsNullOrEmpty(url))
                    {
                        var lastModified = entry.GetPasswordLastModified();
                        var domainBreaches = breaches.Where(b => url == b && (!oldEntriesOnly || lastModified < new DateTime(2017, 02, 17)));
                        if (domainBreaches.Any())
                        {
                            breachedEntries.Add(new BreachedEntry(entry, new CloudbleedSiteEntry(string.Empty, entry.GetUrlDomain())));
                            if (expireEntries)
                            {
                                ExpireEntry(entry);
                            }
                        }
                    }

                    progressIndicator.Report(new ProgressItem((uint)((double)counter / entryCount * 100), string.Format("Checking {0} for breaches", url)));

                    counter++;
                }
            });

            breaches = null;

            return breachedEntries;
        }
        
        private async Task<HashSet<string>> GetBreaches(IProgress<ProgressItem> progressIndicator)
        {
            HashSet<string> breaches = new HashSet<string>();
            var dataLocation = KeePassLib.Native.NativeLib.IsUnix() ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.CommonApplicationData;
            var cloudBleedDataFile = Path.Combine(Environment.GetFolderPath(dataLocation), "KeePass", "cloudbleed.txt");

            if (File.Exists(cloudBleedDataFile))
            {
                using (var fileStream = new FileStream(cloudBleedDataFile, FileMode.Open, FileAccess.Read))
                {
                    breaches = await ExtractBreachesFromStream(fileStream, progressIndicator);
                }
            }
            else
            {                
                HttpResponseMessage response = await client.GetAsync(new Uri("https://raw.githubusercontent.com/pirate/sites-using-cloudflare/master/sorted_unique_cf.txt"));
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    Directory.CreateDirectory(Path.GetDirectoryName(cloudBleedDataFile));
                    using (var fileStream = new FileStream(cloudBleedDataFile, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                    stream.Seek(0, SeekOrigin.Begin);

                    breaches = await ExtractBreachesFromStream(stream, progressIndicator);
                }
                else
                {
                    MessageBox.Show(string.Format("Unable to check githubusercontent.com (returned Status: {0})", response.StatusCode), Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return breaches;
        }

        private async Task<HashSet<string>> ExtractBreachesFromStream(Stream stream, IProgress<ProgressItem> progressIndicator)
        {
            var breaches = new HashSet<string>();
            using (var rd = new StreamReader(stream))
            {
                while (true)
                {
                    var line = await rd.ReadLineAsync();
                    if (line == null)
                        break;
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        breaches.Add(line.ToLower().Trim());
                    }
                }
            }

            return breaches;
        }
    }
}

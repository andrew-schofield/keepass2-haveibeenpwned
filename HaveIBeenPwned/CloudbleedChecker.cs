using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using KeePassLib;
using System.IO;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.Forms;
using System.Threading.Tasks;
using System.Drawing;

namespace HaveIBeenPwned
{
    public class CloudbleedChecker : BaseChecker
    {
        public CloudbleedChecker(PwDatabase database, HttpClient httpClient, IPluginHost pluginHost)
            : base(database, httpClient, pluginHost)
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

        public async override Task<List<BreachedEntry>> CheckDatabase(bool expireEntries, bool oldEntriesOnly, bool ignoreDeleted)
        {
            var breaches = await GetBreaches();
            var entries = passwordDatabase.RootGroup.GetEntries(true).Where(e => !ignoreDeleted || !e.IsDeleted(pluginHost));
            var breachedEntries = new List<BreachedEntry>();
            StatusProgressForm progressForm = new StatusProgressForm();
            var cloudbleedEntry = new CloudbleedEntry();

            progressForm.InitEx("Checking Cloudbleed Breaches", true, false, pluginHost.MainWindow);
            progressForm.Show();
            progressForm.SetProgress(0);
            uint counter = 0;
            var entryCount = entries.Count();
            foreach (var entry in entries)
            {
                progressForm.SetProgress((uint)((double)counter / entryCount * 100));
                var url = entry.GetUrlDomain();
                progressForm.SetText(string.Format("Checking {0} for breaches", url), KeePassLib.Interfaces.LogStatusType.Info);
                if (!string.IsNullOrEmpty(url))
                {                    
                    var lastModified = entry.GetPasswordLastModified();
                    var domainBreaches = breaches.Where(b => url == b && (!oldEntriesOnly || lastModified < new DateTime(2017, 02, 17)));
                    if (domainBreaches.Any())
                    {
                        breachedEntries.Add(new BreachedEntry(entry, cloudbleedEntry));
                        if (expireEntries)
                        {
                            ExpireEntry(entry);
                        }
                    }
                }
                counter++;
                if(progressForm.UserCancelled)
                {
                    break;
                }
            }
            progressForm.Hide();
            progressForm.Close();

            return breachedEntries;
        }
        
        private async Task<HashSet<string>> GetBreaches()
        {
            HashSet<string> breaches = new HashSet<string>();
            var dataLocation = KeePassLib.Native.NativeLib.IsUnix() ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.CommonApplicationData;
            var cloudBleedDataFile = Path.Combine(Environment.GetFolderPath(dataLocation), "KeePass", "cloudbleed.txt");

            if (File.Exists(cloudBleedDataFile))
            {
                using (var fileStream = new FileStream(cloudBleedDataFile, FileMode.Open, FileAccess.Read))
                {
                    ExtractBreachesFromStream(breaches, fileStream);
                }
            }
            else
            {
                StatusProgressForm progressForm = new StatusProgressForm();

                progressForm.InitEx("Downloading Cloudbleed Data File", true, false, pluginHost.MainWindow);
                progressForm.Show();
                progressForm.SetProgress(0);
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

                    progressForm.SetProgress(100);
                    ExtractBreachesFromStream(breaches, stream);
                }
                else
                {
                    MessageBox.Show(string.Format("Unable to check githubusercontent.com (returned Status: {0})", response.StatusCode), Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                progressForm.Hide();
                progressForm.Close();
            }
            return breaches;
        }

        private static void ExtractBreachesFromStream(HashSet<string> breaches, Stream stream)
        {
            using (var rd = new StreamReader(stream))
            {
                while (true)
                {
                    var line = rd.ReadLine();
                    if (line == null)
                        break;
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        breaches.Add(line.ToLower().Trim());
                    }
                }
            }
        }
    }
}

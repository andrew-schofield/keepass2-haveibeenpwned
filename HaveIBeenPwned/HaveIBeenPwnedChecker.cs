using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KeePassLib;
using System.Net.Http;
using Newtonsoft.Json;
using KeePass.Forms;
using KeePass.Plugins;

namespace HaveIBeenPwned
{
    public class HaveIBeenPwnedChecker : BaseChecker
    {
        public HaveIBeenPwnedChecker(PwDatabase database, HttpClient httpClient, IPluginHost pluginHost)
            : base(database, httpClient, pluginHost)
        {
        }

        public override void CheckDatabase(bool expireEntries, bool oldEntriesOnly)
        {
            bool breachesFound = false;
            var breaches = GetBreaches();
            var entries = passwordDatabase.RootGroup.GetEntries(true);
            StatusProgressForm progressForm = new StatusProgressForm();

            progressForm.InitEx("Checking Cloudbleed Breaches", true, false, pluginHost.MainWindow);
            progressForm.Show();
            progressForm.SetProgress(0);
            uint counter = 0;
            var entryCount = entries.Count();
            foreach (var entry in entries)
            {
                progressForm.SetProgress((uint)((double)counter / entryCount * 100));
                var url = entry.Strings.ReadSafe(PwDefs.UrlField).ToLower();
                progressForm.SetText($"Checking {url} for breaches", KeePassLib.Interfaces.LogStatusType.Info);
                var userName = entry.Strings.ReadSafe(PwDefs.UserNameField);
                var lastModified = entry.LastModificationTime;
                if(!string.IsNullOrEmpty(url))
                {
                    var domainBreaches = breaches.Where(b => !string.IsNullOrWhiteSpace(b.Domain) && url.Contains(b.Domain) && (!oldEntriesOnly || lastModified < b.BreachDate)).OrderBy(b => b.BreachDate);
                    if (domainBreaches.Any())
                    {
                        breachesFound = true;
                        MessageBox.Show($"Potentially pwned account details for: {url}\r\nBreached on: {domainBreaches.Last().BreachDate}\r\nThis entry was last modified on: {lastModified}", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if(expireEntries)
                        {
                            ExpireEntry(entry);
                        }
                    }
                }
                counter++;
                if (progressForm.UserCancelled)
                {
                    break;
                }
            }
            progressForm.Hide();
            progressForm.Close();
            if (!breachesFound)
            {
                MessageBox.Show("No breached domains found.", Resources.MessageTitle);
            }
        }

        private List<HaveIBeenPwnedEntry> GetBreaches()
        {
            StatusProgressForm progressForm = new StatusProgressForm();

            progressForm.InitEx("Downloading Have I Been Pwned? Breach List", true, false, pluginHost.MainWindow);
            progressForm.Show();
            progressForm.SetProgress(0);
            List<HaveIBeenPwnedEntry> breaches = null;
            HttpResponseMessage response = client.GetAsync(new Uri("https://haveibeenpwned.com/api/v2/breaches")).Result;
            if (response.IsSuccessStatusCode)
            {
                var jsonString = response.Content.ReadAsStringAsync().Result;
                breaches = JsonConvert.DeserializeObject<List<HaveIBeenPwnedEntry>>(jsonString);
            }
            else
            {
                MessageBox.Show($"Unable to check haveibeenpwned.com (returned Status: {response.StatusCode})", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progressForm.SetProgress(100);

            progressForm.Hide();
            progressForm.Close();
            return breaches;
        }
    }
}

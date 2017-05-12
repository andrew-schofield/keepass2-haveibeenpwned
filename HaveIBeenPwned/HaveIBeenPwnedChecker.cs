using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KeePassLib;
using System.Net.Http;
using Newtonsoft.Json;
using KeePass.Forms;
using KeePass.Plugins;
using System.Threading.Tasks;
using System.Drawing;

namespace HaveIBeenPwned
{
    public class HaveIBeenPwnedChecker : BaseChecker
    {
        public HaveIBeenPwnedChecker(PwDatabase database, HttpClient httpClient, IPluginHost pluginHost)
            : base(database, httpClient, pluginHost)
        {
        }

        public override Image BreachLogo
        {
            get { return Resources.hibp.ToBitmap(); }
        }

        public override string BreachTitle
        {
            get { return "Have I Been Pwned"; }
        }

        public async override Task<List<BreachedEntry>> CheckDatabase(bool expireEntries, bool oldEntriesOnly, bool ignoreDeleted, IProgress<ProgressItem> progressIndicator)
        {
            var breaches = await GetBreaches(progressIndicator);
            var entries = passwordDatabase.RootGroup.GetEntries(true).Where(e => !ignoreDeleted || !e.IsDeleted(pluginHost));
            var breachedEntries = new List<BreachedEntry>();

            uint counter = 0;
            var entryCount = entries.Count();
            await Task.Run(() =>
                {
                    foreach (var entry in entries)
                    {
                        var url = entry.GetUrlDomain();

                        var userName = entry.Strings.ReadSafe(PwDefs.UserNameField);
                        var lastModified = entry.GetPasswordLastModified();
                        if (!string.IsNullOrEmpty(url))
                        {
                            var domainBreaches = breaches.Where(b => !string.IsNullOrWhiteSpace(b.Domain) && url == b.Domain && (!oldEntriesOnly || lastModified < b.BreachDate)).OrderBy(b => b.BreachDate);
                            if (domainBreaches.Any())
                            {
                                breachedEntries.Add(new BreachedEntry(entry, domainBreaches.Last()));
                                if (expireEntries)
                                {
                                    ExpireEntry(entry);
                                }
                            }
                        }
                        // this checker is so quick it probably doesn't need to report progress
                        //progressIndicator.Report(new ProgressItem((uint)((double)counter / entryCount * 100), string.Format("Checking {0} for breaches", url)));
                    }
                    counter++;
                });

            return breachedEntries;
        }

        private async Task<List<HaveIBeenPwnedEntry>> GetBreaches(IProgress<ProgressItem> progressIndicator)
        {            
            List<HaveIBeenPwnedEntry> breaches = null;
            HttpResponseMessage response = null;
            try
            {
                response = await client.GetAsync(new Uri("https://haveibeenpwned.com/api/v2/breaches"));
            }
            catch(Exception ex)
            {
                throw ex;
            }
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                breaches = JsonConvert.DeserializeObject<List<HaveIBeenPwnedEntry>>(jsonString);
            }
            else
            {
                MessageBox.Show(string.Format("Unable to check haveibeenpwned.com (returned Status: {0})", response.StatusCode), Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                        
            return breaches;
        }
    }
}

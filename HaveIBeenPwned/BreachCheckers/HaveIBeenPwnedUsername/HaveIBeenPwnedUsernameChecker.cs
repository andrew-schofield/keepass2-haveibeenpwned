using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KeePassLib;
using System.Net.Http;
using Newtonsoft.Json;
using KeePass.Plugins;
using System.Threading.Tasks;
using System.Drawing;
using KeePassExtensions;
using System.Threading;

namespace HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedUsername
{
    public class HaveIBeenPwnedUsernameChecker : BaseChecker
    {
        public HaveIBeenPwnedUsernameChecker(HttpClient httpClient, IPluginHost pluginHost)
            : base(httpClient, pluginHost)
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
            progressIndicator.Report(new ProgressItem(0, "Getting HaveIBeenPwned breach list..."));
            var entries = passwordDatabase.RootGroup.GetEntries(true).Where(e => !ignoreDeleted || !e.IsDeleted(pluginHost));
            var usernames = entries.Select(e => e.Strings.ReadSafe(PwDefs.UserNameField)).Distinct();
            var breaches = await GetBreaches(progressIndicator, usernames);
            var breachedEntries = new List<BreachedEntry>();

            uint counter = 0;
            var entryCount = entries.Count();
            
            await Task.Run(() =>
            {
                foreach (var breach in breaches)
                {
                    breachedEntries.Add(new BreachedEntry(entries.FirstOrDefault(e => e.GetUrlDomain() == breach.Domain), breach));
                }
            });

            return breachedEntries;
        }

        private async Task<List<HaveIBeenPwnedUsernameEntry>> GetBreaches(IProgress<ProgressItem> progressIndicator, IEnumerable<string> usernames)
        {
            List<HaveIBeenPwnedUsernameEntry> allBreaches = new List<HaveIBeenPwnedUsernameEntry>();
            foreach (var username in usernames.Where(u => !string.IsNullOrWhiteSpace(u) && !u.StartsWith("{REF:")))
            {
                List<HaveIBeenPwnedUsernameEntry> breaches = null;
                HttpResponseMessage response = null;
                try
                {
                    response = await client.GetAsync(new Uri("https://haveibeenpwned.com/api/v2/breachedaccount/" + username));
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    breaches = JsonConvert.DeserializeObject<List<HaveIBeenPwnedUsernameEntry>>(jsonString);
                    breaches.ForEach(b => b.Username = username);
                }
                else if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    MessageBox.Show(string.Format("Unable to check haveibeenpwned.com (returned Status: {0})", response.StatusCode), Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (breaches != null)
                {
                    allBreaches.AddRange(breaches);
                }
                // hibp has a rate limit of 1500ms
                await Task.Delay(1500);
            }
            
                        
            return allBreaches;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using KeePassLib;
using System.Net.Http;
using Newtonsoft.Json;

namespace HaveIBeenPwned
{
    public class HaveIBeenPwnedChecker
    {
        private PwDatabase passwordDatabase;
        private HttpClient client;

        public HaveIBeenPwnedChecker(PwDatabase database, HttpClient httpClient)
        {
            passwordDatabase = database;
            client = httpClient;
        }

        public void CheckHaveIBeenPwned(bool expireEntries, bool oldEntriesOnly)
        {
            bool breachesFound = false;
            var breaches = GetBreaches();
            var entries = passwordDatabase.RootGroup.GetEntries(true);
            foreach (var entry in entries)
            {
                var url = entry.Strings.ReadSafe(PwDefs.UrlField).ToLower();
                var userName = entry.Strings.ReadSafe(PwDefs.UserNameField);
                var lastModified = entry.LastModificationTime;
                if(!string.IsNullOrEmpty(url))
                {
                    var domainBreaches = breaches.Where(b => url.Contains(b.Domain) && (!oldEntriesOnly || lastModified < b.BreachDate)).OrderBy(b => b.BreachDate);
                    if (domainBreaches.Any())
                    {
                        breachesFound = true;
                        MessageBox.Show($"Potentially pwned account details for: {url}\r\nBreached on: {domainBreaches.Last().BreachDate}\r\nThis entry was last modified on: {lastModified}", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if(expireEntries)
                        {
                            entry.Expires = true;
                            entry.ExpiryTime = DateTime.Now;
                        }
                    }
                }
            }
            if(!breachesFound)
            {
                MessageBox.Show("No breached domains found.", Resources.MessageTitle);
            }
        }

        private List<HaveIBeenPwnedEntry> GetBreaches()
        {
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
            return breaches;
        }
    }
}

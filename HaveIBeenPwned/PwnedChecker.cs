using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePassLib;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HaveIBeenPwned
{
    public class PwnedChecker
    {
        PwDatabase passwordDatabase;
        static HttpClient client = new HttpClient();

        public PwnedChecker(PwDatabase database)
        {
            passwordDatabase = database;
            client.BaseAddress = new Uri("https://haveibeenpwned.com/api/breaches");
            client.DefaultRequestHeaders.Add("api-version", "2");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("KeePass HIBP Checker");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void CheckHaveIBeenPwned()
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
                    var domainBreaches = breaches.Where(b => url.Contains(b.Domain)).OrderBy(b => b.BreachDate);
                    if (domainBreaches.Any())
                    {
                        breachesFound = true;
                        MessageBox.Show($"Potentially pwned account details for: {url}\r\nBreached on: {domainBreaches.Last().BreachDate}\r\nThis entry was last modified on: {lastModified}","HaveIBeenPwned Checker");
                    }
                }
            }
            if(!breachesFound)
            {
                MessageBox.Show("No breached domains found.", "HaveIBeenPwned Checker");
            }
        }

        private static List<HaveIBeenPwnedEntry> GetBreaches()
        {
            List<HaveIBeenPwnedEntry> breaches = null;
            HttpResponseMessage response = client.GetAsync("").Result;
            if (response.IsSuccessStatusCode)
            {
                breaches = response.Content.ReadAsAsync<List<HaveIBeenPwnedEntry>>().Result;
            }
            return breaches;
        }
    }
}

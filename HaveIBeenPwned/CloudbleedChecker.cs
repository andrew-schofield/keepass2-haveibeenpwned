using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using System.IO;
using System.Windows.Forms;

namespace HaveIBeenPwned
{
    public class CloudbleedChecker : BaseChecker
    {
        public CloudbleedChecker(PwDatabase database, HttpClient httpClient) : base(database, httpClient)
        {
        }

        public override void CheckDatabase(bool expireEntries, bool oldEntriesOnly)
        {
            bool breachesFound = false;
            var breaches = GetBreaches();
            var entries = passwordDatabase.RootGroup.GetEntries(true);
            foreach (var entry in entries)
            {
                var url = entry.Strings.ReadSafe(PwDefs.UrlField).ToLower();
                if (!string.IsNullOrEmpty(url))
                {
                    if(!url.Contains("://"))
                    {
                        // add http as a fallback protocol
                        url = $"http://{url}";
                    }
                    var uri = new Uri(url).DnsSafeHost;
                    if(uri.StartsWith("www."))
                    {
                        uri = uri.Skip(4).ToString();
                    }
                    var userName = entry.Strings.ReadSafe(PwDefs.UserNameField);
                    var lastModified = entry.LastModificationTime;
                    var domainBreaches = breaches.Where(b => uri == b && (!oldEntriesOnly || lastModified < new DateTime(2017, 02, 17)));
                    if (domainBreaches.Any())
                    {
                        breachesFound = true;
                        MessageBox.Show($"Potentially leaked account details for: {url}\r\nBreached on: {new DateTime(2017, 02, 17)}\r\nThis entry was last modified on: {lastModified}", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        if (expireEntries)
                        {
                            ExpireEntry(entry);
                        }
                    }
                }
            }
            if (!breachesFound)
            {
                MessageBox.Show("No breached domains found.", Resources.MessageTitle);
            }
        }



        private HashSet<string> GetBreaches()
        {
            HashSet<string> breaches = new HashSet<string>();
            var cloudBleedDataFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "KeePass", "cloudbleed.txt");

            if (File.Exists(cloudBleedDataFile))
            {
                using (var fileStream = new FileStream(cloudBleedDataFile, FileMode.Open, FileAccess.Read))
                {
                    ExtractBreachesFromStream(breaches, fileStream);
                }
            }
            else
            {
                HttpResponseMessage response = client.GetAsync(new Uri("https://raw.githubusercontent.com/pirate/sites-using-cloudflare/master/sorted_unique_cf.txt")).Result;
                if (response.IsSuccessStatusCode)
                {
                    var stream = response.Content.ReadAsStreamAsync().Result;
                    Directory.CreateDirectory(Path.GetDirectoryName(cloudBleedDataFile));
                    using (var fileStream = new FileStream(cloudBleedDataFile, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }
                    stream.Seek(0, SeekOrigin.Begin);
                    ExtractBreachesFromStream(breaches, stream);
                }
                else
                {
                    MessageBox.Show($"Unable to check githubusercontent.com (returned Status: {response.StatusCode})", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

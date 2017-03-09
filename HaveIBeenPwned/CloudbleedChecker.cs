using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using KeePassLib;
using System.IO;
using System.Windows.Forms;
using KeePass.Plugins;
using KeePass.Forms;

namespace HaveIBeenPwned
{
    public class CloudbleedChecker : BaseChecker
    {
        public CloudbleedChecker(PwDatabase database, HttpClient httpClient, IPluginHost pluginHost)
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
                progressForm.SetText(string.Format("Checking {0} for breaches", url), KeePassLib.Interfaces.LogStatusType.Info);
                if (!string.IsNullOrEmpty(url))
                {
                    if(!url.Contains("://"))
                    {
                        // add http as a fallback protocol
                        url = string.Format("http://{0}", url);
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
                        MessageBox.Show(string.Format("Potentially leaked account details for: {0}\r\nBreached on: {1}\r\nThis entry was last modified on: {2}", url, new DateTime(2017, 02, 17), lastModified), Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                StatusProgressForm progressForm = new StatusProgressForm();

                progressForm.InitEx("Downloading Cloudbleed Data File", true, false, pluginHost.MainWindow);
                progressForm.Show();
                progressForm.SetProgress(0);
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

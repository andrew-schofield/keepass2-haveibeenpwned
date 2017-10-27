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
using System.Security.Cryptography;

namespace HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedPassword
{
    public class HaveIBeenPwnedPasswordChecker : BaseChecker
    {
        public HaveIBeenPwnedPasswordChecker(HttpClient httpClient, IPluginHost pluginHost)
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
            var breaches = await GetBreaches(progressIndicator, entries);
            var breachedEntries = new List<BreachedEntry>();
            
            await Task.Run(() =>
            {
                foreach (var breach in breaches)
                {
                    var pwEntry = breach.Entry;
                    if(pwEntry != null)
                    {
                        if (expireEntries)
                        {
                            ExpireEntry(pwEntry);
                        }
                    }

                    breachedEntries.Add(new BreachedEntry(pwEntry, breach));
                }
            });

            return breachedEntries;
        }

        private async Task<List<HaveIBeenPwnedPasswordEntry>> GetBreaches(IProgress<ProgressItem> progressIndicator, IEnumerable<PwEntry> entries)
        {
            List<HaveIBeenPwnedPasswordEntry> allBreaches = new List<HaveIBeenPwnedPasswordEntry>();
            /*int counter = 0;
            SHA1 sha = new SHA1CryptoServiceProvider();
            foreach (var entry in entries)
            {
                counter++;
                progressIndicator.Report(new ProgressItem((uint)((double)counter / entries.Count() * 100), string.Format("Checking \"{0}\" for breaches", entry.Strings.ReadSafe(PwDefs.TitleField))));
                if(entry.Strings.Get(PwDefs.PasswordField) == null || string.IsNullOrWhiteSpace(entry.Strings.ReadSafe(PwDefs.PasswordField)) || entry.Strings.ReadSafe(PwDefs.PasswordField).StartsWith("{REF:")) continue;
                var passwordHash = sha.ComputeHash(entry.Strings.Get(PwDefs.PasswordField).ReadUtf8()).Select(x => x.ToString("x2")).ToArray();
            } */    
            return allBreaches;
        }
    }
}

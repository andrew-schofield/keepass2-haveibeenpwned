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
using System.IO;
using KeePass.Forms;

namespace HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedPassword
{
    public class HaveIBeenPwnedPasswordChecker : BaseChecker
    {
        private static readonly string[] HASH_DATA_FILES = { "pwned-passwords-1.0.txt", "pwned-passwords-update-1.txt", "pwned-passwords-update-2.txt" };

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
            Dictionary<PwUuid, HaveIBeenPwnedPasswordEntry> allBreaches = new Dictionary<PwUuid, HaveIBeenPwnedPasswordEntry>();
            var dataLocation = KeePassLib.Native.NativeLib.IsUnix() ? Environment.SpecialFolder.LocalApplicationData : Environment.SpecialFolder.CommonApplicationData;
            var hibpPasswordPath = Path.Combine(Environment.GetFolderPath(dataLocation), "KeePass", "HIBP_passwords");
            var throttle = new Throttle(TimeSpan.FromMilliseconds(50));

            bool foundHashFiles = false;
            foreach (var hashFile in HASH_DATA_FILES)
            {
                if (File.Exists(Path.Combine(hibpPasswordPath, hashFile))) foundHashFiles = true;
            }
            if (!foundHashFiles)
            {
                throw new IOException(string.Format(Resources.NoPasswordsFiles, hibpPasswordPath, string.Join("\r\n", HASH_DATA_FILES)));
            }

            List<Tuple<PwEntry, string>> entryHashes = new List<Tuple<PwEntry, string>>();
            SHA1 sha = new SHA1CryptoServiceProvider();
            foreach (var entry in entries)
            {
                if (entry.Strings.Get(PwDefs.PasswordField) == null || string.IsNullOrWhiteSpace(entry.Strings.ReadSafe(PwDefs.PasswordField)) || entry.Strings.ReadSafe(PwDefs.PasswordField).StartsWith("{REF:")) continue;
                var passwordHash = string.Join("", sha.ComputeHash(entry.Strings.Get(PwDefs.PasswordField).ReadUtf8()).Select(x => x.ToString("x2")));
                entryHashes.Add(new Tuple<PwEntry, string>(entry, passwordHash));
            }

            foreach (var hashFile in HASH_DATA_FILES)
            {
                var hashFilePath = Path.Combine(hibpPasswordPath, hashFile);
                if (File.Exists(hashFilePath))
                {
                    using (var fileStream = new FileStream(hashFilePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            string line;
                            while ((line = await reader.ReadLineAsync()) != null)
                            {
                                throttle.Invoke(() => progressIndicator.Report(new ProgressItem((uint)((double)fileStream.Position / fileStream.Length * 100), string.Format("Checking {0} entries against password files...", entryHashes.Count))));
                                foreach (var entryHashTuple in entryHashes)
                                {
                                    if (string.Equals(line, entryHashTuple.Item2, StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (!allBreaches.ContainsKey(entryHashTuple.Item1.Uuid))
                                        {
                                            var item = new HaveIBeenPwnedPasswordEntry(entryHashTuple.Item1.Strings.ReadSafe(PwDefs.UserNameField), entryHashTuple.Item1.GetUrlDomain(), entryHashTuple.Item1);
                                            allBreaches.Add(entryHashTuple.Item1.Uuid, item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return allBreaches.Values.ToList();
        }

        private class Throttle
        {
            private DateTime lastRun;
            private readonly TimeSpan timeout;

            public Throttle(TimeSpan timeout)
            {
                this.timeout = timeout;
            }

            public void Invoke(Action action)
            {
                if (DateTime.Now - lastRun < timeout) return;
                action?.Invoke();
                lastRun = DateTime.Now;
            }
        }
    }
}

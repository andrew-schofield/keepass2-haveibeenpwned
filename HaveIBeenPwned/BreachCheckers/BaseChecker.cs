using System;
using KeePassLib;
using System.Net.Http;
using KeePass.Plugins;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HaveIBeenPwned.BreachCheckers
{
    public abstract class BaseChecker
    {
        private PwDatabase passwordDatabase;
        protected HttpClient client;
        protected IPluginHost pluginHost;

        protected BaseChecker(HttpClient httpClient, IPluginHost pluginHost)
        {
            passwordDatabase = pluginHost.Database;
            client = httpClient;
            this.pluginHost = pluginHost;
        }

        public async Task<List<BreachedEntry>> CheckDatabase(bool expireEntries, bool oldEntriesOnly, bool ignoreDeleted, IProgress<ProgressItem> progressIndicator)
        {
            return await CheckGroup(passwordDatabase.RootGroup, expireEntries, oldEntriesOnly, ignoreDeleted, progressIndicator);
        }

        public abstract Task<List<BreachedEntry>> CheckGroup(PwGroup group, bool expireEntries, bool oldEntriesOnly, bool ignoreDeleted, IProgress<ProgressItem> progressIndicator);

        public abstract Image BreachLogo { get; }

        public abstract string BreachTitle { get; }

        protected void ExpireEntry(PwEntry entry)
        {
            entry.Expires = true;
            entry.ExpiryTime = DateTime.Now;
        }
    }
}

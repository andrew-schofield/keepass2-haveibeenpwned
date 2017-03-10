using System;
using KeePassLib;
using System.Net.Http;
using KeePass.Plugins;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HaveIBeenPwned
{
    public abstract class BaseChecker
    {
        protected PwDatabase passwordDatabase;
        protected HttpClient client;
        protected IPluginHost pluginHost;

        protected BaseChecker(PwDatabase database, HttpClient httpClient, IPluginHost pluginHost)
        {
            passwordDatabase = database;
            client = httpClient;
            this.pluginHost = pluginHost;
        }

        public abstract Task<List<BreachedEntry>> CheckDatabase(bool expireEntries, bool oldEntriesOnly);

        public abstract Image BreachLogo { get; }

        public abstract string BreachTitle { get; }

        protected void ExpireEntry(PwEntry entry)
        {
            entry.Expires = true;
            entry.ExpiryTime = DateTime.Now;
        }
    }
}

using System;
using KeePassLib;
using System.Net.Http;
using KeePass.Plugins;

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

        public abstract void CheckDatabase(bool expireEntries, bool oldEntriesOnly);

        protected void ExpireEntry(PwEntry entry)
        {
            entry.Expires = true;
            entry.ExpiryTime = DateTime.Now;
        }
    }
}

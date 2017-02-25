using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib;
using System.Net.Http;

namespace HaveIBeenPwned
{
    public abstract class BaseChecker
    {
        protected PwDatabase passwordDatabase;
        protected HttpClient client;

        protected BaseChecker(PwDatabase database, HttpClient httpClient)
        {
            passwordDatabase = database;
            client = httpClient;
        }

        public abstract void CheckDatabase(bool expireEntries, bool oldEntriesOnly);

        protected void ExpireEntry(PwEntry entry)
        {
            entry.Expires = true;
            entry.ExpiryTime = DateTime.Now;
        }
    }
}

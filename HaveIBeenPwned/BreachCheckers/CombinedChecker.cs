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

namespace HaveIBeenPwned.BreachCheckers
{
    public class CombinedChecker : BaseChecker
    {
        BaseChecker[] checkers;
           
        public CombinedChecker(HttpClient httpClient, IPluginHost pluginHost)
            : base(httpClient, pluginHost)
        {
            checkers = new BaseChecker[]
            {
                new CloudbleedSite.CloudbleedSiteChecker(httpClient, pluginHost),
                new HaveIBeenPwnedSite.HaveIBeenPwnedSiteChecker(httpClient, pluginHost),
                new HaveIBeenPwnedPassword.HaveIBeenPwnedPasswordChecker(httpClient, pluginHost),
                new HaveIBeenPwnedUsername.HaveIBeenPwnedUsernameChecker(httpClient, pluginHost)
            };
        }

        public override Image BreachLogo
        {
            get { return Resources.hibp.ToBitmap(); }
        }

        public override string BreachTitle
        {
            get { return "Combined Checker"; }
        }

        public async override Task<List<BreachedEntry>> CheckGroup(PwGroup group, bool expireEntries, bool oldEntriesOnly, bool ignoreDeleted, bool ignoreExpired, IProgress<ProgressItem> progressIndicator, Func<bool> canContinue)
        {
            var results = new List<BreachedEntry>();

            foreach (var checker in checkers)
            {
                if (!canContinue())
                {
                    break;
                }

                var r = await checker.CheckGroup(group, expireEntries, oldEntriesOnly, ignoreDeleted, ignoreExpired, progressIndicator, canContinue);
                results.AddRange(r);
            }
            
            return results;
        }
    }
}

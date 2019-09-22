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
using System.Net;
using System.Text.RegularExpressions;
using KeePassExtensions;
using System.Web;
using KeePassLib.Collections;

namespace HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedUsername
{
    public class HaveIBeenPwnedUsernameChecker : BaseChecker
    {
        // Number of attempts to retrieve the breaches/pastes API when the rate limit is hit repeatedly
        private const int DefaultRetries = 5;

        public HaveIBeenPwnedUsernameChecker(HttpClient httpClient, IPluginHost pluginHost)
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

        public async override Task<List<BreachedEntry>> CheckGroup(PwGroup group, bool expireEntries,
            bool oldEntriesOnly, bool ignoreDeleted, bool ignoreExpired, IProgress<ProgressItem> progressIndicator,
            Func<bool> canContinue)
        {
            progressIndicator.Report(new ProgressItem(0, "Searching for HaveIBeenPwned Api Key..."));
            try
            {
                var apiKey = this.RetrieveApiKey();
                this.client.DefaultRequestHeaders.Add("hibp-api-key", apiKey);
            }
            catch (ApiKeyException e)
            {
                MessageBox.Show(e.Message, Resources.MessageTitle, MessageBoxButtons.OK);
                return Enumerable.Empty<BreachedEntry>().ToList();
            }

            progressIndicator.Report(new ProgressItem(0, "Getting HaveIBeenPwned breach list..."));
            var entries = group.GetEntries(true)
                .Where(e => (!ignoreDeleted || !e.IsDeleted(pluginHost)) && (!ignoreExpired || !e.Expires)).ToArray();
            var usernames = entries.Select(e => e.Strings.ReadSafe(PwDefs.UserNameField).Trim().ToLower()).Distinct();
            var breaches = await this.GetBreaches(progressIndicator, usernames, canContinue);
            var breachedEntries = new List<BreachedEntry>();

            await Task.Run(() =>
            {
                foreach (var breachGrp in breaches.GroupBy(x => x.Username))
                {
                    var username = breachGrp.Key;
                    var oldestUpdate = entries.Min(e => e.GetPasswordLastModified());

                    foreach (var breach in breachGrp)
                    {
                        if (oldEntriesOnly && oldestUpdate >= breach.BreachDate)
                        {
                            continue;
                        }

                        var pwEntry =
                            string.IsNullOrWhiteSpace(breach.Domain)
                                ? null
                                : entries.FirstOrDefault(e =>
                                    e.GetUrlDomain() == breach.Domain && breach.Username ==
                                    e.Strings.ReadSafe(PwDefs.UserNameField).Trim().ToLower());
                        if (pwEntry != null)
                        {
                            var lastModified = pwEntry.GetPasswordLastModified();
                            if (oldEntriesOnly && lastModified >= breach.BreachDate)
                            {
                                continue;
                            }

                            if (expireEntries)
                            {
                                ExpireEntry(pwEntry);
                            }
                        }

                        breachedEntries.Add(new BreachedEntry(pwEntry, breach));
                    }
                }
            });

            return breachedEntries;
        }

        private async Task<List<HaveIBeenPwnedUsernameEntry>> GetBreaches(IProgress<ProgressItem> progressIndicator,
            IEnumerable<string> usernames, Func<bool> canContinue)
        {
            List<HaveIBeenPwnedUsernameEntry> allBreaches = new List<HaveIBeenPwnedUsernameEntry>();
            var filteredUsernames = usernames.Where(u => !string.IsNullOrWhiteSpace(u) && !u.StartsWith("{REF:"));
            int counter = 0;
            foreach (var username in filteredUsernames)
            {
                if (!canContinue())
                {
                    break;
                }

                counter++;
                progressIndicator.Report(new ProgressItem((uint) ((double) counter / filteredUsernames.Count() * 100),
                    string.Format("Checking \"{0}\" for breaches", username)));
                try
                {            
                    var breaches = await GetBreachesForUserName(HttpUtility.UrlEncode(username), DefaultRetries);
                    if (breaches != null)
                    {
                        allBreaches.AddRange(breaches);
                    }
                }
                catch (HaveIBeenPwnedAbortException)
                {
                    // error was already reported to user.
                    break;
                }

                // hibp has a rate limit of 1500ms
                await Task.Delay(1600);
            }


            return allBreaches;
        }

        private async Task<IEnumerable<HaveIBeenPwnedUsernameEntry>> GetBreachesForUserName(string username,
            int remainingRetries)
        {
            IEnumerable<HaveIBeenPwnedUsernameEntry> breaches = Enumerable.Empty<HaveIBeenPwnedUsernameEntry>();
            HttpResponseMessage response = null;
            try
            {
                response = await client.GetAsync(
                    new Uri("https://haveibeenpwned.com/api/v3/breachedaccount/" + username));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                breaches = JsonConvert.DeserializeObject<List<HaveIBeenPwnedUsernameEntry>>(jsonString);
                foreach (var b in breaches)
                {
                    b.Username = username;
                }
            }
            else if ((int) response.StatusCode == 429) // The Rate limit of our API Key was exceeded
            {
                var whenToRetry = response.Headers.RetryAfter.Delta;

                if (whenToRetry.HasValue == false)
                {
                    MessageBox.Show(
                        "The Rate limit for haveibeenpwned.com was exceeded.\n" +
                        "Unfortunately there was no hint when to retry.\n" +
                        "Please try the website manually.",
                        Resources.MessageTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    throw new HaveIBeenPwnedAbortException("Rate limit exceeded, missing retry-after header.");
                }
                else if (remainingRetries > 0)
                {
                    await Task.Delay(whenToRetry.Value.Add(TimeSpan.FromMilliseconds(100)));
                    return await this.GetBreachesForUserName(username, remainingRetries--);
                }
                else
                {
                    // give up to respect the rate limit after several failed attempts, as there is most likely running another process trying to check the API
                    // the specification does not detail what is considered "consistently exceeded" so I hope 5 tries is a reasonable amount
                    MessageBox.Show(
                        "The rate limit for haveibeenpwned.com has been exceeded five times in a row.\n" +
                        "Please make sure there is no other process querying the API with your api key.",
                        Resources.MessageTitle,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    throw new HaveIBeenPwnedAbortException("Rate limit exceeded five times.");
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Further Requests are useless, as we will always get the Unauthorized Return code.
                DialogResult dialogResult = MessageBox.Show(
                    "Unable to check haveibeenpwned.com. Your api key is invalid.",
                    Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                throw new HaveIBeenPwnedAbortException("Invalid Api Key");
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                DialogResult dialogButton = MessageBox.Show(
                    string.Format("Unable to check haveibeenpwned.com (returned Status: {0})", response.StatusCode),
                    Resources.MessageTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                if (dialogButton == DialogResult.Cancel)
                {
                    throw new HaveIBeenPwnedAbortException(
                        string.Format("Unable to check haveibeenpwned.com (returned Status: {0})",
                            response.StatusCode));
                }
            }

            return breaches;
        }

        private string RetrieveApiKey()
        {
            const string regex = "^(hibp-?|haveibeenpwned)apikey$";
            var candidates = new PwObjectList<PwEntry>();
            var searchParameters = new SearchParameters()
            {
                SearchInTitles = true,
                SearchString = "apikey",
            };
            this.pluginHost.Database.RootGroup.SearchEntries(searchParameters, candidates);

            var apiKeys = candidates.Where(x => Regex.IsMatch(x.Strings.ReadSafe(PwDefs.TitleField), regex, RegexOptions.IgnoreCase));

            if (apiKeys.Count() > 1)
            {
                throw new ApiKeyException(string.Format("Found more than one api key matching the pattern: {0}", regex));
            }
            else if (apiKeys.Any() == false)
            {
                throw new ApiKeyException("No API Key found. Please create an entry named \"hibp-apikey\" in your database\n" +
                                          "and set its password to the API key obtained from https://haveibeenpwned.com/API/Key");
            }
            else
            {
                var key = apiKeys.Single();
                return key.Strings.ReadSafe(PwDefs.PasswordField);
            }
        }
    }
}
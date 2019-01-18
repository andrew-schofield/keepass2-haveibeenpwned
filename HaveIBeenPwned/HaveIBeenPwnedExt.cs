using System.Windows.Forms;
using KeePass.Plugins;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using KeePass.Forms;
using HaveIBeenPwned.BreachCheckers;
using HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedSite;
using HaveIBeenPwned.BreachCheckers.CloudbleedSite;
using HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedUsername;
using HaveIBeenPwned.BreachCheckers.HaveIBeenPwnedPassword;
using HaveIBeenPwned.UI;
using KeePassLib;
using System.Text;

namespace HaveIBeenPwned
{
    public sealed class HaveIBeenPwnedExt : Plugin
    {
        private IPluginHost pluginHost = null;
        private ToolStripSeparator toolStripSeperator = null;
        private ToolStripMenuItem haveIBeenPwnedMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedServiceMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedUsernameMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedPasswordMenuItem = null;
        private static HttpClient client;
        private StatusProgressForm progressForm;

        private Dictionary<BreachEnum, Func<HttpClient, IPluginHost, BaseChecker>> supportedBreachCheckers =
            new Dictionary<BreachEnum, Func<HttpClient, IPluginHost, BaseChecker>>
        {
            { BreachEnum.HIBPSite, (h,p) => new HaveIBeenPwnedSiteChecker(h, p) },
            { BreachEnum.CloudBleedSite, (h,p) => new CloudbleedSiteChecker(h, p) },
            { BreachEnum.HIBPUsername, (h, p) => new HaveIBeenPwnedUsernameChecker(h, p) },
            { BreachEnum.HIBPPassword, (h, p) => new HaveIBeenPwnedPasswordChecker(h, p) }
        };

        public HaveIBeenPwnedExt()
        {
            // proxy (updated only on app restart)
            var proxyType = KeePass.Program.Config.Integration.ProxyType;
            var proxyAddress = KeePass.Program.Config.Integration.ProxyAddress;
            var proxyPort = KeePass.Program.Config.Integration.ProxyPort;
            var proxyAuth = KeePass.Program.Config.Integration.ProxyAuthType;
            var proxyUsername = KeePass.Program.Config.Integration.ProxyUserName;
            var proxyPassword = KeePass.Program.Config.Integration.ProxyPassword;

            if (proxyType == ProxyServerType.Manual) {
                HttpClientHandler handler;
                if (proxyAuth == ProxyAuthType.Manual)
                {
                    handler = new HttpClientHandler()
                    {
                        Proxy = new WebProxy(String.Format("{0}:{1}", proxyAddress, proxyPort), false, null, new NetworkCredential(proxyUsername, proxyPassword)),
                        UseProxy = true,
                    };
                }
                else
                {
                    handler = new HttpClientHandler()
                    {
                        Proxy = new WebProxy(String.Format("{0}:{1}", proxyAddress, proxyPort)),
                        UseProxy = true,
                    };
                }
                client = new HttpClient(handler);
            }
            else
            {
                client = new HttpClient();
            }

            // we need to force the security protocol to use Tls first, as HIBP only accepts this as a valid secure protocol
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("KeePass HIBP Checker/{0}", Application.ProductVersion));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(0, 30, 0);
        }

        public override bool Initialize(IPluginHost host)
        {
            pluginHost = host;

            // Get a reference to the 'Tools' menu item container
            ToolStripItemCollection tsMenu = pluginHost.MainWindow.ToolsMenu.DropDownItems;

            // Add a separator at the bottom
            toolStripSeperator = new ToolStripSeparator();
            tsMenu.Add(toolStripSeperator);

            // Add menu item 'Have I Been Pwned?'
            haveIBeenPwnedMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedMenuItem.Text = "Have I Been Pwned?";
            haveIBeenPwnedMenuItem.Image = Resources.hibp.ToBitmap();

            haveIBeenPwnedServiceMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedServiceMenuItem.Text = "Check for breaches based on site/service";
            haveIBeenPwnedServiceMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedServiceMenuItem.Click += this.CheckHaveIBeenPwnedSites;
            haveIBeenPwnedMenuItem.DropDown.Items.Add(haveIBeenPwnedServiceMenuItem);

            haveIBeenPwnedUsernameMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedUsernameMenuItem.Text = "Check for breaches based on username";
            haveIBeenPwnedUsernameMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedUsernameMenuItem.Click += this.CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedMenuItem.DropDown.Items.Add(haveIBeenPwnedUsernameMenuItem);

            haveIBeenPwnedPasswordMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedPasswordMenuItem.Text = "Check for breaches based on password";
            haveIBeenPwnedPasswordMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedPasswordMenuItem.Click += this.CheckHaveIBeenPwnedPasswords;
            haveIBeenPwnedMenuItem.DropDown.Items.Add(haveIBeenPwnedPasswordMenuItem);

            tsMenu.Add(haveIBeenPwnedMenuItem);

            return true;
        }

        public override void Terminate()
        {
            // Remove all of our menu items
            ToolStripItemCollection tsMenu = pluginHost.MainWindow.ToolsMenu.DropDownItems;
            haveIBeenPwnedServiceMenuItem.Click -= this.CheckHaveIBeenPwnedSites;
            haveIBeenPwnedUsernameMenuItem.Click -= this.CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedPasswordMenuItem.Click -= this.CheckHaveIBeenPwnedPasswords;
            haveIBeenPwnedMenuItem.DropDown.Items.Remove(haveIBeenPwnedServiceMenuItem);
            haveIBeenPwnedMenuItem.DropDown.Items.Remove(haveIBeenPwnedUsernameMenuItem);
            haveIBeenPwnedMenuItem.DropDown.Items.Remove(haveIBeenPwnedPasswordMenuItem);
            tsMenu.Remove(haveIBeenPwnedMenuItem);
            tsMenu.Remove(toolStripSeperator);
        }

        public override string UpdateUrl
        {
            get
            {
                return "https://raw.githubusercontent.com/andrew-schofield/keepass2-haveibeenpwned/master/VERSION";
            }
        }

        public override Image SmallIcon
        {
            get
            {
                return Resources.hibp.ToBitmap();
            }
        }

        private void ReportProgress(ProgressItem progress)
        {
            if (progressForm != null && !progressForm.IsDisposed)
            {
                var progressHelper = (ProgressHelper)progressForm.Tag;
                var currentProgress = ((100f / progressHelper.TotalBreaches) * progressHelper.CurrentBreach) + (progress.Progress / progressHelper.TotalBreaches);
                progressForm.SetProgress((uint)currentProgress);
                progressForm.SetText(progress.ProgressText, KeePassLib.Interfaces.LogStatusType.Info);
            }
        }

        private async void CheckHaveIBeenPwnedSites(object sender, EventArgs e)
        {
            await CheckBreach(CheckTypeEnum.SiteDomain);
        }

        private async void CheckHaveIBeenPwnedUsernames(object sender, EventArgs e)
        {
            await CheckBreach(CheckTypeEnum.Username);
        }

        private async void CheckHaveIBeenPwnedPasswords(object sender, EventArgs e)
        {
            await CheckBreach(CheckTypeEnum.Password);
        }

        private async Task CheckBreach(CheckTypeEnum breachType)
        {
            if (!pluginHost.Database.IsOpen)
            {
                MessageBox.Show("You must first open a database", Resources.MessageTitle);
                return;
            }

            var dialog = new CheckerPrompt(breachType, breachType != CheckTypeEnum.Password);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                progressForm = new StatusProgressForm();
                var progressIndicator = new Progress<ProgressItem>(ReportProgress);
                progressForm.InitEx("Checking Breaches", false, breachType == CheckTypeEnum.SiteDomain, pluginHost.MainWindow);
                progressForm.Show();
                progressForm.SetProgress(0);
                List<BreachedEntry> result = new List<BreachedEntry>();
                if (dialog.CheckAllBreaches)
                {
                    progressForm.Tag = new ProgressHelper(Enum.GetValues(typeof(BreachEnum)).Length);
                    foreach (var breach in Enum.GetValues(typeof(BreachEnum)))
                    {
                        var foundBreaches = await CheckBreaches(supportedBreachCheckers[(BreachEnum)breach](client, pluginHost),
                        dialog.ExpireEntries, dialog.OnlyCheckOldEntries, dialog.IgnoreDeletedEntries, progressIndicator);
                        result.AddRange(foundBreaches);
                        ((ProgressHelper)progressForm.Tag).CurrentBreach++;
                    }
                }
                else
                {
                    progressForm.Tag = new ProgressHelper(1);
                    var foundBreaches = await CheckBreaches(supportedBreachCheckers[dialog.SelectedBreach](client, pluginHost),
                        dialog.ExpireEntries, dialog.OnlyCheckOldEntries, dialog.IgnoreDeletedEntries, progressIndicator);
                    result.AddRange(foundBreaches);
                }
                progressForm.Close();

                if (!result.Any())
                {
                    MessageBox.Show("No breached entries found.", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var breachedEntriesDialog = new BreachedEntriesDialog(pluginHost);
                    breachedEntriesDialog.AddBreaches(result);
                    breachedEntriesDialog.ShowDialog();
                }
            }

            pluginHost.MainWindow.Show();
        }

        private async Task<IList<BreachedEntry>> CheckBreaches(
            BaseChecker breachChecker,
            bool expireEntries,
            bool oldEntriesOnly,
            bool ignoreDeleted,
            IProgress<ProgressItem> progressIndicator)
        {
           return await breachChecker.CheckDatabase(expireEntries, oldEntriesOnly, ignoreDeleted, progressIndicator);
        }
    }
}

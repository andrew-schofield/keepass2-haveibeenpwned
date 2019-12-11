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
using KeePassExtensions;

namespace HaveIBeenPwned
{
    public sealed class HaveIBeenPwnedExt : Plugin
    {
        private IPluginHost pluginHost = null;

        private ToolStripSeparator toolStripSeperatorGlobal = null;
        private ToolStripMenuItem haveIBeenPwnedGlobalMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedGlobalServiceMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedGlobalUsernameMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedGlobalPasswordMenuItem = null;
        private ToolStripMenuItem checkAllGlobalMenuItem = null;
        private ToolStripMenuItem clearIgnoreListMenuItem = null;

        private ToolStripSeparator toolStripSeperatorGroup = null;
        private ToolStripMenuItem haveIBeenPwnedGroupMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedGroupServiceMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedGroupUsernameMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedGroupPasswordMenuItem = null;
        private ToolStripMenuItem checkAllGroupMenuItem = null;

        private ToolStripSeparator toolStripSeperatorEntry = null;
        private ToolStripMenuItem haveIBeenPwnedEntryMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedEntryServiceMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedEntryUsernameMenuItem = null;
        private ToolStripMenuItem haveIBeenPwnedEntryPasswordMenuItem = null;
        private ToolStripMenuItem checkAllEntryMenuItem = null;

        private static HttpClient client;
        private StatusProgressForm progressForm;

        private Dictionary<BreachEnum, Func<HttpClient, IPluginHost, BaseChecker>> supportedBreachCheckers =
            new Dictionary<BreachEnum, Func<HttpClient, IPluginHost, BaseChecker>>
        {
            { BreachEnum.HIBPSite, (h,p) => new HaveIBeenPwnedSiteChecker(h, p) },
            { BreachEnum.CloudBleedSite, (h,p) => new CloudbleedSiteChecker(h, p) },
            { BreachEnum.HIBPUsername, (h, p) => new HaveIBeenPwnedUsernameChecker(h, p) },
            { BreachEnum.HIBPPassword, (h, p) => new HaveIBeenPwnedPasswordChecker(h, p) },
            { BreachEnum.CheckAll, (h, p) => new CombinedChecker(h, p) }
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.UserAgent.ParseAdd(string.Format("KeePass HIBP Checker/{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(0, 30, 0);
        }

        public override bool Initialize(IPluginHost host)
        {
            pluginHost = host;

            // Get a reference to the 'Tools' menu item container
            ToolStripItemCollection tsMenu = pluginHost.MainWindow.ToolsMenu.DropDownItems;
            var groupContextMenu = pluginHost.MainWindow.GroupContextMenu.Items;
            var entryContextMenu = pluginHost.MainWindow.EntryContextMenu.Items;

            // Add a separator at the bottom
            toolStripSeperatorGlobal = new ToolStripSeparator();
            tsMenu.Add(toolStripSeperatorGlobal);
            toolStripSeperatorGroup = new ToolStripSeparator();
            groupContextMenu.Add(toolStripSeperatorGroup);
            toolStripSeperatorEntry = new ToolStripSeparator();
            entryContextMenu.Add(toolStripSeperatorEntry);

            // Add global menu item 'Have I Been Pwned?' for all database entries
            haveIBeenPwnedGlobalMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGlobalMenuItem.Text = Resources.MenuTitle;
            haveIBeenPwnedGlobalMenuItem.Image = Resources.hibp.ToBitmap();

            haveIBeenPwnedGlobalServiceMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGlobalServiceMenuItem.Text = Resources.MenuItemSiteTitle;
            haveIBeenPwnedGlobalServiceMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedGlobalServiceMenuItem.Click += this.Database_CheckHaveIBeenPwnedSites;
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Add(haveIBeenPwnedGlobalServiceMenuItem);

            haveIBeenPwnedGlobalUsernameMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGlobalUsernameMenuItem.Text = Resources.MenuItemUsernameTitle;
            haveIBeenPwnedGlobalUsernameMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedGlobalUsernameMenuItem.Click += this.Database_CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Add(haveIBeenPwnedGlobalUsernameMenuItem);

            haveIBeenPwnedGlobalPasswordMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGlobalPasswordMenuItem.Text = Resources.MenuItemPasswordTitle;
            haveIBeenPwnedGlobalPasswordMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedGlobalPasswordMenuItem.Click += this.Database_CheckHaveIBeenPwnedPasswords;
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Add(haveIBeenPwnedGlobalPasswordMenuItem);

            checkAllGlobalMenuItem = new ToolStripMenuItem();
            checkAllGlobalMenuItem.Text = Resources.MenuItemCheckAllTitle;
            checkAllGlobalMenuItem.Image = Resources.hibp.ToBitmap();
            checkAllGlobalMenuItem.Click += this.Database_CheckAll;
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Add(checkAllGlobalMenuItem);

            clearIgnoreListMenuItem = new ToolStripMenuItem();
            clearIgnoreListMenuItem.Text = Resources.MenuItemClearIgnoreListTitle;
            clearIgnoreListMenuItem.Image = Resources.hibp.ToBitmap();
            clearIgnoreListMenuItem.Click += this.Database_ClearIgnoreList;
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Add(clearIgnoreListMenuItem);

            tsMenu.Add(haveIBeenPwnedGlobalMenuItem);

            // Add group context menu item for the selected group
            haveIBeenPwnedGroupMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGroupMenuItem.Text = Resources.MenuTitle;
            haveIBeenPwnedGroupMenuItem.Image = Resources.hibp.ToBitmap();

            haveIBeenPwnedGroupServiceMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGroupServiceMenuItem.Text = Resources.MenuItemSiteTitle;
            haveIBeenPwnedGroupServiceMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedGroupServiceMenuItem.Click += this.Group_CheckHaveIBeenPwnedSites;
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Add(haveIBeenPwnedGroupServiceMenuItem);

            haveIBeenPwnedGroupUsernameMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGroupUsernameMenuItem.Text = Resources.MenuItemUsernameTitle;
            haveIBeenPwnedGroupUsernameMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedGroupUsernameMenuItem.Click += this.Group_CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Add(haveIBeenPwnedGroupUsernameMenuItem);

            haveIBeenPwnedGroupPasswordMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedGroupPasswordMenuItem.Text = Resources.MenuItemPasswordTitle;
            haveIBeenPwnedGroupPasswordMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedGroupPasswordMenuItem.Click += this.Group_CheckHaveIBeenPwnedPasswords;
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Add(haveIBeenPwnedGroupPasswordMenuItem);
            
            checkAllGroupMenuItem = new ToolStripMenuItem();
            checkAllGroupMenuItem.Text = Resources.MenuItemCheckAllTitle;
            checkAllGroupMenuItem.Image = Resources.hibp.ToBitmap();
            checkAllGroupMenuItem.Click += this.Group_CheckAll;
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Add(checkAllGroupMenuItem);

            groupContextMenu.Add(haveIBeenPwnedGroupMenuItem);

            // Add entry context menu item for the selected entries
            haveIBeenPwnedEntryMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedEntryMenuItem.Text = Resources.MenuTitle;
            haveIBeenPwnedEntryMenuItem.Image = Resources.hibp.ToBitmap();

            haveIBeenPwnedEntryServiceMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedEntryServiceMenuItem.Text = Resources.MenuItemSiteTitle;
            haveIBeenPwnedEntryServiceMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedEntryServiceMenuItem.Click += this.Entries_CheckHaveIBeenPwnedSites;
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Add(haveIBeenPwnedEntryServiceMenuItem);

            haveIBeenPwnedEntryUsernameMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedEntryUsernameMenuItem.Text = Resources.MenuItemUsernameTitle;
            haveIBeenPwnedEntryUsernameMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedEntryUsernameMenuItem.Click += this.Entries_CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Add(haveIBeenPwnedEntryUsernameMenuItem);

            haveIBeenPwnedEntryPasswordMenuItem = new ToolStripMenuItem();
            haveIBeenPwnedEntryPasswordMenuItem.Text = Resources.MenuItemPasswordTitle;
            haveIBeenPwnedEntryPasswordMenuItem.Image = Resources.hibp.ToBitmap();
            haveIBeenPwnedEntryPasswordMenuItem.Click += this.Entries_CheckHaveIBeenPwnedPasswords;
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Add(haveIBeenPwnedEntryPasswordMenuItem);

            checkAllEntryMenuItem = new ToolStripMenuItem();
            checkAllEntryMenuItem.Text = Resources.MenuItemCheckAllTitle;
            checkAllEntryMenuItem.Image = Resources.hibp.ToBitmap();
            checkAllEntryMenuItem.Click += this.Entries_CheckAll;
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Add(checkAllEntryMenuItem);

            entryContextMenu.Add(haveIBeenPwnedEntryMenuItem);

            return true;
        }

        public override void Terminate()
        {
            // Remove all of our menu items
            ToolStripItemCollection tsMenu = pluginHost.MainWindow.ToolsMenu.DropDownItems;
            haveIBeenPwnedGlobalServiceMenuItem.Click -= this.Database_CheckHaveIBeenPwnedSites;
            haveIBeenPwnedGlobalUsernameMenuItem.Click -= this.Database_CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedGlobalPasswordMenuItem.Click -= this.Database_CheckHaveIBeenPwnedPasswords;
            checkAllGlobalMenuItem.Click -= this.Database_CheckAll;
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Remove(haveIBeenPwnedGlobalServiceMenuItem);
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Remove(haveIBeenPwnedGlobalUsernameMenuItem);
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Remove(haveIBeenPwnedGlobalPasswordMenuItem);
            haveIBeenPwnedGlobalMenuItem.DropDown.Items.Remove(checkAllGlobalMenuItem);
            tsMenu.Remove(haveIBeenPwnedGlobalMenuItem);
            tsMenu.Remove(toolStripSeperatorGlobal);

            var groupContextMenu = pluginHost.MainWindow.GroupContextMenu.Items;
            haveIBeenPwnedGroupServiceMenuItem.Click -= this.Group_CheckHaveIBeenPwnedSites;
            haveIBeenPwnedGroupUsernameMenuItem.Click -= this.Group_CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedGroupPasswordMenuItem.Click -= this.Group_CheckHaveIBeenPwnedPasswords;
            checkAllGroupMenuItem.Click -= this.Group_CheckAll;
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Remove(haveIBeenPwnedGroupServiceMenuItem);
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Remove(haveIBeenPwnedGroupUsernameMenuItem);
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Remove(haveIBeenPwnedGroupPasswordMenuItem);
            haveIBeenPwnedGroupMenuItem.DropDown.Items.Remove(checkAllGroupMenuItem);
            groupContextMenu.Remove(haveIBeenPwnedGroupMenuItem);
            groupContextMenu.Remove(toolStripSeperatorGroup);

            var entryContextMenu = pluginHost.MainWindow.EntryContextMenu.Items;
            haveIBeenPwnedEntryServiceMenuItem.Click -= this.Entries_CheckHaveIBeenPwnedSites;
            haveIBeenPwnedEntryUsernameMenuItem.Click -= this.Entries_CheckHaveIBeenPwnedUsernames;
            haveIBeenPwnedEntryPasswordMenuItem.Click -= this.Entries_CheckHaveIBeenPwnedPasswords;
            checkAllEntryMenuItem.Click -= this.Entries_CheckAll;
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Remove(haveIBeenPwnedEntryServiceMenuItem);
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Remove(haveIBeenPwnedEntryUsernameMenuItem);
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Remove(haveIBeenPwnedEntryPasswordMenuItem);
            haveIBeenPwnedEntryMenuItem.DropDown.Items.Remove(checkAllEntryMenuItem);
            entryContextMenu.Remove(haveIBeenPwnedEntryMenuItem);
            entryContextMenu.Remove(toolStripSeperatorEntry);
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

        private async void Database_CheckHaveIBeenPwnedSites(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.SiteDomain, pluginHost.Database.RootGroup);
        }

        private async void Database_CheckHaveIBeenPwnedUsernames(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.Username, pluginHost.Database.RootGroup);
        }

        private async void Database_CheckHaveIBeenPwnedPasswords(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.Password, pluginHost.Database.RootGroup);
        }

        private async void Database_CheckAll(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.CheckAll, pluginHost.Database.RootGroup);
        }

        private  void Database_ClearIgnoreList(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;

            foreach (var key in pluginHost.Database.RootGroup.CustomData.Select(x => x.Key).Where(x => x.StartsWith("HaveIBeenPwned:")).ToArray())
            {
                pluginHost.Database.RootGroup.CustomData.Remove(key);
            }

            foreach (var entry in pluginHost.Database.RootGroup.GetEntries(true))
            {
                foreach (var key in entry.CustomData.Select(x => x.Key).Where(x => x.StartsWith("HaveIBeenPwned:")).ToArray())
                {
                    entry.CustomData.Remove(key);
                }
            }

            pluginHost.MarkAsModified();
        }

        private async void Group_CheckHaveIBeenPwnedSites(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.SiteDomain, pluginHost.MainWindow.GetSelectedGroup());
        }

        private async void Group_CheckHaveIBeenPwnedUsernames(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.Username, pluginHost.MainWindow.GetSelectedGroup());
        }

        private async void Group_CheckHaveIBeenPwnedPasswords(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.Password, pluginHost.MainWindow.GetSelectedGroup());
        }

        private async void Group_CheckAll(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.CheckAll, pluginHost.MainWindow.GetSelectedGroup());
        }

        private async void Entries_CheckHaveIBeenPwnedSites(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.SiteDomain, pluginHost.MainWindow.GetSelectedEntriesAsGroup());
        }

        private async void Entries_CheckHaveIBeenPwnedUsernames(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.Username, pluginHost.MainWindow.GetSelectedEntriesAsGroup());
        }

        private async void Entries_CheckHaveIBeenPwnedPasswords(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.Password, pluginHost.MainWindow.GetSelectedEntriesAsGroup());
        }

        private async void Entries_CheckAll(object sender, EventArgs e)
        {
            if (!AssertDatabaseOpen()) return;
            await CheckBreach(CheckTypeEnum.CheckAll, pluginHost.MainWindow.GetSelectedEntriesAsGroup());
        }

        private UserSelectedOptions GetUserSelection(CheckTypeEnum breachType)
        {
            var dialog = new CheckerPrompt(breachType, breachType != CheckTypeEnum.Password);
            
            return
                dialog.ShowDialog() != DialogResult.OK ? null : 
                    new UserSelectedOptions()
                    {
                        CheckAllBreaches = dialog.CheckAllBreaches,
                        ExpireEntries = dialog.ExpireEntries,
                        OnlyCheckOldEntries = dialog.OnlyCheckOldEntries,
                        IgnoreDeletedEntries = dialog.IgnoreDeletedEntries,
                        IgnoreExpiredEntries = dialog.IgnoreExpiredEntries,
                        SelectedBreach = dialog.SelectedBreach
                    };
        }

        private async Task CheckBreach(CheckTypeEnum breachType, PwGroup group)
        {
            var selectedOptions = GetUserSelection(breachType);
            await CheckBreach(breachType, group, selectedOptions);
        }

        private async Task CheckBreach(CheckTypeEnum breachType, PwGroup group, UserSelectedOptions selectedOptions)
        {
            if (selectedOptions == null)
            {
                pluginHost.MainWindow.Show();
                return;
            }

            progressForm = new StatusProgressForm();
            var progressIndicator = new Progress<ProgressItem>(ReportProgress);
            progressForm.InitEx("Checking Breaches", true, breachType == CheckTypeEnum.SiteDomain, pluginHost.MainWindow);
            progressForm.Show();
            progressForm.SetProgress(0);
            List<BreachedEntry> result = new List<BreachedEntry>();
            try
            {
                if (selectedOptions.CheckAllBreaches)
                {
                    System.Windows.Forms.MessageBox.Show("IsCheckAllBreaches");
                    var breaches = Enum.GetValues(typeof(BreachEnum)).Cast<BreachEnum>().Where(b => b.GetAttribute<CheckerTypeAttribute>().Type == breachType);
                    progressForm.Tag = new ProgressHelper(breaches.Count());
                    foreach (var breach in breaches)
                    {
                        var foundBreaches = await CheckBreaches(supportedBreachCheckers[(BreachEnum)breach](client, pluginHost),
                        group, selectedOptions.ExpireEntries, selectedOptions.OnlyCheckOldEntries, selectedOptions.IgnoreDeletedEntries, selectedOptions.IgnoreExpiredEntries, progressIndicator, () => progressForm.ContinueWork());
                        result.AddRange(foundBreaches);
                        ((ProgressHelper)progressForm.Tag).CurrentBreach++;
                    }
                }
                else
                {
                    progressForm.Tag = new ProgressHelper(1);
                    var foundBreaches = await CheckBreaches(supportedBreachCheckers[selectedOptions.SelectedBreach](client, pluginHost),
                        group, selectedOptions.ExpireEntries, selectedOptions.OnlyCheckOldEntries, selectedOptions.IgnoreDeletedEntries, selectedOptions.IgnoreExpiredEntries, progressIndicator, () => progressForm.ContinueWork());
                    result.AddRange(foundBreaches);
                }
            }
            catch (Exception ex)
            {
                result = null;
                MessageBox.Show(pluginHost.MainWindow, ex.Message, Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressForm.Close();
            }

            if (result != null)
            {
                if (!result.Any())
                {
                    MessageBox.Show(pluginHost.MainWindow, "No breached entries found.", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var breachedEntriesDialog = new BreachedEntriesDialog(pluginHost);
                    breachedEntriesDialog.AddBreaches(result);
                    breachedEntriesDialog.ShowDialog(pluginHost.MainWindow);
                }
            }

            pluginHost.MainWindow.Show();
        }

        private async Task<IList<BreachedEntry>> CheckBreaches(
            BaseChecker breachChecker,
            PwGroup group,
            bool expireEntries,
            bool oldEntriesOnly,
            bool ignoreDeleted,
            bool ignoreExpired,
            IProgress<ProgressItem> progressIndicator,
            Func<bool> canContinue)
        {
           return await breachChecker.CheckGroup(group, expireEntries, oldEntriesOnly, ignoreDeleted, ignoreExpired, progressIndicator, canContinue);
        }

        private bool AssertDatabaseOpen()
        {
            if (!pluginHost.Database.IsOpen)
            {
                MessageBox.Show("You must first open a database", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private class UserSelectedOptions
        {
            public bool CheckAllBreaches { get; set; }
            public bool ExpireEntries { get; set; }
            public bool OnlyCheckOldEntries { get; set; }
            public bool IgnoreDeletedEntries { get; set; }
            public bool IgnoreExpiredEntries { get; set; }
            public BreachEnum SelectedBreach { get; set; }
        }
    }
}

using System.Windows.Forms;
using KeePass.Plugins;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Drawing;
using System.Linq;

namespace HaveIBeenPwned
{
    public sealed class HaveIBeenPwnedExt : Plugin
    {
        private IPluginHost pluginHost = null;
        private ToolStripSeparator toolStripSeperator = null;
        private ToolStripMenuItem haveIBeenPwnedMenuItem = null;
        private ToolStripMenuItem cloudBleedMenuItem = null;
        private static HttpClient client = new HttpClient();

        public HaveIBeenPwnedExt()
        {
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
            haveIBeenPwnedMenuItem.Click += this.CheckHaveIBeenPwned;
            tsMenu.Add(haveIBeenPwnedMenuItem);

            // Add menu item 'Cloudbleed Checker'
            cloudBleedMenuItem = new ToolStripMenuItem();
            cloudBleedMenuItem.Text = "Cloudbleed Checker";
            cloudBleedMenuItem.Image = Resources.cloudbleed.ToBitmap();
            cloudBleedMenuItem.Click += this.CheckCloudBleed;
            tsMenu.Add(cloudBleedMenuItem);

            return true;
        }

        public override void Terminate()
        {
            // Remove all of our menu items
            ToolStripItemCollection tsMenu = pluginHost.MainWindow.ToolsMenu.DropDownItems;
            haveIBeenPwnedMenuItem.Click -= this.CheckHaveIBeenPwned;
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

        private void CheckHaveIBeenPwned(object sender, EventArgs e)
        {
            CheckBreaches(new HaveIBeenPwnedChecker(pluginHost.Database, client, pluginHost));
        }

        private void CheckCloudBleed(object sender, EventArgs e)
        {
            CheckBreaches(new CloudbleedChecker(pluginHost.Database, client, pluginHost));
        }

        private void CheckBreaches(BaseChecker breachChecker)
        {
            if (!pluginHost.Database.IsOpen)
            {
                MessageBox.Show("You must first open a database", Resources.MessageTitle);
                return;
            }

            var dialog = new CheckerPrompt(breachChecker.BreachLogo, breachChecker.BreachTitle);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var breachedEntries = breachChecker.CheckDatabase(dialog.ExpireEntries, dialog.OnlyCheckOldEntries);
                breachedEntries.ContinueWith((result) =>
                {
                    // make sure any exceptions we aren't catching ourselves (like URIFormatException) are thrown correctly
                    if(result.IsFaulted)
                    {
                        throw result.Exception;
                    }

                    if (!result.Result.Any())
                    {
                        MessageBox.Show("No breached entries found.", Resources.MessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        var breachedEntriesDialog = new BreachedEntriesDialog(pluginHost);
                        breachedEntriesDialog.AddBreaches(result.Result);
                        breachedEntriesDialog.ShowDialog();
                    }
                });
            }

            pluginHost.MainWindow.Show();

        }
    }
}

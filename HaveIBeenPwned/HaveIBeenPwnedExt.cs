using System.Windows.Forms;
using KeePass.Plugins;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

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
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"KeePass HIBP Checker/{Application.ProductVersion}");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = new TimeSpan(0, 10, 0);
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

        private void CheckHaveIBeenPwned(object sender, EventArgs e)
        {
            if (!pluginHost.Database.IsOpen)
            {
                MessageBox.Show("You must first open a database", Resources.MessageTitle);
                return;
            }

            var dialog = new CheckerPrompt(Resources.hibp.ToBitmap(), "Have I Been Pwned");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Called when the menu item is clicked
                var haveIBeenPwnedChecker = new HaveIBeenPwnedChecker(pluginHost.Database, client, pluginHost);
                haveIBeenPwnedChecker.CheckDatabase(dialog.ExpireEntries, dialog.OnlyCheckOldEntries);
            }
        }

        private void CheckCloudBleed(object sender, EventArgs e)
        {
            if (!pluginHost.Database.IsOpen)
            {
                MessageBox.Show("You must first open a database", Resources.MessageTitle);
                return;
            }

            var dialog = new CheckerPrompt(Resources.cloudbleed.ToBitmap(), "Cloudbleed Vulnerability");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Called when the menu item is clicked
                var cloudBleedChecker = new CloudbleedChecker(pluginHost.Database, client, pluginHost);
                cloudBleedChecker.CheckDatabase(dialog.ExpireEntries, dialog.OnlyCheckOldEntries);
            }
        }
    }
}

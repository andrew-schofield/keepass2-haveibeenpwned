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
        private ToolStripMenuItem pluginMenuItem = null;
        private static HttpClient client = new HttpClient();

        public HaveIBeenPwnedExt()
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"KeePass HIBP Checker/{Application.ProductVersion}");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
            pluginMenuItem = new ToolStripMenuItem();
            pluginMenuItem.Text = "Have I Been Pwned?";
            pluginMenuItem.Image = Resources.menuIcon.ToBitmap();
            pluginMenuItem.Click += this.CheckHaveIBeenPwned;
            tsMenu.Add(pluginMenuItem);

            return true;
        }

        public override void Terminate()
        {
            // Remove all of our menu items
            ToolStripItemCollection tsMenu = pluginHost.MainWindow.ToolsMenu.DropDownItems;
            pluginMenuItem.Click -= this.CheckHaveIBeenPwned;
            tsMenu.Remove(pluginMenuItem);
            tsMenu.Remove(toolStripSeperator);
        }

        private void CheckHaveIBeenPwned(object sender, EventArgs e)
        {
            if (!pluginHost.Database.IsOpen)
            {
                MessageBox.Show("You must first open a database", Resources.MessageTitle);
                return;
            }

            var dialog = new CheckerPrompt();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Called when the menu item is clicked
                var haveIBeenPwnedChecker = new HaveIBeenPwnedChecker(pluginHost.Database, client);
                haveIBeenPwnedChecker.CheckDatabase(dialog.ExpireEntries, dialog.OnlyCheckOldEntries);
            }
        }
    }
}

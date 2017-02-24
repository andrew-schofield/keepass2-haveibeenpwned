using System.Windows.Forms;
using KeePass.Plugins;
using System;

namespace HaveIBeenPwned
{
    public sealed class HaveIBeenPwnedExt : Plugin
    {
        private IPluginHost pluginHost = null;
        private ToolStripSeparator toolStripSeperator = null;
        private ToolStripMenuItem pluginMenuItem = null;

        public override bool Initialize(IPluginHost host)
        {
            pluginHost = host;

            // Get a reference to the 'Tools' menu item container
            ToolStripItemCollection tsMenu = pluginHost.MainWindow.ToolsMenu.DropDownItems;

            // Add a separator at the bottom
            toolStripSeperator = new ToolStripSeparator();
            tsMenu.Add(toolStripSeperator);

            // Add menu item 'Do Something'
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
            // Called when the menu item is clicked
        }
    }
}

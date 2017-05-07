using System.Windows.Forms;
using KeePass.Plugins;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;

namespace HaveIBeenPwned
{
    public sealed class HaveIBeenPwnedExt : Plugin
    {
        private IPluginHost pluginHost = null;
        private ToolStripSeparator toolStripSeperator = null;
        private ToolStripMenuItem haveIBeenPwnedMenuItem = null;
        private static HttpClient client = new HttpClient();

        private Dictionary<BreachEnum, Func<KeePassLib.PwDatabase, HttpClient, IPluginHost, BaseChecker>> supportedBreachCheckers =
            new Dictionary<BreachEnum, Func<KeePassLib.PwDatabase, HttpClient, IPluginHost, BaseChecker>>
        {
            { BreachEnum.HIBP, (d,h,p) => new HaveIBeenPwnedChecker(d, h, p) },
            { BreachEnum.CloudBleed, (d,h,p) => new CloudbleedChecker(d, h, p) },
        };

        public HaveIBeenPwnedExt()
        {
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
            haveIBeenPwnedMenuItem.Click += this.CheckHaveIBeenPwned;
            tsMenu.Add(haveIBeenPwnedMenuItem);

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
            if (!pluginHost.Database.IsOpen)
            {
                MessageBox.Show("You must first open a database", Resources.MessageTitle);
                return;
            }

            var dialog = new CheckerPrompt();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                List<BreachedEntry> result = new List<BreachedEntry>();
                if(dialog.CheckAllBreaches)
                {
                    foreach(var breach in Enum.GetValues(typeof(BreachEnum)))
                    {
                        result.AddRange(CheckBreaches(supportedBreachCheckers[(BreachEnum)breach](pluginHost.Database, client, pluginHost),
                        dialog.ExpireEntries, dialog.OnlyCheckOldEntries, dialog.IgnoreDeletedEntries));
                    }
                }
                else
                {
                    result.AddRange(CheckBreaches(supportedBreachCheckers[dialog.SelectedBreach](pluginHost.Database, client, pluginHost),
                        dialog.ExpireEntries, dialog.OnlyCheckOldEntries, dialog.IgnoreDeletedEntries));
                }

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

        private IList<BreachedEntry> CheckBreaches(
            BaseChecker breachChecker, 
            bool expireEntries,
            bool oldEntriesOnly,
            bool ignoreDeleted)
        {         
            var breachedEntries = breachChecker.CheckDatabase(expireEntries, oldEntriesOnly, ignoreDeleted);
            var breaches =  breachedEntries.ContinueWith((result) =>
            {
                // make sure any exceptions we aren't catching ourselves (like URIFormatException) are thrown correctly
                if (result.IsFaulted)
                {
                    throw result.Exception;
                }

                return result.Result;
                
            }).Result;

            return breaches;
        }
    }
}

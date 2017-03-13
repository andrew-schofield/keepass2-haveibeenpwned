using KeePass.Plugins;
using KeePassLib;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HaveIBeenPwned
{
    public partial class BreachedEntriesDialog : Form
    {
        private IPluginHost pluginHost;

        public BreachedEntriesDialog(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
            InitializeComponent();
        }

        public void AddBreaches(IList<BreachedEntry> breaches)
        {
            breachedEntryList.Items.Clear();
            foreach (var breach in breaches)
            {
                var newItem = new ListViewItem(new[]
                {
                    breach.Entry.Strings.ReadSafe(PwDefs.TitleField),
                    breach.Entry.Strings.ReadSafe(PwDefs.UserNameField),
                    breach.Entry.Strings.ReadSafe(PwDefs.UrlField),
                    breach.Entry.GetPasswordLastModified().ToShortDateString(),
                    breach.BreachDate.ToShortDateString()
                })
                {
                    Tag = breach.Entry
                };
                breachedEntryList.Items.Add(newItem);
            }            
        }

        private void breachedEntryList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*if(breachedEntryList.SelectedItems != null && breachedEntryList.SelectedItems.Count == 1)
            {
                var entry = ((PwEntry)breachedEntryList.SelectedItems[0].Tag);
                var pwForm = new KeePass.Forms.PwEntryForm();
                pwForm.InitEx(entry, KeePass.Forms.PwEditMode.EditExistingEntry, pluginHost.Database, pluginHost.MainWindow.ClientIcons, false, false);
                pwForm.ShowDialog();
            }*/
        }
    }
}

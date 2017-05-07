using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HaveIBeenPwned
{
    public partial class CheckerPrompt : Form
    {
        public CheckerPrompt()
        {
            InitializeComponent();
            this.supportedBreachList.DataSource = Enum.GetValues(typeof(BreachEnum)).Cast<BreachEnum>()
                .Select(b => new ListViewItem { Text = b.GetAttribute<DisplayAttribute>().Name, Tag = b }).ToList();

            checkAllBreaches.CheckedChanged += CheckAllBreachesCheckedChanged;
        }

        private void CheckAllBreachesCheckedChanged(object sender, EventArgs e)
        {
            supportedBreachList.Enabled = !checkAllBreaches.Checked;
            checkAllBreachLabel.Enabled = !checkAllBreaches.Checked;
        }

        public bool ExpireEntries
        {
            get { return expireEntries.Checked; }
        }

        public bool OnlyCheckOldEntries
        {
            get { return checkOldEntries.Checked; }
        }

        public bool IgnoreDeletedEntries
        {
            get { return ignoreDeletedEntries.Checked; }
        }

        public BreachEnum SelectedBreach
        {
            get { return (BreachEnum)((ListViewItem)supportedBreachList.SelectedItem).Tag; }
        }

        public bool CheckAllBreaches
        {
            get { return checkAllBreaches.Checked; }
        }
    }
}

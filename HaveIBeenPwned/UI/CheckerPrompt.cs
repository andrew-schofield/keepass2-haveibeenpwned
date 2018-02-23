using KeePassExtensions;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HaveIBeenPwned.UI
{
    public partial class CheckerPrompt : Form
    {
        public CheckerPrompt(CheckTypeEnum checkType, bool enableOldEntries)
        {
            InitializeComponent();
            Text = string.Format("Have I Been Pwned? - {0}", checkType.GetAttribute<DisplayAttribute>().Name);
            this.supportedBreachList.DataSource = Enum.GetValues(typeof(BreachEnum)).Cast<BreachEnum>().Where(b => b.GetAttribute<CheckerTypeAttribute>().Type == checkType)
                .Select(b => new ListViewItem { Text = b.GetAttribute<CheckerTypeAttribute>().Name, Tag = b }).ToList();

            checkOldEntries.Enabled = enableOldEntries;
            checkOldEntries.Checked = enableOldEntries;

            checkAllBreaches.CheckedChanged += CheckAllBreachesCheckedChanged;
            supportedBreachList.SelectedIndexChanged += SupportedBreachListSelectedIndexChanged;
            SetBreachDescription();
        }

        private void SupportedBreachListSelectedIndexChanged(object sender, EventArgs e)
        {
            SetBreachDescription();
        }

        private void SetBreachDescription()
        {
            breachDescription.Text = SelectedBreach.GetAttribute<CheckerTypeAttribute>().Description;
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

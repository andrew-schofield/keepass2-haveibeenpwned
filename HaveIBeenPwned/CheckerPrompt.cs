using System.Windows.Forms;

namespace HaveIBeenPwned
{
    public partial class CheckerPrompt : Form
    {
        public CheckerPrompt()
        {
            InitializeComponent();
        }

        public bool ExpireEntries => expireEntries.Checked;

        public bool OnlyCheckOldEntries => checkOldEntries.Checked;
    }
}

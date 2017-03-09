using System.Drawing;
using System.Windows.Forms;

namespace HaveIBeenPwned
{
    public partial class CheckerPrompt : Form
    {
        public CheckerPrompt(Image checkerLogo, string checkerTitle)
        {
            InitializeComponent();
            breachCheckerLogo.Image = checkerLogo;
            breachCheckerText.Text = checkerTitle;
        }

        public bool ExpireEntries
        {
            get { return expireEntries.Checked; }
        }

        public bool OnlyCheckOldEntries
        {
            get { return checkOldEntries.Checked; }
        }
    }
}

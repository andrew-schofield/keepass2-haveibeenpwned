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

        public bool ExpireEntries => expireEntries.Checked;

        public bool OnlyCheckOldEntries => checkOldEntries.Checked;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HaveIBeenPwned
{
    public static class CloudFlareErrorProcessor
    {
        private const string KeePassHIBPPath = "KeePassHIBP";

        public static async void SendErrorMessage(HttpResponseMessage response)
        {
            var errorResponse = await response.Content.ReadAsByteArrayAsync();
            var tempPath = Path.GetTempPath();
            var uniqueFilename = Path.GetRandomFileName();
            var filePathFolder = Path.Combine(tempPath, KeePassHIBPPath);
            var filePath = Path.Combine(filePathFolder, uniqueFilename + ".txt");
            Directory.CreateDirectory(filePathFolder);
            File.WriteAllBytes(filePath, errorResponse);
            if (MessageBox.Show(string.Format("Cloudflare error message saved here: {0}. Please attach this to the following github issue: https://github.com/andrew-schofield/keepass2-haveibeenpwned/issues/56", filePath), "Error response saved", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start(filePathFolder);
            }
        }
    }
}

namespace HaveIBeenPwned
{
    public class ProgressItem
    {
        public ProgressItem(uint progress, string progressText)
        {
            Progress = progress;
            ProgressText = progressText;
        }

        public uint Progress { get; set; }

        public string ProgressText { get; set; }
    }
}

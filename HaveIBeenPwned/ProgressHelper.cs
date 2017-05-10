namespace HaveIBeenPwned
{
    public class ProgressHelper
    {
        public ProgressHelper(int totalBreaches)
        {
            TotalBreaches = totalBreaches;
            CurrentBreach = 0;
        }

        public int TotalBreaches { get; private set; }

        public int CurrentBreach { get; set; }
    }
}

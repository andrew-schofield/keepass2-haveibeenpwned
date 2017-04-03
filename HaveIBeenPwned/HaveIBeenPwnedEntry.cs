using System;

namespace HaveIBeenPwned
{
    public class HaveIBeenPwnedEntry
    public class HaveIBeenPwnedEntry : IBreach
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Domain { get; set; }
        public DateTime BreachDate { get; set; }
        public DateTime AddDate { get; set; }
        public int PwnCount { get; set; }
        public string Description { get; set; }
        public string[] DataClasses { get; set; }
        public bool IsVerified { get; set; }
        public bool IsFabricated { get; set; }
        public bool IsSensitive { get; set; }
        public bool IsRetired { get; set; }
        public bool IsSpamList { get; set; }
    }
}

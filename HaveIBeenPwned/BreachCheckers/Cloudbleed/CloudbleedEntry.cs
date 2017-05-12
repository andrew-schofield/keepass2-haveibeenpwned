using System;

namespace HaveIBeenPwned.BreachCheckers.Cloudbleed
{
    public class CloudbleedEntry : IBreach
    {
        private readonly DateTime breachDate = new DateTime(2017, 02, 17);

        public DateTime BreachDate
        {
            get
            {
                return breachDate;
            }
        }

        public string Title
        {
            get
            {
                return "Cloudbleed";
            }
        }

        public override string ToString()
        {
            return "Cloudbleed breach\r\nThis domain was included in a list of suspected sites hosted by cloudflare\r\nwhen they had a bug which leaked private data in HTTP responses.";
        }
    }
}

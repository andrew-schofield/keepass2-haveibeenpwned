using System;

namespace HaveIBeenPwned.BreachCheckers.CloudbleedSite
{
    public class CloudbleedSiteEntry : IBreach
    {
        private readonly DateTime breachDate = new DateTime(2017, 02, 17);
        private string username;
        private string domain;

        public CloudbleedSiteEntry(string username, string domain)
        {
            this.username = username;
            this.domain = domain;
        }

        public DateTime BreachDate
        {
            get
            {
                return breachDate;
            }
        }

        public string[] DataClasses
        {
            get
            {
                return new[] { "Potential Private Data Leak" };
            }
        }

        public string Description
        {
            get
            {
                return "Cloudbleed is a security bug discovered affecting Cloudflare's reverse proxies which could leak private information such as HTTP cookies, authentication tokens, HTTP POST bodies, and other sensitive data.";
            }
        }

        public string Title
        {
            get
            {
                return "Cloudbleed";
            }
        }

        public string Domain
        {
            get
            {
                return domain;
            }
        }

        public string Username
        {
            get
            {
                return username;
            }
        }

        public override string ToString()
        {
            return "Cloudbleed breach\r\nThis domain was included in a list of suspected sites hosted by cloudflare\r\nwhen they had a bug which leaked private data in HTTP responses.";
        }
    }
}

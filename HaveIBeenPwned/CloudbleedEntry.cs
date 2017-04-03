using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveIBeenPwned
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

        public string Name
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

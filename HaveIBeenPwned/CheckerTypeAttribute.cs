using System;

namespace HaveIBeenPwned
{
    internal class CheckerTypeAttribute : Attribute
    {
        public string Name { get; set; }

        public CheckTypeEnum Type { get; set; }
    }
}
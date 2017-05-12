using System;

namespace HaveIBeenPwned
{
    internal class DisplayAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
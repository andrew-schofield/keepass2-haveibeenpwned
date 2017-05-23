using KeePassLib.Security;
using KeePassLib.Utility;
using System.Linq;

namespace HaveIBeenPwned.Extensions
{
    public static class ProtectedStringExtensions
    {
        public static bool ValueEquals(this ProtectedString protectedString, ProtectedString compareWith)
        {
            // extract the unencrypted strings
            byte[] string1 = protectedString.ReadUtf8();
            byte[] string2 = compareWith.ReadUtf8();

            // compare the results
            var result = string1.SequenceEqual(string2);

            // clean up the byte arrays so we don't leak data
            MemUtil.ZeroByteArray(string1);
            MemUtil.ZeroByteArray(string2);

            // finally return the result
            return result;
        }
    }
}

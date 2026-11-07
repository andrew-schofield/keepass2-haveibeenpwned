using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeePassExtensions
{
    public static class ProtectedStringExtensions
    {
        /// <summary>
        /// Compare equality of ProtectedStrings.
        /// </summary>
        /// <param name="protectedString">Left-hand ProtectedString</param>
        /// <param name="compareWith">Right-hand ProtectedString</param>
        /// <returns></returns>
        public static bool ValueEquals(this ProtectedString protectedString, ProtectedString compareWith)
        {
            // extract the unencrypted strings
            byte[] string1 = protectedString.ReadUtf8();
            byte[] string2 = compareWith.ReadUtf8();

            // compare the results
            var result = MemUtil.ArraysEqual(string1, string2);

            // clean up the byte arrays so we don't leak data
            MemUtil.ZeroByteArray(string1);
            MemUtil.ZeroByteArray(string2);

            // finally return the result
            return result;
        }
    }
}

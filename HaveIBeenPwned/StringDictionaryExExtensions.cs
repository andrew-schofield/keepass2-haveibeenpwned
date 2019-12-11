using KeePassLib.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaveIBeenPwned
{
    public static class StringDictionaryExExtensions
    {
        public static bool Set<T>(this StringDictionaryEx dict, string fieldName, T value)
        {
            var isDefault = EqualityComparer<T>.Default.Equals(default(T), value);
            if (isDefault)
            {
                if (!dict.Any(x => x.Key == fieldName))
                {
                    return false;
                }

                dict.Remove(fieldName);
                return true;
            }

            var oldValue = dict.Get(fieldName);
            var newValue = JsonConvert.SerializeObject(value);

            if (oldValue == newValue)
            {
                return false;
            }

            dict.Set(fieldName, newValue);
            return true;
        }

        public static T Get<T>(this StringDictionaryEx dict, string fieldName)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(dict.Get(fieldName));
            }
            catch (Exception) {
                return default(T);
            }
        }
    }
}

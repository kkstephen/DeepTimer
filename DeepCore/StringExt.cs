using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeepCore
{
    public static class StringExt
    {
        public static string ToTimespan(this long t)
        {
            if (t < 0) t = 0;

            DateTime dt = new DateTime(t);

            return dt.ToString("mm:ss.fff");
        }

        public static string Serialize<T>(this T instance) where T : class
        {
            return JsonConvert.SerializeObject(instance);
        }

        public static T Deserialize<T>(this string str) where T : class
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            foreach (T item in source)
            {
                destination.Add(item);
            }
        }
    }
}

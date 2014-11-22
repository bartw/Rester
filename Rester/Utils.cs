using System.Collections.Generic;

namespace BeeWee.Rester
{
    internal static class Utils
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();

            foreach (var dict in dictionaries)
            {
                foreach (var x in dict)
                {
                    result[x.Key] = x.Value;
                }
            }

            return result;
        }
    }
}

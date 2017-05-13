using System.Collections.Generic;

namespace Transmission.API.RPC.ExtensionMethods
{
    public static class DictionaryExtensionMethods
    {
        public static void AddRange<TKey, TValue>(
            this Dictionary<TKey, TValue> dst, Dictionary<TKey, TValue> src)
        {
            foreach (KeyValuePair<TKey, TValue> keyValue in src)
            {
                dst.Add(keyValue.Key, keyValue.Value);
            }
        }
    }
}

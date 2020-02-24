using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClashRoyaleUtils.Extensions
{
    public static class DistributedCacheExtension
    {
        public static void Set<T>(this IDistributedCache cache, string key, T value, TimeSpan? timeExpiration) where T : class
        {
            var objJson = JsonConvert.SerializeObject(value);
            var bytesObj = Encoding.UTF8.GetBytes(objJson);

            if (!timeExpiration.HasValue)
            {
                cache.Set(key, bytesObj);
                return;
            }

            var options = new DistributedCacheEntryOptions()
            {
                SlidingExpiration = timeExpiration
            };

            cache.Set(key, bytesObj, options);
        }

        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var byteObj = cache.Get(key);
            if (byteObj == null) return default(T); 

            var jsonObj = Encoding.UTF8.GetString(byteObj);
            var objReturn = JsonConvert.DeserializeObject<T>(jsonObj);
           
            return objReturn;
        }
    }
}



using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils 
{
    public class CacheUtil
    {
        /// <summary>
        /// 键值前缀
        /// </summary>
        public static readonly string CACHEKEY_PREFIX = "_51XUANXIAO_COURSE_";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetKey(string key)
        {
            return string.Concat(CACHEKEY_PREFIX, key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Insert<T>(string key, T value)
        {
            string jsonValue = JsonConvert.SerializeObject(value);
            MemcachedHelper.Set<string>(GetKey(key), jsonValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime"></param>
        public static void Insert<T>(string key, T value, double? expiredSeconds)
        {
            string jsonValue = JsonConvert.SerializeObject(value);
            if (expiredSeconds != null)
            {
                MemcachedHelper.Set<string>(GetKey(key), jsonValue, DateTime.Now.AddSeconds(expiredSeconds.Value));
            }
            else
            {
                MemcachedHelper.Set<string>(GetKey(key), jsonValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            MemcachedHelper.Remove(GetKey(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            string jsonValue = MemcachedHelper.Get<string>(GetKey(key));
            if (!string.IsNullOrEmpty(jsonValue))
            {
                T data = JsonConvert.DeserializeObject<T>(jsonValue);
                return data;
            }
            else
            {
                return default(T);
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGet<T>(string key, out T result)
        {
            bool has = false;

            result = Get<T>(key);
            if (result != null)
            {
                has = true;
            }
            return has;
        }


    }
}

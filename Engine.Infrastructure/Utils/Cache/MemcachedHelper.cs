using System;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// Memcached助手
    /// </summary>
    public class MemcachedHelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add<T>(string key, T value)
        {
            Store(StoreMode.Add, key, value, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(string key, object value)
        {
            Store(StoreMode.Add, key, value, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime"></param>
        public static void Add<T>(string key, T value, DateTime? expiredTime)
        {
            Store(StoreMode.Add, key, value, expiredTime);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime"></param>
        public static void Add(string key, object value, DateTime? expiredTime)
        {
            Store(StoreMode.Add, key, value, expiredTime);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set<T>(string key, T value)
        {
            Store(StoreMode.Set, key, value, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, object value)
        {
            Store(StoreMode.Set, key, value, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime"></param>
        public static void Set<T>(string key, T value, DateTime? expiredTime)
        {
            Store(StoreMode.Set, key, value, expiredTime);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime"></param>
        public static void Set(string key, object value, DateTime? expiredTime)
        {
            Store(StoreMode.Set, key, value, expiredTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiredTime"></param>
        public static void Store(StoreMode mode, string key, object value, DateTime? expiredTime)
        {
            if (expiredTime != null)
            {
                MemcachedInstance.Client.Store(mode, key, value, expiredTime.Value);
            }
            else
            {
                MemcachedInstance.Client.Store(mode, key, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            MemcachedInstance.Client.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            T result = MemcachedInstance.Client.Get<T>(key); ;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            object result = MemcachedInstance.Client.Get(key);
            return result;
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
            object obj;
            bool has = MemcachedInstance.Client.TryGet(key, out obj);
            if (has)
            {
                result = (T)obj;
                return true;
            }
            else
            {
                result = default(T);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGet(string key, out object result)
        {
            object obj;
            bool has = MemcachedInstance.Client.TryGet(key, out obj);
            if (has)
            {
                result = obj;
                return true;
            }
            else
            {
                result = null;
            }
            return false;
        }
    }
}

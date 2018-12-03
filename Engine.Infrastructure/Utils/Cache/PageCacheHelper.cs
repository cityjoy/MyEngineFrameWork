using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Engine.Infrastructure.Utils
{

    /// <summary>
    /// 对页面级缓存HttpContext.Items的封装操作的助手类
    /// </summary>
  
    public sealed class PageCacheHelper
    {

        /// <summary>
        /// 尝试获取页面缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>是否获取成功</returns>
        public static bool TryGet<T>(string key, out T value)
        {
            if (HttpContext.Current == null || !HttpContext.Current.Items.Contains(key))
            {
                value = default(T);
                return false;
            }
            else
            {
                if (HttpContext.Current.Items[key] is T)
                {
                    value = (T)HttpContext.Current.Items[key];
                    return true;
                }
            }
            value = default(T);
            return false;
        }

        /// <summary>
        /// 是否存在指定缓名的页面缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            if (HttpContext.Current == null)
                return false;

            return (HttpContext.Current.Items.Contains(key));
        }
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(string key, object value)
        {
            if (HttpContext.Current == null)
                return;

            HttpContext.Current.Items[key] = value;
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            if (HttpContext.Current == null)
                return;

            HttpContext.Current.Items.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyPrefix"></param>
        public static void RemoveBySearch(string keyPrefix)
        {
            if (string.IsNullOrEmpty(keyPrefix))
                return;

            List<string> keys = new List<string>();

            foreach (DictionaryEntry elem in HttpContext.Current.Items)
            {
                string key = elem.Key.ToString();

                if (StringHelper.StartsWithIgnoreCase(key, keyPrefix))
                    keys.Add(key);
            }

            foreach (string key in keys)
            {
                try
                {
                    HttpContext.Current.Items.Remove(key);
                }
                catch { }
            }
        }

    }
}

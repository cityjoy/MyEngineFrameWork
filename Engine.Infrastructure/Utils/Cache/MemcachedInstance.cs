using System;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 单例Memcached客户端
    /// </summary>

    public class MemcachedInstance
    {

        private static object _instanceLocker = new object();

        /// <summary>
        /// 
        /// </summary>
        static MemcachedInstance()
        {
            Init();
        }

        /// <summary>
        /// 单例Memcached客户端
        /// </summary>
        public static MemcachedClient Client
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            lock (_instanceLocker)
            {
                Client = new MemcachedClient("enyim.com/memcached");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Dispose()
        {
            lock (_instanceLocker)
            {
                Client.Dispose();
            }
        }

    }
}

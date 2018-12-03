using System;
using System.Collections.Generic;
using System.Configuration;
using ServiceStack.Redis;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// Redis缓存读取设置  封装
    /// </summary>
    public static class RedisHelper
    {
        /// <summary>
        /// 创建Redis连接池管理对象(添加ServiceStack.Interfaces.dll、ServiceStack.Redis.dll)
        /// </summary>
        /// <param name="readWriteHosts">只写服务器</param>
        /// <param name="readOnlyHosts">只读服务器</param>
        /// <returns></returns>
        private static PooledRedisClientManager CreateRedisManager(IEnumerable<string> readWriteHosts,
            IEnumerable<string> readOnlyHosts)
        {
            //支持读写分离，均衡负载
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
            {
                MaxWritePoolSize = 5, //“写”链接池数
                MaxReadPoolSize = 5, //“读”链接池数
                AutoStart = true,
            });
        }

        /// <summary>
        /// 调用CreateRedisManager方法，创建连接池管理对象,Redis服务器地址在配置文件中配置(创建只读，只写连接池)
        /// <add key="RedisHosts" value="127.0.0.1:6379" />
        /// </summary>
        private static readonly PooledRedisClientManager Prcm = CreateRedisManager(
            ConfigurationManager.AppSettings["Hosts"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries),
            ConfigurationManager.AppSettings["Hosts"].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

        /// <summary>
        /// 给缓存中添加数据，使用：RedisHelper.Set(key,值(需要存放的值));
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public static void Set<T>(string key, T val)
        {
            using (IRedisClient rdc = Prcm.GetClient())
            {
                rdc.Set<T>(key, val);
            }
        }

        /// <summary>
        /// 读取缓存中的数据，使用：var result=RedisHelper.Get<T>(key);
        /// </summary>
        /// <typeparam name="T">返回读取的数据</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T Get<T>(string key) where T : class
        {
            using (IRedisClient rdc = Prcm.GetReadOnlyClient())
            {
                return rdc.Get<T>(key);
            }
        }

        /// <summary>
        /// 删除缓存中的数据，使用  RedisHelper.Remove(key);
        /// </summary>
        /// <param name="key">键</param>
        public static void Remove(string key)
        {
            using (IRedisClient rdc = Prcm.GetClient())
            {
                rdc.Remove(key);
            }
        }

        /// <summary>
        /// 缓存中是否包含查询的键数据，使用 var isTrue=RedisHelper.ContainsKey(key);
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>如果包含，返回true,否则返回false</returns>
        public static bool ContainsKey(string key)
        {
            using (IRedisClient rdc = Prcm.GetReadOnlyClient())
            {
                return rdc.ContainsKey(key);
            }
        }

        /// <summary>
        /// 给缓存中添加Object对象，使用：RedisHelper.Add(key,值(需要存放的值))
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Add(string key, object value)
        {
            using (IRedisClient rdc = Prcm.GetClient())
            {
                if (!rdc.ContainsKey(key))
                {
                    rdc.Add(key, value, DateTime.Now.AddMinutes(30));
                }
                else
                {
                    rdc.Set(key, value);
                }
            }
        }

        /// <summary>
        /// 根据key刷新缓存中的数据信息，使用：RedisHelper.RefreshCache(key)
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">键</param>
        public static void RefreshCache<T>(string key) where T : class
        {
            using (IRedisClient rdc = Prcm.GetClient())
            {
                var value = rdc.Get<T>(key);
                rdc.Remove(key);
                rdc.Set<T>(key, value);
            }
        }

        /// <summary>
        /// 根据key集合信息读取缓存中的键值对，返回字典形式的数据存放，使用：RedisHelper.GetList(keys);
        /// </summary>
        /// <param name="keys">key集合</param>
        /// <returns>返回字典集合</returns>
        public static Dictionary<string, string> GetList(List<string> keys)
        {
            using (IRedisClient rdc = Prcm.GetReadOnlyClient())
            {
                return rdc.GetValuesMap<string>(keys);
            }
        }

        /// <summary>
        /// 将字典集合添加到缓存中，使用：RedisHelper.Set(values);
        /// </summary>
        /// <param name="values">字典集合信息</param>
        public static void Set(Dictionary<string, string> values)
        {
            using (IRedisClient rdc = Prcm.GetReadOnlyClient())
            {
                rdc.SetAll(values);
            }
        }

    }
}
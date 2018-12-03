using Engine.Domain.Entity;
using Engine.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engine.Doman.Cache
{
    public class UserCache
    {
      
        /// <summary>
        /// 从缓存层获取指定用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static User GetUserById(int userId)
        {
            string cacheKey = string.Concat("ENGINEUSER_", userId);
            User user = CacheUtil.Get<User>(cacheKey);
            return user;
        }

        /// <summary>
        /// 缓存指定用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="secends"></param>
        /// <returns></returns>
        public static User SetUser(User user,long secends=7200)
        {
            string cacheKey = string.Concat("ENGINEUSER_", user.Id);
            CacheUtil.Insert<User>(cacheKey, user, secends);
            return user;
        }

        /// <summary>
        /// 移除指定用户缓存
        /// </summary>
        /// <param name="userId"></param>
        public static void RemoveUserCache(int userId)
        {
            string cacheKey = string.Concat("ENGINEUSER_", userId);
            CacheUtil.Remove(cacheKey);
        }


    }
}
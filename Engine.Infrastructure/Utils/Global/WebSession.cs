using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Engine.Infrastructure.Utils
{
        /// <summary>
    /// 模拟SESSION(保存于memcached中)，唯一键依赖于同一COOKIE域下的COOKIE值:ASP.NET_SessionId
    /// (使用此机制取代ASP.NET的Session，注意：因为依赖于COOKIE，所以仅限于在User Agent发起的请求使用有效)
    /// </summary>

    public class WebSession
    {

        private const string CIKEY_CURRENT_SESSION = "CURRENT_WEBSESSION";

        /// <summary>
        /// 默认session
        /// </summary>
        public WebSession()
        {
            this.SessionId = HttpContext.Current.Session.SessionID;
            if (this.SessionId != null)
            {
                HttpContext.Current.Items[CIKEY_CURRENT_SESSION] = this;
            } 
            else
            {
                throw new Exception("invalid session_id!");
            }
        }

        /// <summary>
        /// 实例化指定session_id的session
        /// </summary>
        public WebSession(string sessionId)
        {
            this.SessionId = sessionId;
        }

        /// <summary>
        /// 默认的当前session
        /// </summary>
        public static WebSession Current
        {
            get
            {
                WebSession s = null;
                if (HttpContext.Current != null)
                    s = HttpContext.Current.Items[CIKEY_CURRENT_SESSION] as WebSession;

                if (s == null)
                {
                    s = new WebSession();

                    if (HttpContext.Current != null)
                        HttpContext.Current.Items[CIKEY_CURRENT_SESSION] = s;
                }
                return s;
            }
        }

        /// <summary>
        /// 获取指定session_id的session
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        public static WebSession GetSession(string sessionId)
        {
            WebSession session = new WebSession(sessionId);
            return session;
        }

        /// <summary>
        /// 将ASP.NET_SessionId的COOKIE写到主域
        /// </summary>
        public static void InitSessionId()
        {
            #region 将ASP.NET_SessionId的COOKIE写到主域

            string sessionId = null;
            HttpCookie sessionIdCookie = WebHelper.GetCookie(Constants.ASP_NET_SESSIONID_KEY);
            if (sessionIdCookie != null)
            {
                sessionId = sessionIdCookie.Value;
            }

            if (sessionId == null)
            {
                string s = Guid.NewGuid().ToString().Replace("-", "");
                s += "|cybd2016";

                sessionId = SecurityHelper.DESCryptoEncode(s);
            }

            WebHelper.SetCookie(Constants.ASP_NET_SESSIONID_KEY, sessionId, Constants.COOKIE_DOMAIN);

            #endregion
        }

        /// <summary>
        /// Gets the current session identifier.
        /// </summary>
        public static string GetCurrentSessionId()
        {
            string sessionId = null;
            HttpCookie sessionIdCookie = WebHelper.GetCookie(Constants.ASP_NET_SESSIONID_KEY);
            if (sessionIdCookie != null)
            {
                sessionId = sessionIdCookie.Value;
            }

            return sessionId;
        }

        /// <summary>
        /// 
        /// </summary>
        public string SessionId
        {
            get;
            private set;
        }


        public List<string> GetKeys()
        {
            string cacheKey = string.Concat("SESSION_KEYS_", this.SessionId);
            List<string> keys;
            if (CacheUtil.TryGet<List<string>>(cacheKey, out keys))
            {
                return keys;
            }
            return null;
        }
        private void AddKey(string key)
        {
            List<string> keys = GetKeys();
            if (keys == null) { keys = new List<string>(); }
            if (!keys.Contains(key))
            {
                keys.Add(key);

                string cacheKey = string.Concat("SESSION_KEYS_", this.SessionId);
                CacheUtil.Insert<List<string>>(cacheKey, keys, 2592000f);
            }
        }
        private void RemoveKey(string key)
        {
            List<string> keys = GetKeys();
            if (keys != null)
            {
                if (keys.Contains(key))
                {
                    keys.Remove(key);

                    string cacheKey = string.Concat("SESSION_KEYS_", this.SessionId);
                    CacheUtil.Insert<List<string>>(cacheKey, keys, 2592000f);
                }
            }
        }

        /// <summary>
        /// memcached缓存键值
        /// </summary>
        /// <param name="key">The key.</param>
        private string GetCacheKey(string key)
        {
            string sessionId = this.SessionId;
            if (!string.IsNullOrEmpty(sessionId))
            {
                return string.Concat("SESSION_", sessionId, "_", key);
            }
            throw new Exception("invalid session_id!");
        }


        /// <summary>
        /// 设置指定键的Session项值，如果存在则覆盖
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set<T>(string key, T value)
        {
            string cacheKey = GetCacheKey(key);
            CacheUtil.Insert<T>(cacheKey, value, Constants.SESSION_EXPIRED_SECONDS);
            PageCacheHelper.Set(cacheKey, value);

            AddKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T Get<T>(string key, T defaultValue)
        {
            string cacheKey = GetCacheKey(key);
            T v;
            if (!PageCacheHelper.TryGet<T>(cacheKey, out v))
            {
                if (!CacheUtil.TryGet<T>(cacheKey, out v))
                {
                    v = defaultValue;
                }
                PageCacheHelper.Set(cacheKey, v);
            }

            return v;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            string cacheKey = GetCacheKey(key);

            CacheUtil.Remove(cacheKey);

            RemoveKey(key);
        }

        /// <summary>
        /// 清除该用户的所有session数据
        /// </summary>
        public void Clear()
        {
            List<string> keys = GetKeys();
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    string ck = GetCacheKey(key);
                    CacheUtil.Remove(ck);
                }

                string cacheKey = string.Concat("SESSION_KEYS_", this.SessionId);
                CacheUtil.Remove(cacheKey);
            }
        }
    }
}


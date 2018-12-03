using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Web.UI;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// URL的操作类
    /// </summary>
    public class UrlHelper
    {
        static Encoding encoding = Encoding.UTF8;

        #region URL的64位编码
        /// <summary>
        /// URL的64位编码
        /// </summary>
        /// <param name="sourthUrl"></param>
        /// <returns></returns>
        public static string Base64Encrypt(string sourthUrl)
        {
            string eurl = HttpUtility.UrlEncode(sourthUrl);
            eurl = Convert.ToBase64String(encoding.GetBytes(eurl));
            return eurl;
        }
        #endregion

        #region URL的64位解码
        /// <summary>
        /// URL的64位解码
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static string Base64Decrypt(string eStr)
        {
            if (!IsBase64(eStr))
            {
                return eStr;
            }
            byte[] buffer = Convert.FromBase64String(eStr);
            string sourthUrl = encoding.GetString(buffer);
            sourthUrl = HttpUtility.UrlDecode(sourthUrl);
            return sourthUrl;
        }
        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(string eStr)
        {
            if ((eStr.Length % 4) != 0)
            {
                return false;
            }
            if (!Regex.IsMatch(eStr, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 获取POST数据
        /// </summary>
        /// <typeparam name="T">数据类型（暂支持：int,string Guid,DateTime）,转换失败讲返回（int---0,string---"", Guid---Guid.Empty,DateTime---DateTime.MinValue）</typeparam>
        /// <param name="type">参数名</param>
        /// <returns></returns>       
        public static T GetPostValue<T>(string paramName)
        {
            string paramValue = System.Web.HttpContext.Current.Request.Form[paramName];
            return Common.ConvertValue<T>(paramValue);
        }

        /// <summary>
        /// 获取GET数据
        /// </summary>
        /// <typeparam name="T">数据类型（暂支持：int,string Guid,DateTime）,转换失败讲返回（int---0,string---"", Guid---Guid.Empty,DateTime---DateTime.MinValue）</typeparam>
        /// <param name="type">参数名</param>
        /// <returns></returns>
        public static T GetGetValue<T>(string paramName)
        {
            string paramValue = System.Web.HttpContext.Current.Request.QueryString[paramName];
            return Common.ConvertValue<T>(paramValue);
        }

        /// <summary>
        /// 获取POST或Get数据(当POST数据为空时，则获取GET数据)
        /// </summary>
        /// <typeparam name="T">数据类型（暂支持：int,string Guid,DateTime）,转换失败讲返回（int---0,string---"", Guid---Guid.Empty,DateTime---DateTime.MinValue）</typeparam>
        /// <param name="type">参数名</param>
        /// <returns></returns>
        public static T GetPostOrGetValue<T>(string paramName)
        {
            if (typeof(T).Name.ToLower() == "int32")
            {
                int value = GetPostValue<int>(paramName);
                if (value == 0)
                {
                    value = GetGetValue<int>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "string")
            {
                string value = GetPostValue<string>(paramName);
                if (value == string.Empty)
                {
                    value = GetGetValue<string>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "guid")
            {
                Guid value = GetPostValue<Guid>(paramName);
                if (value == Guid.Empty)
                {
                    value = GetGetValue<Guid>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "datetime")
            {
                DateTime value = GetPostValue<DateTime>(paramName);
                if (value == DateTime.MinValue)
                {
                    value = GetGetValue<DateTime>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "boolean")
            {
                bool value = GetPostValue<bool>(paramName);
                if (!value)
                {
                    value = GetGetValue<bool>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "byte")
            {
                byte value = GetPostValue<byte>(paramName);
                if (value == 0)
                {
                    value = GetGetValue<byte>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "double")
            {
                double value = GetPostValue<double>(paramName);
                if (value == 0)
                {
                    value = GetGetValue<double>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "single")
            {
                float value = GetPostValue<float>(paramName);
                if (value == 0)
                {
                    value = GetGetValue<float>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "int64")
            {
                Int64 value = GetPostValue<Int64>(paramName);
                if (value == 0)
                {
                    value = GetGetValue<Int64>(paramName);
                }
                return (T)(object)value;
            }
            else if (typeof(T).Name.ToLower() == "long")
            {
                long value = GetPostValue<long>(paramName);
                if (value == 0)
                {
                    value = GetGetValue<long>(paramName);
                }
                return (T)(object)value;
            }
            return default(T);
        }

        /// <summary>
        /// 移除URL中指定参数
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="paramName">要移除的参数</param>
        /// <returns></returns>
        public static string RemoveParam(string url, string paramName)
        {
            url = UpdateParam(url, paramName, string.Empty);
            url = url.Replace(string.Format("?{0}=", paramName), string.Empty);
            url = url.Replace(string.Format("&{0}=", paramName), string.Empty);
            url = url.IndexOf("?") == -1 ? url.Replace("&", "?") : url;
            return url;
        }

        /// <summary>
        /// 设置转发URL
        /// </summary>
        /// <param name="jumpUrl">跳转的URL</param>
        /// <returns></returns>
        public static string SetReturnUrl(string jumpUrl)
        {
            return UpdateParam(jumpUrl, "returnUrl", HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()));
        }

        /// <summary>
        /// 设置转发URL
        /// </summary>
        /// <param name="jumpUrl">跳转的URL</param>
        /// <returns></returns>
        public static string SetReturnUrl(string jumpUrl, string returnUrl)
        {
            return UpdateParam(jumpUrl, "returnUrl", HttpUtility.UrlEncode(returnUrl));
        }

        /// <summary>
        /// 添加URL参数
        /// </summary>
        public static string AddParam(string url, string paramName, string value)
        {
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.Query))
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "?" + paramName + "=" + eval);
            }
            else
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "&" + paramName + "=" + eval);
            }
        }
        /// <summary>
        /// 更新URL参数
        /// </summary>
        public static string UpdateParam(string url, string paramName, string value)
        {
            string keyWord = "?" + paramName + "=";
            int index = url.IndexOf(keyWord) + keyWord.Length;

            string keyWord_2 = "&" + paramName + "=";
            int index_2 = url.IndexOf(keyWord_2) + keyWord_2.Length;

            if (url.IndexOf(keyWord) != -1)
            {
                int index1 = url.IndexOf("&", index);
                if (index1 == -1)
                {
                    url = url.Remove(index, url.Length - index);
                    url = string.Concat(url, value);
                    return url;
                }
                url = url.Remove(index, index1 - index);
                url = url.Insert(index, value);
            }
            else if (url.IndexOf(keyWord_2) != -1)
            {
                int index1 = url.IndexOf("&", index_2);
                if (index1 == -1)
                {
                    url = url.Remove(index_2, url.Length - index_2);
                    url = string.Concat(url, value);
                    return url;
                }
                url = url.Remove(index_2, index1 - index_2);
                url = url.Insert(index_2, value);
            }
            else
            {
                if (url.IndexOf("?") == -1)
                {
                    url += string.Format("{0}{1}", keyWord, value);
                }
                else
                {
                    url += string.Format("{0}{1}", keyWord_2, value);
                }
            }
            return url;
        }

        /// <summary>
        /// 获取以http://开头的当前请求的服务器的域名系统(DNS)主机名或IP地址和端口号
        /// </summary>
        public static string GetUrlAuthority
        {
            get
            {
                return "http://" + HttpContext.Current.Request.Url.Host.TrimEnd('/');
            }
        }

        /// <summary>
        /// 获取当前请求的Url
        /// </summary>
        public static string CurrUrl
        {
            get
            {
                return HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.ToString());
            }
        }


        /// <summary>
        /// 返回UrlEncode编码后的url
        /// </summary>
        public static string UrlEncode(string url)
        {
            return HttpContext.Current.Server.UrlEncode(url);
        }

        /// <summary>
        /// 返回UrlEncode解码后的url路径
        /// </summary>
        public static string UrlPathEncode(string url)
        {
            return HttpContext.Current.Server.UrlPathEncode(url);
        }

        /// <summary>
        /// 返回UrlEncode解码后的url
        /// </summary>
        public static string UrlDecode(string url)
        {
            return HttpContext.Current.Server.UrlDecode(url);
        }


        /// <summary>
        /// 将URL转换成在请求客户端可用的URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ResolveUrl(string url)
        {
            return new Control().ResolveUrl(url);
        }

        /// <summary>
        /// 将URL转换成浏览器可用的URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ResolveClientUrl(string url)
        {
            return new Control().ResolveClientUrl(url);
        }

        /// <summary>
        /// 返回Web服务器上指定虚拟路径相对应的物理文件路径
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string MapPath(string url)
        {
            return HttpContext.Current.Server.MapPath(url);
        }

        #region 分析URL所属的域
        public static void GetDomain(string fromUrl, out string domain, out string subDomain)
        {
            domain = "";
            subDomain = "";
            try
            {
                if (fromUrl.IndexOf("的名片") > -1)
                {
                    subDomain = fromUrl;
                    domain = "名片";
                    return;
                }

                UriBuilder builder = new UriBuilder(fromUrl);
                fromUrl = builder.ToString();

                Uri u = new Uri(fromUrl);

                if (u.IsWellFormedOriginalString())
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";

                    }
                    else
                    {
                        string Authority = u.Authority;
                        string[] ss = u.Authority.Split('.');
                        if (ss.Length == 2)
                        {
                            Authority = "www." + Authority;
                        }
                        int index = Authority.IndexOf('.', 0);
                        domain = Authority.Substring(index + 1, Authority.Length - index - 1).Replace("comhttp", "com");
                        subDomain = Authority.Replace("comhttp", "com");
                        if (ss.Length < 2)
                        {
                            domain = "不明路径";
                            subDomain = "不明路径";
                        }
                    }
                }
                else
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";
                    }
                    else
                    {
                        subDomain = domain = "不明路径";
                    }
                }
            }
            catch
            {
                subDomain = domain = "不明路径";
            }
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            nvc = new NameValueCollection();
            baseUrl = "";

            if (url == "")
                return;

            int questionMarkIndex = url.IndexOf('?');

            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);

            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);

            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }

        #endregion
    }
}

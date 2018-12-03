using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// WEB助手
    /// </summary>

    public sealed class WebHelper
    {

        #region Decode, Encode

        /// <summary>
        /// 对URL字符串进行编码
        /// </summary>
        /// <param name="content"></param>
        public static string UrlEncode(string content)
        {
            string result = HttpUtility.UrlEncode(content);
            return result;
        }

        /// <summary>
        /// 对URL字符串进行编码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        public static string UrlEncode(string content, Encoding encoding)
        {
            string result = HttpUtility.UrlEncode(content, encoding);
            return result;
        }


        /// <summary>
        /// 对URL字符串进行解码
        /// </summary>
        /// <param name="content"></param>
        public static string UrlDecode(string content)
        {
            return UrlDecode(content, true);
        }
        /// <summary>
        /// 对URL字符串进行解码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="special">對特殊符號進行解碼處理</param>
        public static string UrlDecode(string content, bool special)
        {
            string result = HttpUtility.UrlDecode(content);
            if (special)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result = Regex.Replace(result, "%2B", "+", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%2F", "/", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%3F", "?", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%25", "%", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%23", "#", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%26", "&", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%3D", "=", RegexOptions.IgnoreCase);
                }
            }
            return result;
        }

        /// <summary>
        /// 对URL字符串进行解码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        public static string UrlDecode(string content, Encoding encoding)
        {
            return UrlDecode(content, encoding, true);
        }
        /// <summary>
        /// 对URL字符串进行解码
        /// </summary>
        /// <param name="content"></param>
        /// <param name="encoding"></param>
        /// <param name="special">對特殊符號進行解碼處理</param>
        public static string UrlDecode(string content, Encoding encoding, bool special)
        {
            string result = HttpUtility.UrlDecode(content, encoding);
            if (special)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result = Regex.Replace(result, "%2B", "+", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%2F", "/", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%3F", "?", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%25", "%", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%23", "#", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%26", "&", RegexOptions.IgnoreCase);
                    result = Regex.Replace(result, "%3D", "=", RegexOptions.IgnoreCase);
                }
            }
            return result;
        }

        /// <summary>
        /// HTML编码
        /// </summary>
        /// <param name="content"></param>
        public static string HtmlEncode(string content)
        {
            string result = HttpUtility.HtmlEncode(content);

            return result;
        }

        /// <summary>
        /// HTML解码
        /// </summary>
        /// <param name="content"></param>
        public static string HtmlDecode(string content)
        {
            string result = HttpUtility.HtmlDecode(content);

            return result;
        }

        #endregion


        #region Cookie

        /// <summary>
        /// 移除Cookie
        /// </summary>
        /// <param name="cookieKey">键名</param>
        public static void RemoveCookie(string cookieKey)
        {
            RemoveCookie(cookieKey, null);
        }

        /// <summary>
        /// 移除Cookie
        /// </summary>
        /// <param name="cookieKey">键名</param>
        /// <param name="domain"></param>
        public static void RemoveCookie(string cookieKey, string domain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieKey];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);

                cookie.Path = "/";
                if (!string.IsNullOrEmpty(domain))
                {
                    cookie.Domain = domain;
                }

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 修改Cookie的值 如果不存在键则创建 (不设置过期时间)
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="cookieValue"></param>
        public static void SetCookie(string cookieKey, string cookieValue)
        {
            SetCookie(cookieKey, cookieValue, DateTime.MinValue);
        }

        /// <summary>
        /// 修改Cookie的值 如果不存在键则创建
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expires">过期时间</param>
        public static void SetCookie(string cookieKey, string cookieValue, DateTime? expires)
        {
            SetCookie(cookieKey, cookieValue, null, expires);
        }


        /// <summary>
        /// 修改Cookie的值 如果不存在键则创建
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="cookieValue"></param>
        /// <param name="domain">关联的主域</param>
        public static void SetCookie(string cookieKey, string cookieValue, string domain)
        {
            SetCookie(cookieKey, cookieValue, domain, null);
        }
        /// <summary>
        /// 修改Cookie的值 如果不存在键则创建
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="cookieValue"></param>
        /// <param name="expires">过期时间</param>
        /// <param name="domain"></param>
        public static void SetCookie(string cookieKey, string cookieValue, string domain, DateTime? expires)
        {
            HttpCookie cookie = new HttpCookie(cookieKey, cookieValue);
            cookie.Path = "/";
            if (!string.IsNullOrEmpty(domain))
            {
                cookie.Domain = domain;
            }

            if (expires != null && expires.Value != DateTime.MinValue)
                cookie.Expires = expires.Value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <returns></returns>
        public static HttpCookie GetCookie(string cookieKey)
        {
            return HttpContext.Current.Request.Cookies[cookieKey];
        }

        /// <summary>
        /// 设置Cookie (不设置过期时间)
        /// </summary>
        /// <param name="cookieKey"></param>
        /// <param name="itemKey">子项</param>
        /// <param name="cookieValue">子项的值</param>
        public static void SetCookieItem(string cookieKey, string itemKey, string cookieValue)
        {
            SetCookieItem(cookieKey, itemKey, cookieValue, null);
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="cookieKey">键值</param>
        /// <param name="itemKey">子项</param>
        /// <param name="cookieValue">子项的值</param>
        /// <param name="expires">过期时间</param>
        public static void SetCookieItem(string cookieKey, string itemKey, string cookieValue, DateTime? expires)
        {
            HttpCookie cookie = HttpContext.Current.Response.Cookies[cookieKey];

            cookie[itemKey] = cookieValue;

            cookie.Path = "/";

            if (expires != null)
                cookie.Expires = expires.Value;

        }

        #endregion


        /// <summary>
        /// 采集使用的默认客户端
        /// </summary>
        public static string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0";

        /// <summary>
        /// 模拟浏览器抓取指定url地址页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CatchPageContent(string url)
        {
            string contentType = "text/html";
            Uri responseUri;
            return CatchPageContent(url, DefaultUserAgent, contentType, null, Encoding.UTF8, out responseUri);
        }
        /// <summary>
        /// 模拟浏览器抓取指定url地址页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string CatchPageContent(string url, Encoding encoding)
        {
            string contentType = "text/html";
            Uri responseUri;
            return CatchPageContent(url, DefaultUserAgent, contentType, null, encoding, out responseUri);
        }
        /// <summary>
        /// 模拟浏览器抓取指定url地址页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <param name="referer"></param>
        /// <param name="responseUri"></param>
        /// <returns></returns>
        public static string CatchPageContent(string url, Encoding encoding, string referer, out Uri responseUri)
        {
            string contentType = "text/html";

            return CatchPageContent(url, DefaultUserAgent, contentType, referer, encoding, out responseUri);
        }
        /// <summary>
        /// 模拟浏览器抓取指定url地址页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public static string CatchPageContent(string url, Encoding encoding, string referer)
        {
            string contentType = "text/html";
            Uri responseUri;
            return CatchPageContent(url, DefaultUserAgent, contentType, referer, encoding, out responseUri);
        }
        /// <summary>
        /// 模拟浏览器抓取指定url地址页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userAgent"></param>
        /// <param name="contentType"></param>
        /// <param name="referer"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string CatchPageContent(string url, string userAgent, string contentType, string referer, Encoding encoding)
        {
            Uri responseUri;
            return CatchPageContent(url, userAgent, contentType, referer, encoding, out responseUri);
        }
        /// <summary>
        /// 模拟浏览器抓取指定url地址页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userAgent"></param>
        /// <param name="contentType"></param>
        /// <param name="referer"></param>
        /// <param name="encoding"></param>
        /// <param name="responseUri">实际请求的地址信息</param>
        /// <returns></returns>
        public static string CatchPageContent(string url, string userAgent, string contentType, string referer, Encoding encoding, out Uri responseUri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UseDefaultCredentials = true;
            if (!string.IsNullOrEmpty(contentType))
            {
                webRequest.ContentType = contentType;
            }
            if (!string.IsNullOrEmpty(userAgent))
            {
                webRequest.UserAgent = userAgent;
            }
            if (!string.IsNullOrEmpty(referer))
            {
                webRequest.Referer = referer;
            }
            webRequest.Method = "GET";
            webRequest.Timeout = int.MaxValue;
            webRequest.CookieContainer = new CookieContainer();
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            string pageContent;
            using (StreamReader responseReader = new StreamReader(webResponse.GetResponseStream(), encoding))
            {
                pageContent = responseReader.ReadToEnd();
            }
            responseUri = webResponse.ResponseUri;
            webResponse.Close();

            return pageContent;
        }

        /// <summary>
        /// 抓取指定url地址页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CatchDefaultPageContent(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = int.MaxValue;
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            string pageContent;
            using (Stream stream = webResponse.GetResponseStream())
            {
                using (StreamReader responseReader = new StreamReader(stream))
                {
                    pageContent = responseReader.ReadToEnd();
                }
            }
            webResponse.Close();

            return pageContent;
        }


        /// <summary>
        /// 获取真实请求地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Uri GetRealResponseUri(string url)
        {
            return GetRealResponseUri(url, DefaultUserAgent, null, 0);
        }
        /// <summary>
        /// 获取真实请求地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public static Uri GetRealResponseUri(string url, string referer)
        {
            return GetRealResponseUri(url, DefaultUserAgent, referer, 0);
        }
        /// <summary>
        /// 获取真实请求地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Uri GetRealResponseUri(string url, string referer, int timeout)
        {
            return GetRealResponseUri(url, DefaultUserAgent, referer, timeout);
        }
        /// <summary>
        /// 获取真实请求地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userAgent"></param>
        /// <param name="referer"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Uri GetRealResponseUri(string url, string userAgent, string referer, int timeout)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.UseDefaultCredentials = true;

            if (!string.IsNullOrEmpty(userAgent))
            {
                webRequest.UserAgent = userAgent;
            }
            if (!string.IsNullOrEmpty(referer))
            {
                webRequest.Referer = referer;
            }
            if (timeout > 0)
            {
                webRequest.Timeout = timeout;
            }
            webRequest.CookieContainer = new CookieContainer();
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            Uri responseUri = webResponse.ResponseUri;
            webResponse.Close();

            return responseUri;
        }

        /// <summary>
        /// 下载指定地址的二进制数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] DownloadRemoteFileBinary(string url)
        {
            return DownloadRemoteFileBinary(url, DefaultUserAgent, null, string.Empty, null);
        }
        /// <summary>
        /// 下载指定地址的二进制数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static byte[] DownloadRemoteFileBinary(string url, NameValueCollection postData)
        {
            return DownloadRemoteFileBinary(url, DefaultUserAgent, null, null, postData);
        }
        /// <summary>
        /// 下载指定地址的二进制数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        public static byte[] DownloadRemoteFileBinary(string url, string referer)
        {
            return DownloadRemoteFileBinary(url, DefaultUserAgent, referer, null, null);
        }
        /// <summary>
        /// 下载指定地址的二进制数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static byte[] DownloadRemoteFileBinary(string url, string contentType, NameValueCollection postData)
        {
            return DownloadRemoteFileBinary(url, DefaultUserAgent, null, contentType, postData);
        }
        /// <summary>
        /// 下载指定地址的二进制数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="userAgent"></param>
        /// <param name="referer"></param>
        /// <param name="contentType"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static byte[] DownloadRemoteFileBinary(string url, string userAgent, string referer, string contentType, NameValueCollection postData)
        {
            using (WebClient client = new WebClient())
            {
                client.UseDefaultCredentials = true;

                if (!string.IsNullOrEmpty(userAgent))
                {
                    client.Headers.Add(HttpRequestHeader.UserAgent, userAgent);
                }
                if (!string.IsNullOrEmpty(referer))
                {
                    client.Headers.Add(HttpRequestHeader.Referer, userAgent);
                }
                if (!string.IsNullOrEmpty(contentType))
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, contentType);
                }
                if (postData != null && postData.Count > 0)
                {
                    client.QueryString = postData;
                }

                byte[] data = client.DownloadData(url);

                return data;
            }
        }

        /// <summary>
        /// 上传文件至指定
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileStream"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static byte[] UploadFileToRemote(string url, Stream fileStream, NameValueCollection postData)
        {
            return UploadFileToRemote(url, fileStream, postData, DefaultUserAgent, string.Empty);
        }
        /// <summary>
        /// 上传文件至指定
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileStream"></param>
        /// <param name="postData"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static byte[] UploadFileToRemote(string url, Stream fileStream, NameValueCollection postData, string contentType)
        {
            return UploadFileToRemote(url, fileStream, postData, DefaultUserAgent, contentType);
        }
        /// <summary>
        /// 上传文件至指定
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileStream"></param>
        /// <param name="postData"></param>
        /// <param name="userAgent"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static byte[] UploadFileToRemote(string url, Stream fileStream, NameValueCollection postData, string userAgent, string contentType)
        {
            try
            {
                byte[] fileData = BinaryHelper.ConvertStreamToBytes(fileStream);
                using (WebClient client = new WebClient())
                {
                    client.UseDefaultCredentials = true;
                    client.Headers.Add(HttpRequestHeader.UserAgent, userAgent);
                    if (!string.IsNullOrEmpty(contentType))
                    {
                        client.Headers.Add(HttpRequestHeader.ContentType, contentType);
                    }

                    if (postData != null && postData.Count > 0)
                    {
                        client.QueryString = postData;
                    }
                    byte[] result = client.UploadData(url, "POST", fileData);
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return null;
            }
        }
 
    }
}

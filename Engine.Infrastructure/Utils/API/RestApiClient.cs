using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using Engine.Infrastructure.Utils;
using System.Net;
using System.Web.Routing;
using System.Web;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// RestSharp封装类
    /// </summary>
    public class RestApiClient
    {
        public string API_SITE = "";

        public RestRequest Request;
        /// <summary>
        /// 构造函数，初始化RestRequest
        /// </summary>
        public RestApiClient(string resource)
        {
            API_SITE = resource;
            Request = new RestRequest();
            Request.RequestFormat = DataFormat.Json;
            string secretkey = "!cybd*2017$";
            string nonce = Guid.NewGuid().ToString().Replace("-", string.Empty);
            long timestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);
            string signature = BuildSignature(secretkey, timestamp, nonce);
            Request.AddHeader("signature", signature);
            Request.AddHeader("timestamp", timestamp.ToString());
            Request.AddHeader("nonce", nonce);
        }


        public T Get<T>(string url, object pars = null)
        {
            var type = Method.GET;
            T data = GetApiInfo<T>(url, type, pars);
            return data;
        }
        public T Post<T>(string url, object pars = null)
        {
            var type = Method.POST;
            T data = GetApiInfo<T>(url, type, pars);
            return data;
        }
        public T PostFiles<T>(string url, object pars = null, HttpPostedFileBase attachFile = null)
        {
            var type = Method.POST;
            T data = PostAttachFile<T>(url, type, pars, attachFile);
            return data;
        }

        public T PostFiles<T>(string url, object pars = null, ApiPostFile file = null)
        {
            var type = Method.POST;
            T data = PostFile<T>(url, type, pars, file);
            return data;
        }
        public T Delete<T>(string url, object pars = null)
        {
            var type = Method.DELETE;
            T data = GetApiInfo<T>(url, type, pars);
            return data;
        }
        public T Put<T>(string url, object pars = null)
        {
            var type = Method.PUT;
            T data = GetApiInfo<T>(url, type, pars);
            return data;
        }

        /// <summary>
        /// 获取API信息结果
        /// </summary>
        /// <typeparam name="T">结果数据类型</typeparam>
        /// <param name="url">api地址</param>
        /// <param name="type">方法类型</param>
        /// <param name="pars">参数</param>
        /// <param name="attachFile">客户端上传的附件</param>
        /// <returns></returns>
        private T GetApiInfo<T>(string url, Method type, object pars = null)
        {
            Request.Method = type;
            if (pars != null)
            {
                Request.AddObject(pars);
            }

            var client = new RestClient(API_SITE + "/api" + url);
            client.CookieContainer = new System.Net.CookieContainer();
            IRestResponse response = client.Execute(Request);
            if (response.ErrorException != null)
            {
                var s = response.Content;
                LogHelper.WriteLog("调用API出错：" + s);
                throw new Exception("请求出错");
            }
            try
            {
                var s = response.Content;
                T data = JsonConvert.DeserializeObject<T>(response.Content);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("序列化结果出错：" + ex.ToString());

            }
            return default(T);
        }

        /// <summary>
        /// 获取API信息结果
        /// </summary>
        /// <typeparam name="T">结果数据类型</typeparam>
        /// <param name="url">api地址</param>
        /// <param name="type">方法类型</param>
        /// <param name="pars">参数</param>
        /// <param name="attachFile">客户端上传的附件</param>
        /// <returns></returns>
        private T PostAttachFile<T>(string url, Method type, object pars = null, HttpPostedFileBase attachFile = null)
        {
            Request.Method = type;
            Request.RequestFormat = DataFormat.Json;
            string secretkey = "!cybd*2017$";

            string nonce = Guid.NewGuid().ToString().Replace("-", string.Empty);

            long timestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);
            string signature = BuildSignature(secretkey, timestamp, nonce);
            Request.AddHeader("signature", signature);
            Request.AddHeader("timestamp", timestamp.ToString());
            Request.AddHeader("nonce", nonce);
            if (pars != null)
            {
                Request.AddObject(pars);
            }
            if (attachFile != null)
            {

                ApiPostFile file = new ApiPostFile();
                file.FormName = "AttachFile";
                file.FileStream = attachFile.InputStream;
                file.SourceFileName = attachFile.FileName;
                int fileLength = Convert.ToInt32(file.FileStream.Length);
                byte[] fileBytes = new byte[fileLength];
                file.FileStream.Read(fileBytes, 0, fileLength);
                Request.AddFile(file.FormName, fileBytes, file.SourceFileName);

            }
            var client = new RestClient(API_SITE + "/api" + url);
            client.CookieContainer = new System.Net.CookieContainer();
            IRestResponse response = client.Execute(Request);
            if (response.ErrorException != null)
            {
                var s = response.Content;
                LogHelper.WriteLog("调用API出错：" + s);
                throw new Exception("请求出错");
            }
            try
            {
                var s = response.Content;
                T data = JsonConvert.DeserializeObject<T>(response.Content);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("序列化结果出错：" + ex.ToString());

            }
            return default(T);
        }


        /// <summary>
        /// 获取API信息结果
        /// </summary>
        /// <typeparam name="T">结果数据类型</typeparam>
        /// <param name="url">api地址</param>
        /// <param name="type">方法类型</param>
        /// <param name="pars">参数</param>
        /// <param name="file">附件</param>
        /// <returns></returns>
        private T PostFile<T>(string url, Method type, object pars = null, ApiPostFile file = null)
        {
            Request.Method = type;

            if (pars != null)
            {
                Request.AddObject(pars);
            }
            if (file != null)
            {

                file.FormName = "AttachFile";
                int fileLength = Convert.ToInt32(file.FileStream.Length);
                byte[] fileBytes = new byte[fileLength];
                Request.AddFile(file.FormName, fileBytes, file.SourceFileName);

            }
            var client = new RestClient(API_SITE + "/api" + url);
            client.CookieContainer = new System.Net.CookieContainer();
            IRestResponse response = client.Execute(Request);
            if (response.ErrorException != null)
            {
                var s = response.Content;
                LogHelper.WriteLog("调用API出错：" + s);
                throw new Exception("请求出错");
            }
            try
            {
                var s = response.Content;
                T data = JsonConvert.DeserializeObject<T>(response.Content);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("序列化结果出错：" + ex.ToString());

            }
            return default(T);
        }

        /// <summary>
        /// 构建签名串
        /// </summary>
        /// <param name="key">用户密钥</param>
        /// <param name="timestamp">当前时间戳</param>
        /// <param name="nonce">随机字符串</param>
        /// <returns></returns>
        public static string BuildSignature(string key, long timestamp, string nonce)
        {
            string[] tmpArr = new string[] { key, timestamp.ToString(), nonce };
            Array.Sort(tmpArr);
            string tmpStr = StringHelper.Join<string>(tmpArr, "");
            tmpStr = SecurityHelper.SHA1(tmpStr).ToLower();

            return tmpStr;
        }
    }

}

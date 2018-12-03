using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Engine.Infrastructure.Utils;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// Api请求
    /// </summary>
    /// <remarks>
    
    public class ApiRequest
    {
        /// <summary>
        /// RestSharp IRestRequest对象
        /// </summary>
        public IRestRequest RestRequest
        {
            get;
            private set;
        }

        /// <summary>
        /// 资源地址(如: local/v1/school/get_school)
        /// </summary>
        public string Resource
        {
            get;
            private set;
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        public Method  RequestMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// 添加的头部
        /// </summary>
        public Dictionary<string, string> AddedHeaders
        {
            get;
            private set;
        }

        /// <summary>
        /// 添加的请求数据
        /// </summary>
        public NameValueCollection AddedData
        {
            get;
            private set;
        }

        /// <summary>
        /// 添加的文件
        /// </summary>
        public List<ApiPostFile> AddedFiles
        {
            get;
            private set;
        }


        /// <summary>
        /// 初始化一个Api请求对象(默认GET方式)
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        public ApiRequest(string resource)
            : this(resource, Method.GET, null, null, null)
        {
        }
        /// <summary>
        /// 初始化一个Api请求对象
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        /// <param name="method">请求方式(GET或POST...)</param>
        public ApiRequest(string resource, Method method)
            : this(resource, method, null, null, null)
        {
        }
        /// <summary>
        /// 初始化一个Api请求对象(默认GET方式)
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        /// <param name="headers">头部数据</param>
        public ApiRequest(string resource, Dictionary<string, string> headers)
            : this(resource, Method.GET, null, headers, null)
        {
        }
        /// <summary>
        /// 初始化一个Api请求对象(默认GET方式)
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        /// <param name="data">提交的数据(GET或POST的数据)</param>
        public ApiRequest(string resource, NameValueCollection data)
            : this(resource, Method.GET, data, null, null)
        {
        }
        /// <summary>
        /// 初始化一个Api请求对象
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        /// <param name="method">请求方式(GET或POST...)</param>
        /// <param name="headers">头部数据</param>
        public ApiRequest(string resource, Method method, Dictionary<string, string> headers)
            : this(resource, method, null, headers, null)
        {
        }
        /// <summary>
        /// 初始化一个Api请求对象
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        /// <param name="method">请求方式(GET或POST...)</param>
        /// <param name="data">提交的数据(GET或POST的数据)</param>
        public ApiRequest(string resource, Method method, NameValueCollection data)
            : this(resource, method, data, null, null)
        {
        }
        /// <summary>
        /// 初始化一个Api请求对象
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        /// <param name="method">请求方式(GET或POST...)</param>
        /// <param name="headers">头部数据</param>
        /// <param name="data">提交的数据(GET或POST的数据)</param>
        public ApiRequest(string resource, Method method, NameValueCollection data, Dictionary<string, string> headers)
            : this(resource, method, data, headers, null)
        {
        }
        /// <summary>
        /// 初始化一个Api请求对象
        /// </summary>
        /// <param name="resource">资源地址(如: local/v1/school/get_school)</param>
        /// <param name="method">请求方式(GET或POST...)</param>
        /// <param name="headers">头部数据</param>
        /// <param name="data">提交的数据(GET或POST的数据)</param>
        /// <param name="files">提交的文件</param>
        public ApiRequest(string resource, Method method, NameValueCollection data, Dictionary<string, string> headers, List<ApiPostFile> files)
        {
            this.RequestMethod = method;
            this.Resource = resource;
            this.AddedHeaders = headers;
            this.AddedData = data;
            this.AddedFiles = files;
            this.RestRequest = new RestRequest(this.Resource, method);
            #region 添加签名
            string secretkey = "!cybd*2017$";
            if (secretkey == null)
            {
                throw new Exception("invalid api secret key!");
            }

            string nonce = Guid.NewGuid().ToString().Replace("-", string.Empty);

            long timestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);
            string signature = BuildSignature(secretkey, timestamp, nonce);
            this.RestRequest.AddHeader("signature", signature);
            this.RestRequest.AddHeader("timestamp", timestamp.ToString());
            this.RestRequest.AddHeader("nonce", nonce);
          
            #endregion

            if (this.AddedData != null && this.AddedData.Count > 0)
            {
                string[] keys = this.AddedData.AllKeys;

                foreach (string key in keys)
                {
                    var value = this.AddedData[key];
                    this.RestRequest = this.RestRequest.AddParameter(key, value);
                }
            }
            if (this.AddedHeaders != null && this.AddedHeaders.Count > 0)
            {
                foreach (KeyValuePair<string, string> headerKv in this.AddedHeaders)
                {
                    this.RestRequest.AddHeader(headerKv.Key, headerKv.Value);
                }
            }
            if (this.AddedFiles != null && this.AddedFiles.Count > 0)
            {
                foreach (ApiPostFile file in this.AddedFiles)
                {
                    int fileLength = Convert.ToInt32(file.FileStream.Length);
                    byte[] fileBytes = new byte[fileLength];
                    file.FileStream.Read(fileBytes, 0, fileLength);
                    this.RestRequest.AddFile(file.FormName, fileBytes, file.SourceFileName);
                }
            }
        }

        /// <summary>
        /// 添加请求数据
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        public void AddData(string key, string value)
        {
            this.AddedData.Add(key, value);
            this.RestRequest = this.RestRequest.AddParameter(key, value);
        }

        /// <summary>
        /// 添加头信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public void AddHeader(string name, string value)
        {
            this.AddedHeaders.Add(name, value);
            this.RestRequest.AddHeader(name, value);
        }

        /// <summary>
        /// 添加上传文件
        /// </summary>
        /// <param name="formName">Name of the form.</param>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="sourceFileName">Name of the source file.</param>
        public void AddFile(string formName, Stream fileStream, string sourceFileName)
        {
            ApiPostFile file = new ApiPostFile();
            file.FormName = formName;
            file.FileStream = fileStream;
            file.SourceFileName = sourceFileName;
            this.AddedFiles.Add(file);

            int fileLength = Convert.ToInt32(file.FileStream.Length);
            byte[] fileBytes = new byte[fileLength];
            file.FileStream.Read(fileBytes, 0, fileLength);
            this.RestRequest.AddFile(file.FormName, fileBytes, file.SourceFileName);
        }

        /// <summary>
        /// 构建签名串
        /// </summary>
        /// <param name="key">用户密钥</param>
        /// <param name="timestamp">当前时间戳</param>
        /// <param name="nonce">随机字符串</param>
        /// <returns></returns>
        public  static string BuildSignature(string key, long timestamp, string nonce)
        {
            string[] tmpArr = new string[] { key, timestamp.ToString(), nonce };
            Array.Sort(tmpArr);
            string tmpStr = StringHelper.Join<string>(tmpArr, "");
            tmpStr = SecurityHelper.SHA1(tmpStr).ToLower();

            return tmpStr;
        }


    }
}

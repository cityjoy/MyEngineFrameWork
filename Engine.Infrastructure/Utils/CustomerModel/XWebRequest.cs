using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils 
{
    /// <summary>
    /// 封装WEB http请求
    /// </summary>
 
    public class XWebRequest
    {
        /// <summary>
        /// 发送的文件数据
        /// </summary>
        public class PostFile
        {
            /// <summary>
            /// 
            /// </summary>
            public string FormName
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string FileName
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public Stream FileStream
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public byte[] PostHeaderBytes
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 发送的表单项数据
        /// </summary>
        public class PostItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string FormName
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public string Data
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public byte[] PostHeaderBytes
            {
                get;
                set;
            }

            /// <summary>
            /// 
            /// </summary>
            public byte[] PostDataBytes
            {
                get;
                set;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public const string POST_CONTENT_TYPE_URLENCODED = "application/x-www-form-urlencoded";
        /// <summary>
        /// 
        /// </summary>
        public const string POST_CONTENT_TYPE_FILE = "multipart/form-data";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public XWebRequest(string url)
        {
            this.Url = url;
        }

        /// <summary>
        /// 请求的地址
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// application/x-www-form-urlencoded
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// 客户端
        /// </summary>
        public string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// 引用页
        /// </summary>
        public string Referer
        {
            get;
            set;
        }

        /// <summary>
        /// GET,POST...
        /// </summary>
        public string Method
        {
            get;
            set;
        }

        private int _timeout = int.MaxValue;
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public CookieContainer CookieContainer
        {
            get;
            set;
        }

        /// <summary>
        /// 发送的网址参数
        /// </summary>
        public Dictionary<string, string> QueryItems
        {
            get;
            set;
        }

        /// <summary>
        /// 添加网址参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddQueryItem(string key, string value)
        {
            if (QueryItems == null)
            {
                QueryItems = new Dictionary<string, string>();
            }

            if (!QueryItems.ContainsKey(key))
            {
                QueryItems.Add(key, value);
            }
            else
            {
                QueryItems[key] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, PostItem> PostItems
        {
            get;
            set;
        }

        /// <summary>
        /// 添加提交的表单项
        /// </summary>
        public void AddPostData(string formName, string data)
        {
            if (PostItems == null)
            {
                PostItems = new Dictionary<string, PostItem>();
            }

            PostItem item = new PostItem();
            item.FormName = formName;
            item.Data = data;

            if (!this.PostItems.ContainsKey(formName))
            {

                PostItems.Add(formName, item);
            }
            else
            {
                PostItems[formName] = item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, PostFile> PostFiles
        {
            get;
            set;
        }

        /// <summary>
        /// 添加发送文件
        /// </summary>
        /// <param name="formName"></param>
        /// <param name="fileName"></param>
        /// <param name="fileStream"></param>
        public void AddPostFile(string formName, string fileName, Stream fileStream)
        {
            if (PostFiles == null)
            {
                PostFiles = new Dictionary<string, PostFile>();
            }
            PostFile item = new PostFile();
            item.FormName = formName;
            item.FileName = fileName;
            item.FileStream = fileStream;

            if (!this.PostFiles.ContainsKey(formName))
            {
                PostFiles.Add(formName, item);
            }
            else
            {
                PostFiles[formName] = item;
            }
        }

        private Encoding _encoding = Encoding.UTF8;
        /// <summary>
        /// 编码
        /// </summary>
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        /// <summary>
        /// 模拟表单提交数据
        /// </summary>
        /// <returns></returns>
        public string ApplyForm()
        {
            string queryString = string.Empty;
            if (this.QueryItems != null && this.QueryItems.Count > 0)
            {
                foreach (KeyValuePair<string, string> queryItem in this.QueryItems)
                {
                    queryString += string.Concat(queryItem.Key, "=", WebHelper.UrlEncode(queryItem.Value), "&");
                }
                if (!string.IsNullOrEmpty(queryString))
                {
                    queryString = queryString.Substring(0, queryString.Length - 1);
                }
                if (this.Url.LastIndexOf("?") > 0)
                {
                    this.Url += string.Concat("&", queryString);
                }
                else
                {
                    this.Url += string.Concat("?", queryString);
                }
            }
            if (PostItems != null && PostItems.Count > 0)
            {
                this.Url = PathHelper.ResolveParamUrl(this.Url, "__UrlEncode", "True");
            }

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.UseDefaultCredentials = true;
            if (!string.IsNullOrEmpty(ContentType))
            {
                webRequest.ContentType = ContentType;
            }
            if (!string.IsNullOrEmpty(UserAgent))
            {
                webRequest.UserAgent = UserAgent;
            }
            if (!string.IsNullOrEmpty(Referer))
            {
                webRequest.Referer = Referer;
            }
            if (!string.IsNullOrEmpty(Method))
            {
                webRequest.Method = Method;
            }
            webRequest.Timeout = Timeout;

            if (string.Compare(webRequest.Method, WebRequestMethods.Http.Post, true) == 0 && PostFiles != null && PostFiles.Count > 0)
            {
                #region 上传文件

                //时间戳
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] trailerBoundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                webRequest.AllowWriteStreamBuffering = false;
                webRequest.ContentType = POST_CONTENT_TYPE_FILE + "; boundary=" + boundary;

                long totalLength = 0;

                #region 表单项的头与长度

                if (PostItems != null && PostItems.Count > 0)
                {
                    foreach (KeyValuePair<string, PostItem> kv in PostItems)
                    {
                        PostItem item = kv.Value;

                        string formName = item.FormName;

                        //请求头部信息
                        StringBuilder sb = new StringBuilder();
                        sb.Append("\r\n--");
                        sb.Append(boundary);
                        sb.Append("\r\n");
                        sb.Append("Content-Disposition: form-data; name=\"");
                        sb.Append(formName);
                        sb.Append("\"");
                        sb.Append("\r\n");
                        sb.Append("\r\n");
                        string strPostHeader = sb.ToString();
                        byte[] postHeaderBytes = Encoding.GetBytes(strPostHeader);
                        byte[] dataBytes = Encoding.GetBytes(WebHelper.UrlEncode(item.Data));

                        item.PostHeaderBytes = postHeaderBytes;
                        item.PostDataBytes = dataBytes;

                        totalLength += dataBytes.Length + postHeaderBytes.Length;
                    }
                }

                #endregion

                #region 上传文件的头与长度

                foreach (KeyValuePair<string, PostFile> uploadFileKV in PostFiles)
                {
                    PostFile uploadFile = uploadFileKV.Value;

                    string formName = uploadFile.FormName;
                    string fileName = uploadFile.FileName;
                    Stream fs = uploadFile.FileStream;

                    //请求头部信息
                    StringBuilder sb = new StringBuilder();
                    sb.Append("\r\n--");
                    sb.Append(boundary);
                    sb.Append("\r\n");
                    sb.Append("Content-Disposition: form-data; name=\"");
                    sb.Append(formName);
                    sb.Append("\"; filename=\"");
                    sb.Append(fileName);
                    sb.Append("\"");
                    sb.Append("\r\n");
                    sb.Append("Content-Type: ");
                    sb.Append("application/octet-stream");
                    sb.Append("\r\n");
                    sb.Append("\r\n");
                    string strPostHeader = sb.ToString();
                    byte[] postHeaderBytes = Encoding.GetBytes(strPostHeader);

                    uploadFile.PostHeaderBytes = postHeaderBytes;

                    totalLength += fs.Length + postHeaderBytes.Length;

                    uploadFile.FileStream.Position = 0;
                }

                #endregion

                totalLength += trailerBoundaryBytes.Length; //尾部的时间戳

                webRequest.ContentLength = totalLength;

                Stream postStream = webRequest.GetRequestStream();

                #region 上传表单数据

                if (PostItems != null && PostItems.Count > 0)
                {
                    foreach (KeyValuePair<string, PostItem> kv in PostItems)
                    {
                        PostItem item = kv.Value;

                        string formName = item.FormName;
                        byte[] postHeaderBytes = item.PostHeaderBytes;
                        byte[] dataBytes = item.PostDataBytes;

                        //发送请求头部消息
                        postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                        //发送数据
                        postStream.Write(dataBytes, 0, dataBytes.Length);
                    }
                }

                #endregion

                #region 上传文件

                foreach (KeyValuePair<string, PostFile> uploadFileKV in PostFiles)
                {
                    PostFile uploadFile = uploadFileKV.Value;

                    string formName = uploadFile.FormName;
                    string fileName = uploadFile.FileName;
                    Stream fs = uploadFile.FileStream;
                    BinaryReader r = new BinaryReader(fs);
                    byte[] postHeaderBytes = uploadFile.PostHeaderBytes;

                    //每次上传4k
                    int bufferLength = 4096;
                    byte[] buffer = new byte[bufferLength];

                    //已上传的字节数
                    long offset = 0;
                    int size = r.Read(buffer, 0, bufferLength);

                    //发送请求头部消息
                    postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                    while (size > 0)
                    {
                        postStream.Write(buffer, 0, size);
                        offset += size;

                        size = r.Read(buffer, 0, bufferLength);
                    }

                    fs.Close();
                    r.Close();
                }

                #endregion

                //添加尾部的时间戳
                postStream.Write(trailerBoundaryBytes, 0, trailerBoundaryBytes.Length);
                postStream.Close();

                #endregion
            }
            else
            {
                if (PostItems != null && PostItems.Count > 0 && string.Compare(webRequest.Method, WebRequestMethods.Http.Post, true) == 0)
                {
                    #region POST

                    webRequest.ContentType = POST_CONTENT_TYPE_URLENCODED;

                    string param = string.Empty;
                    foreach (KeyValuePair<string, PostItem> kv in PostItems)
                    {
                        PostItem item = kv.Value;
                        param += string.Concat(item.FormName, "=", WebHelper.UrlEncode(item.Data), "&");
                    }
                    if (!string.IsNullOrEmpty(param))
                    {
                        param = param.Substring(0, param.Length - 1);
                    }
                    byte[] bs = this.Encoding.GetBytes(param);
                    using (Stream reqStream = webRequest.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                    }

                    #endregion
                }
                else
                {
                    webRequest.Method = WebRequestMethods.Http.Get;
                }
            }

            if (CookieContainer != null)
            {
                webRequest.CookieContainer = new CookieContainer();
            }

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            string content;
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                using (StreamReader responseReader = new StreamReader(responseStream, Encoding))
                {
                    content = responseReader.ReadToEnd();
                }
            }
            webResponse.Close();

            return content;
        }

        /// <summary>
        /// 发送body数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SendBodyData(byte[] data)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.UseDefaultCredentials = true;
            if (!string.IsNullOrEmpty(ContentType))
            {
                webRequest.ContentType = ContentType;
            }
            if (!string.IsNullOrEmpty(UserAgent))
            {
                webRequest.UserAgent = UserAgent;
            }
            if (!string.IsNullOrEmpty(Referer))
            {
                webRequest.Referer = Referer;
            }
            if (!string.IsNullOrEmpty(Method))
            {
                webRequest.Method = Method;
            }
            webRequest.Timeout = Timeout;

            if (CookieContainer != null)
            {
                webRequest.CookieContainer = new CookieContainer();
            }

            using (Stream postStream = webRequest.GetRequestStream())
            {
                postStream.Write(data, 0, data.Length);
            }

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            string content;
            using (Stream responseStream = webResponse.GetResponseStream())
            {
                using (StreamReader responseReader = new StreamReader(responseStream, Encoding))
                {
                    content = responseReader.ReadToEnd();
                }
            }
            webResponse.Close();

            return content;
        }

    }
}

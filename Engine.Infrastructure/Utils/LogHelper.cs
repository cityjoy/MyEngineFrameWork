using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Web;

namespace  Engine.Infrastructure.Utils
{
    /// <summary>
    /// 日志文件名命名规则
    /// </summary>
    public enum LogFileNameType
    {
        /// <summary>
        /// 按天命名
        /// </summary>
        Day = 1,
        /// <summary>
        /// 按小时
        /// </summary>
        Hour = 2,
        /// <summary>
        /// 按月命名
        /// </summary>
        Month = 3,
        /// <summary>
        /// 按年命名
        /// </summary>
        Year = 4
    }


    /// <summary>
    /// 错误日志及其它日志的操作助手
    /// </summary>
    /// <remarks>
    
    public sealed class LogHelper
    {
        private static object locker = new object();

        private static string _logFileDirectoryPath;
        /// <summary>
        /// 指定日志文件目录路径
        /// </summary>
        public static string LogFileDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(_logFileDirectoryPath))
                {
                    _logFileDirectoryPath = ConfigurationManager.AppSettings["LogDirectoryPath"];
                    if (string.IsNullOrEmpty(_logFileDirectoryPath))
                        _logFileDirectoryPath = "Logs\\";

                    _logFileDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _logFileDirectoryPath);
                }
                return _logFileDirectoryPath;
            }
            set { _logFileDirectoryPath = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        static LogHelper()
        {
            if (!Directory.Exists(LogFileDirectoryPath))
                Directory.CreateDirectory(LogFileDirectoryPath);
        }

        private static LogFileNameType? _logFileNameType;
        /// <summary>
        /// 错误日志文件名命名方式
        /// </summary>
        public static LogFileNameType LogFileNameType
        {
            get
            {
                if (_logFileNameType == null)
                {
                    _logFileNameType = StringHelper.TryParse<LogFileNameType>(ConfigurationManager.AppSettings["LogFileNameType"], LogFileNameType.Hour);
                }
                return _logFileNameType.Value;
            }
        }

        /// <summary>
        /// 写入出错日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>新日志唯一编号</returns>
        public static string WriteLog(Exception ex)
        {
            return WriteLog(ex, string.Empty, LogFileDirectoryPath);
        }
        /// <summary>
        /// 写入出错日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="errorMessage">自定义错误消息</param>
        /// <returns>新日志唯一编号</returns>
        public static string WriteLog(Exception ex, string errorMessage)
        {
            return WriteLog(ex, errorMessage, LogFileDirectoryPath);
        }
        /// <summary>
        /// 写入出错日志
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="errorMessage">自定义错误消息</param>
        /// <param name="logDirPath">日志保存目录路径</param>
        /// <returns>新日志唯一编号</returns>
        public static string WriteLog(Exception ex, string errorMessage, string logDirPath)
        {
            try
            {
                if (ex == null) { return string.Empty; }

                if (ex is HttpException)
                {
                    if (((HttpException)ex).GetHttpCode() == 404)
                        return string.Empty;
                }
                //专门用于处理 Windows Vista 上 IIS 7.0 的一个BUG导致的异常
                else if (ex is NullReferenceException)
                {
                    if (((NullReferenceException)ex).TargetSite.DeclaringType.FullName == "System.Web.Hosting.IIS7WorkerRequest")
                        return string.Empty;
                }

                lock (locker)
                {

                    string errorFilePath;
                    switch (LogFileNameType)
                    {
                        case LogFileNameType.Day:
                            errorFilePath = Path.Combine(logDirPath, "error-log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".config");
                            break;
                        case LogFileNameType.Hour:
                        default:
                            errorFilePath = Path.Combine(logDirPath, "error-log-" + DateTime.Now.ToString("yyyy-MM-dd--HH") + ".config");
                            break;
                        case LogFileNameType.Month:
                            errorFilePath = Path.Combine(logDirPath, "error-log-" + DateTime.Now.ToString("yyyy-MM") + ".config");
                            break;
                        case LogFileNameType.Year:
                            errorFilePath = Path.Combine(logDirPath, "error-log-" + DateTime.Now.ToString("yyyy") + ".config");
                            break;
                    }

                    string errorID = Guid.NewGuid().ToString("N").ToUpper();

                    if (!Directory.Exists(logDirPath))
                    {
                        Directory.CreateDirectory(logDirPath);
                    }
                    using (StreamWriter streamWriter = new StreamWriter(errorFilePath, true, System.Text.Encoding.UTF8))
                    {
                        streamWriter.WriteLine("错误编号:" + errorID);
                        streamWriter.WriteLine("错误消息:" + ex.Message);
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            streamWriter.WriteLine("附加消息:" + errorMessage);
                        }
                        streamWriter.WriteLine("-------------------");

                        if (System.Web.HttpContext.Current != null)
                        {
                            System.Web.HttpRequest request = System.Web.HttpContext.Current.Request;

                            string userVar = request.RawUrl;
                            if (ex != null)
                            {
                                streamWriter.WriteLine("请求的原始地址:" + userVar);
                            }

                            if (request.UrlReferrer != null)
                            {
                                userVar = request.UrlReferrer.ToString();
                                streamWriter.WriteLine("请求来自:" + userVar);
                            }

                            userVar = System.Web.HttpContext.Current.Request.UserAgent;
                            streamWriter.WriteLine("客户端:" + userVar);

                            userVar = request.UserHostAddress;
                            streamWriter.WriteLine("IP:" + userVar);

                            if (request.Cookies.Count > 0)
                                streamWriter.WriteLine("Cookie:");
                            for (int i = 0; i < request.Cookies.Count; i++)
                            {
                                System.Web.HttpCookie cookie = request.Cookies[i];
                                streamWriter.WriteLine("    name:" + cookie.Name + "    value:" + cookie.Value);
                            }

                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < HttpContext.Current.Request.Form.Count; i++)
                            {
                                if (i > 0)
                                    sb.Append("&");
                                sb.Append(request.Form.GetKey(i));
                                sb.Append("=");
                                sb.Append(request.Form[i].ToString());

                            }
                            streamWriter.WriteLine("PostData:" + sb.ToString());
                        }
                        streamWriter.WriteLine("------------------------------------------------------");

                        streamWriter.WriteLine("Source:" + ex.Source);
                        streamWriter.WriteLine("StackTrace:");
                        streamWriter.WriteLine(ex.StackTrace);
                        streamWriter.WriteLine("Method:" + ex.TargetSite.Name);
                        streamWriter.WriteLine("Class:" + ex.TargetSite.DeclaringType.FullName);
                        streamWriter.WriteLine("Time:" + DateTime.Now.ToString());
                        streamWriter.WriteLine("********************************************************************************************");
                        streamWriter.WriteLine(string.Empty);
                        streamWriter.WriteLine(string.Empty);
                    }

                    return errorID;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void WriteLog(string message)
        {
            WriteLog(message, LogFileDirectoryPath);
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="logDirPath"></param>
        /// <returns></returns>
        public static void WriteLog(string message, string logDirPath)
        {
            try
            {
                lock (locker)
                {
                    string logFilePath;
                    switch (LogFileNameType)
                    {
                        case LogFileNameType.Day:
                            logFilePath = Path.Combine(logDirPath, "log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".config");
                            break;
                        case LogFileNameType.Hour:
                        default:
                            logFilePath = Path.Combine(logDirPath, "log-" + DateTime.Now.ToString("yyyy-MM-dd--HH") + ".config");
                            break;
                        case LogFileNameType.Month:
                            logFilePath = Path.Combine(logDirPath, "log-" + DateTime.Now.ToString("yyyy-MM") + ".config");
                            break;
                        case LogFileNameType.Year:
                            logFilePath = Path.Combine(logDirPath, "log-" + DateTime.Now.ToString("yyyy") + ".config");
                            break;
                    }

                    if (!Directory.Exists(logDirPath))
                    {
                        Directory.CreateDirectory(logDirPath);
                    }
                    using (StreamWriter streamWriter = new StreamWriter(logFilePath, true, System.Text.Encoding.UTF8))
                    {
                        streamWriter.WriteLine(message);
                        streamWriter.WriteLine("********************************************************************************************");
                        streamWriter.WriteLine(string.Empty);
                        streamWriter.WriteLine(string.Empty);
                    }
                }
            }
            catch
            {

            }
        }
    }
}


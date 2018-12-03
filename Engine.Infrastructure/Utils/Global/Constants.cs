using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
namespace   Engine.Infrastructure.Utils

{
    /// <summary>
    /// 系统使用的一些常量
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// ASP.NET_SessionId COOKIE键名
        /// </summary>
        public const string ASP_NET_SESSIONID_KEY = "Engine.NET_SessionId";

        public const double SESSION_EXPIRED_SECONDS = 3600;

        public const string SESSION_KEY_CURRENT_USER = "CURRENT_USER";
        /// <summary>
        /// 用户授权访问令牌COOKIE键值
        /// </summary>
        public const string COOKIEKEY_USER_ACCESS_TOKEN = "USER_ACCESS_TOKEN";

        /// <summary>
        /// 登录用户COOKIE信息分隔符
        /// </summary>
        public const string SEPARATOR_LOGIN_USER_COOKIE = "|||";

        /// <summary>
        /// COOKIE共享域名
        /// </summary>
        public readonly static string COOKIE_DOMAIN = ConfigurationManager.AppSettings["COOKIE_DOMAIN"];
        /// <summary>
        /// 主站 
        /// </summary>
        public readonly static string MAIN_SITE = ConfigurationManager.AppSettings["MAIN_SITE"];

        /// <summary>
        /// API站 
        /// </summary>
        public readonly static string API_SITE = ConfigurationManager.AppSettings["API_SITE"];

        /// <summary>
        /// 文件上传存储站 
        /// </summary>
        public readonly static string FILESTORE_SITE = ConfigurationManager.AppSettings["FILESTORE_SITE"];
        #region 邮件服务配置
        /// <summary>
        /// 邮件服务主机
        /// </summary>
        public readonly static string EMAIL_SERV_HOST = "XXXX.163.com";
        /// <summary>
        /// 邮件服务端口
        /// </summary>
        public readonly static int EMAIL_SERV_PORT = -1;
        /// <summary>
        /// 邮件服务账号
        /// </summary>
        public readonly static string EMAIL_SERV_ACCOUNT = "XXXX@163.com";
        /// <summary>
        /// 邮件服务账号密码
        /// </summary>
        public readonly static string EMAIL_SERV_PASSWORD = "XXXX";
        /// <summary>
        /// 发送邮件使用的邮箱地址
        /// </summary>
        public readonly static string EMAIL_FROM_ACCOUNT = "XXXX@163.com";
        /// <summary>
        /// 发送邮件使用的发送者名称
        /// </summary>
        public readonly static string EMAIL_SENDER_NAME = "5XXXX";
        #endregion

       

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 数据库配置
    /// </summary>
    public class SqlConfig
    {

        static SqlConfig()
        {
            LoadDbConnectionString();
        }

        /// <summary>
        /// 主库
        /// </summary>
        internal static string MainDbConnectionString
        {
            get;
            set;
        }
        /// <summary>
        /// 各种日志库
        /// </summary>
        internal static string LogDbConnectionString
        {
            get;
            set;
        }
        /// <summary>
        /// 后台库
        /// </summary>
        internal static string AuthorityDbConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// 加载连接字符串配置
        /// </summary>
        private static void LoadDbConnectionString()
        {
            string stapleConnStr = ConfigurationManager.AppSettings["MainDbConnectionString"];
            MainDbConnectionString = stapleConnStr;

            string logConnStr = ConfigurationManager.AppSettings["LogDbConnectionString"];
            LogDbConnectionString = logConnStr;

            string adminConnStr = ConfigurationManager.AppSettings["AuthorityDbConnectionString"];
            AuthorityDbConnectionString = adminConnStr;

        }
    }
}

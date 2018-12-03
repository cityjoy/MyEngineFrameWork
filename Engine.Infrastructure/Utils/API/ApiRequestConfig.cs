using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// Api配置
    /// </summary>
    /// <remarks>

    public class ApiRequestConfig
    {
        /// <summary>
        /// 接口身份验证方式的安全密钥
        /// </summary>
        public static string SecretKey
        {
            get;
            set;
        }
    }

}

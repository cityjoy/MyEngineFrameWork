using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Application.Model
{
    /// <summary>
    /// 用户登录视图模型
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string LoginAccount { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string LoginPassword { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string UserIp { get; set; }
        /// <summary>
        /// 客户端代理
        /// </summary>
        public string UserAgent { get; set; }

    }

}

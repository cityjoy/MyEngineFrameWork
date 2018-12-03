using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Engine.Domain.Entity;

namespace Engine.Application.Model
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public class UserAccessToken
    {
        /// <summary>
        /// 授权的访问令牌
        /// </summary>
        public string AuthToken
        {
            get;
            set;
        }

        /// <summary>
        /// 访问令牌过期时间(然后必须请求刷新令牌接口)
        /// </summary>
        public DateTime ExpiredTime
        {
            get;
            set;
        }

        /// <summary>
        /// 访问令牌失效时间(然后必须请求重新授权接口)
        /// </summary>
        public DateTime InvalidTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public User AuthUser
        {
            get;
            set;
        }
        /// <summary>
        ///  附加数据
        /// </summary>
        public object Data { get; set; }
       
        /// <summary>
        ///   提示信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        ///  结果值 大于等于1:成功,小于等于0:失败(应用的业务逻辑可自定义其接口的返回值含义)
        /// </summary>
        public int? Result { get; set; }

    }
}

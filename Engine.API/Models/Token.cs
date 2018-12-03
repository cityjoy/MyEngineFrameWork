using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engine.API.Model
{
    /// <summary>
    /// Token
    /// </summary>
    public class Token
    { 
        /// <summary>
        /// 用户AppId
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 用户名对应签名Token
        /// </summary>
        public Guid SignToken { get; set; }


        /// <summary>
        /// Token过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }
}
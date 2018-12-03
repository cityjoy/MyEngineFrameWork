using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Domain.Model
{
    /// <summary>
    /// 表示jwt的payload
    /// </summary>
    public class JWTAuthInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 角色列表，可以用于记录该用户的角色
        /// </summary>
        public List<string> Roles { get; set; }
        /// <summary>
        /// 访问令牌失效时间(然后必须请求重新授权接口)
        /// </summary>
        public long InvalidTime
        {
            get;
            set;
        }
        /// <summary>
        /// 访问令牌过期时间
        /// </summary>

        public long ExpiredTime
        {
            get;
            set;
        }
    }
}

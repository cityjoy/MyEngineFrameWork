using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.API.Model
{
    /// <summary>
    /// 异常信息结果消息类
    /// </summary>
    public class ResultMsg
    {
        /// <summary>
        /// 请求是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
       
    }
}

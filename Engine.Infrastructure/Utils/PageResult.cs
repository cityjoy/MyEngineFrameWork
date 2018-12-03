using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Engine.Infrastructure.Utils
{
    /// <summary>
    /// 页面操作结果
    /// </summary>
    public enum PageResultType : int
    {
        /// <summary>
        /// 无操作
        /// </summary>
        Default = 0,
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 操作失败
        /// </summary>
        Failed = -1
    }

    /// <summary>
    /// 页面操作结果
    /// </summary>
    public class PageResult
    {
        /// <summary>
        /// 默认实例化
        /// </summary>
        public PageResult()
        {

        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="resultType"></param>
        public PageResult(PageResultType resultType)
        {
            this.Result = resultType;
            this.ErrorCode = 0;
            this.Message = string.Empty;
            this.Keywords = string.Empty;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="data"></param>
        public PageResult(PageResultType resultType, object data)
        {
            this.Result = resultType;
            this.ErrorCode = 0;
            this.Message = string.Empty;
            this.Keywords = string.Empty;
            this.Data = data;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public PageResult(PageResultType resultType, string message, int errorCode)
        {
            this.Result = resultType;
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Keywords = string.Empty;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="keywords"></param>
        public PageResult(PageResultType resultType, string message, int errorCode, string keywords)
        {
            this.Result = resultType;
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Keywords = keywords;
        }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="resultType"></param>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="keywords"></param>
        /// <param name="data"></param>
        public PageResult(PageResultType resultType, string message, int errorCode, string keywords, object data)
        {
            this.Result = resultType;
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Keywords = keywords;
            this.Data = data;
        }

        /// <summary>
        /// 结果
        /// </summary>

        public virtual PageResultType Result
        {
            get;
            set;
        }

        /// <summary>
        /// 信息
        /// </summary>

        public virtual string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 错误代码
        /// </summary>

        public virtual int ErrorCode
        {
            get;
            set;
        }

        /// <summary>
        /// 关键词
        /// </summary>

        public virtual string Keywords
        {
            get;
            set;
        }

        /// <summary>
        /// 数据
        /// </summary>

        public virtual object Data
        {
            get;
            set;
        }

    }
}

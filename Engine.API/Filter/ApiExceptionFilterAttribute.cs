using Engine.API.Common;
using Engine.API.Model;
using Engine.Domain.Enums;
using Engine.Domain.Model;
using Engine.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Engine.API.Filter
{
    /// <summary>
    /// API异常过滤器特性
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
       /// <summary>
        /// 统一对调用异常信息进行处理，返回自定义的异常信息
        /// </summary>
        /// <param name="actionExecutedContext">HTTP上下文对象</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            ResultMsg resultMsg = null;
            //自定义异常的处理
            if (actionExecutedContext.Exception is NotImplementedException)
            {
               
                resultMsg = new ResultMsg();
                resultMsg.Success = false;
                resultMsg.StatusCode = (int)StatusCodeEnum.NotImplemented;
                resultMsg.Info = StatusCodeEnum.NotImplemented.GetEnumText();
                resultMsg.Data = "NotImplementedException";
                actionExecutedContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);
            }
            else if (actionExecutedContext.Exception is TimeoutException)
            {
                resultMsg = new ResultMsg();
                resultMsg.Success = false;
                resultMsg.StatusCode = (int)StatusCodeEnum.RequestTimeout;
                resultMsg.Info = StatusCodeEnum.RequestTimeout.GetEnumText();
                resultMsg.Data = "TimeoutException";
                actionExecutedContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);
            }
            //.....这里可以根据项目需要返回到客户端特定的状态码。如果找不到相应的异常，统一返回服务端错误500
            else
            {
                resultMsg = new ResultMsg();
                resultMsg.Success = false;
                resultMsg.StatusCode = (int)StatusCodeEnum.InternalServerError;
                resultMsg.Info = StatusCodeEnum.InternalServerError.GetEnumText();
                resultMsg.Data = "InternalServerErrorException";
                actionExecutedContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);

            }
            //记录关键的异常信息
            LogHelper.WriteLog("请求异常：" + actionExecutedContext.Exception.ToString());
            //Debug.WriteLine(context.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}
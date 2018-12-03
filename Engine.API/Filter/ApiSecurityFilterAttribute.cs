
using Engine.API.Model;
using Client.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Engine.Infrastructure.Utils;
using Engine.Domain.Enums;
using Engine.Domain.Model;
using Engine.API.Common;

namespace Engine.API.Filter
{
    /// <summary>
    /// api安全过滤器特性
    /// </summary>
    public class ApiSecurityFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 校验客户端请求的签名
        /// </summary>
        /// <param name="actionContext">HTTP上下文对象</param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            ResultMsg resultMsg = null;
            var request = actionContext.Request;
            string method = request.Method.Method;

            #region 校验签名
            string signature = string.Empty;
            string timestamp = string.Empty;
            string nonce = string.Empty;
            if (actionContext.Request.Headers.Contains("signature"))
            {
                signature = actionContext.Request.Headers.GetValues("signature").FirstOrDefault();
            }
            if (actionContext.Request.Headers.Contains("timestamp"))
            {
                timestamp = actionContext.Request.Headers.GetValues("timestamp").FirstOrDefault();
            }

            if (actionContext.Request.Headers.Contains("nonce"))
            {
                nonce = actionContext.Request.Headers.GetValues("nonce").FirstOrDefault();
            }
            if (string.IsNullOrEmpty(signature))
            {
                resultMsg = new ResultMsg();
                resultMsg.StatusCode = (int)StatusCodeEnum.Unauthorized;
                resultMsg.Info = StatusCodeEnum.ParameterError.GetEnumText();
                resultMsg.Data = "invalid signature";
                actionContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);
                base.OnActionExecuting(actionContext);
                return;

            }

            string key = "!cybd*2017$";
            if (key == null)
            {
                resultMsg = new ResultMsg();
                resultMsg.Success = false;
                resultMsg.StatusCode = (int)StatusCodeEnum.Unauthorized;
                resultMsg.Info = StatusCodeEnum.Unauthorized.GetEnumText();
                resultMsg.Data = "invalid api secret key!";
                actionContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);
                base.OnActionExecuting(actionContext);
                return;
            }

            string userSignature = ApiRequest.BuildSignature(key, ConvertHelper.ToInt64(timestamp, 0), nonce);

            if (userSignature == signature)
            {
                long iTimestamp = ConvertHelper.ToInt64(timestamp);
                long nowTimestamp = DateTimeHelper.ConvertToUnixTimestamp(DateTime.Now);

                if (nowTimestamp - iTimestamp > 600)
                {
                    resultMsg = new ResultMsg();
                    resultMsg.Success = false;
                    resultMsg.StatusCode = (int)StatusCodeEnum.Unauthorized;
                    resultMsg.Info = StatusCodeEnum.Unauthorized.GetEnumText();
                    resultMsg.Data = "signature expired!";
                    actionContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);
                    base.OnActionExecuting(actionContext);
                    return;
                }
                else if (iTimestamp > nowTimestamp)
                {
                    resultMsg = new ResultMsg();
                    resultMsg.Success = false;
                    resultMsg.StatusCode = (int)StatusCodeEnum.Unauthorized;
                    resultMsg.Info = StatusCodeEnum.Unauthorized.GetEnumText();
                    resultMsg.Data = "invalid timestamp!";
                    actionContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);
                    base.OnActionExecuting(actionContext);
                    return;
                }

            }
            else
            {
                resultMsg = new ResultMsg();
                resultMsg.Success = false;
                resultMsg.StatusCode = (int)StatusCodeEnum.Unauthorized;
                resultMsg.Info = StatusCodeEnum.Unauthorized.GetEnumText();
                resultMsg.Data = "invalid signature";
                actionContext.Response = HttpResponseExtension.ToResponseMessage(resultMsg);
                base.OnActionExecuting(actionContext);
                return;

            }
            #endregion


        }
    }
}

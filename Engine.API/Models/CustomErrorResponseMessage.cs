using Engine.API.Common;
using Engine.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Engine.Domain.Enums;
using Engine.Domain.Model;

namespace Engine.API.Models
{
    /// <summary>
    /// API自定义错误消息处理委托类。
    /// 用于处理访问不到对应API地址的情况，对错误进行自定义操作。
    /// </summary>
    public class ErrorMessageDelegatingHandler : DelegatingHandler
    {
       /// <summary>
        /// 以异步操作发送 HTTP 请求到内部管理器以发送到服务器。
       /// </summary>
       /// <param name="request"></param>
       /// <param name="cancellationToken"></param>
       /// <returns></returns>
 
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((responseToCompleteTask) =>
            {
                HttpResponseMessage response = responseToCompleteTask.Result;
                HttpError error = null;
                ResultMsg resultMsg = null;
                if (response.TryGetContentValue<HttpError>(out error))
                {
                  
                    //添加自定义错误处理
                }

                if (error != null)
                {
                    //获取抛出自定义异常，拦截器统一解析
                    
                    resultMsg = new ResultMsg();
                    resultMsg.StatusCode = (int)StatusCodeEnum.NotFound;
                    resultMsg.Info = StatusCodeEnum.NotFound.GetEnumText();
                    resultMsg.Data = "NotFound";

                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        //封装处理异常信息，返回指定JSON对象
                        Content = new StringContent(JsonConvert.SerializeObject(resultMsg)),
                        ReasonPhrase = "Exception"
                    });
                }
                else
                {
                    return response;
                }
            });
        }
    }
}
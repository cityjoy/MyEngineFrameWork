using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Http.Filters;
using System.Linq;
using Engine.API.Model;
using Engine.API.Common;
using Newtonsoft.Json;
using Engine.Domain.Entity;
using Engine.Doman.Cache;
using System.Web.Http;
using System.Net.Http;
using Engine.Infrastructure.Utils;
using Engine.Domain.Enums;
using Engine.Domain.Model;
namespace Engine.API.Filter
{
    /// <summary>
    /// 用户身份授权令牌过滤器特性
    /// </summary>

    public class ApiAuthUserFilterAttribute : ActionFilterAttribute
    {
       
        /// <summary>
        /// 校验用户身份授权令牌，若无传递合法令牌，则返回相应的错误信息
        /// </summary>
        /// <param name="actionContext">HTTP上下文对象</param>
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            PageResult resultMsg = null;
            try
            {
                User currentUser = HttpContext.Current.Items["CurrentUser"] as User;
                if (currentUser == null)
                {
                    resultMsg = new PageResult();
                    resultMsg.Result = PageResultType.Failed;
                    resultMsg.Data = StatusCodeEnum.Unauthorized.GetEnumText();
                    actionContext.Response = new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(resultMsg)),
                        StatusCode = System.Net.HttpStatusCode.Unauthorized
                    };

                }
            }
            catch (Exception ex)
            {
                resultMsg = new PageResult();
                resultMsg.Result = PageResultType.Failed;
                resultMsg.Data = "用户身份授权令牌无效";
                actionContext.Response = new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(resultMsg)),
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                };
                LogHelper.WriteLog("校验用户身份授权令牌异常" + ex.ToString());

            }

            base.OnActionExecuting(actionContext);
        }


    }
}

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
using System.Configuration;
using Engine.Infrastructure.Utils;
using Engine.Infrastructure.Data;
using Engine.Domain.IRepository;
using Engine.Application;
using Engine.Infrastructure.Repository;
using Engine.Doman.Cache;
using Engine.Application.Model;
using System.Web.Http;
using System.Net.Http;
using Engine.Domain.Model;
using Engine.Domain.Enums;
namespace Engine.API.Filter
{
    /// <summary>
    /// 年级管理员用户身份授权令牌过滤器特性
    /// </summary>

    public class ApiAuthAdminFilterAttribute : ActionFilterAttribute
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
                if (currentUser != null)
                {

                    if (currentUser.UserType != UserType.Admin )
                    {
                        resultMsg = new PageResult();
                        resultMsg.Result = PageResultType.Failed;
                        resultMsg.Data = "没有权限" + StatusCodeEnum.NotImplemented.GetEnumText();
                        actionContext.Response = new HttpResponseMessage()
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(resultMsg)),
                            StatusCode = System.Net.HttpStatusCode.Forbidden
                        };
                    }
                }
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

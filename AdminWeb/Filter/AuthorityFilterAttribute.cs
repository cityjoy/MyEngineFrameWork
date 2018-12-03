using Engine.Domain.Entity;
using Engine.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace Engine.AdminWeb.Filter
{
    /// <summary>
    /// 租户管理系统权限过滤器
    /// </summary>

    public class AuthorityFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool isJson = filterContext.HttpContext.Request.IsAjaxRequest();

            #region 登录判断

            //User loginUser = WebSession.Current.Get<User>(Constants.SESSION_KEY_CURRENT_USER);
            User loginUser = HttpContext.Current.Items["CurrentUser"] as User;
            if (loginUser == null || loginUser.Id <= 0)
            {
                if (isJson)
                {
                    JsonResult jsonResult = new JsonResult();
                    jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    jsonResult.ContentEncoding = Encoding.UTF8;
                    jsonResult.Data = new PageResult() { Result = PageResultType.Failed, ErrorCode = 403, Message = "您未登录！" };
                    filterContext.Result = jsonResult;
                    return;
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Login/Index");
                    return;
                }
            }

            #endregion
        }

    }
}

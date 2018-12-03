using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Engine.Domain.Entity;
using Engine.Infrastructure.Utils;
using Engine.Web.Models;
using Engine.Web.Common;
using Engine.Application.Model;
using Engine.Application;

namespace Engine.Web.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUserServer userServer)
            : base(userServer)
        {
        }
        public ActionResult Index()
        {

            return View();
        }

        

        /// <summary>
        /// 请求登录
        /// </summary>
        /// <returns></returns>
       [HttpPost]
        public JsonResult QueryLogin(LoginViewModel model)
        {
            OperateResult = new PageResult();
            
            string returnUrl = "";
            model.LoginPassword = SecurityHelper.DESCryptoEncode(model.LoginPassword).ToLower();
            model.UserIp = Request.UserHostAddress;
            model.UserAgent = Request.UserAgent;
            #region 本地调用 示例
            //UserAccessToken accessToken = userServer.GetJsonWebToken(model);
            #endregion
            #region 调用API方法 示例
            RestApiClient = new Infrastructure.Utils.RestApiClient(Constants.API_SITE);
            UserAccessToken accessToken = RestApiClient.Post<UserAccessToken>("/Users/GetJsonWebToken", model );
            #endregion
            if (accessToken.Result > 0)
            {
                WebHelper.SetCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN, accessToken.AuthToken, Constants.COOKIE_DOMAIN, DateTime.Now.AddDays(30));
                Session.Set<User>(Constants.SESSION_KEY_CURRENT_USER, accessToken.AuthUser);
                OperateResult.Data = new { ReturnUrl = returnUrl };
                OperateResult.Result = PageResultType.Success;
                OperateResult.Message = "登录成功";
            }
            else
            {
                OperateResult.Data = new { ReturnUrl = returnUrl };
                OperateResult.Result = PageResultType.Failed;
                OperateResult.ErrorCode = accessToken.Result.Value;
                OperateResult.Message = accessToken.Message;
            }

            return Json(OperateResult);
        }

        /// <summary>
        /// 请求登出
        /// </summary>
        /// <returns></returns>
       public JsonResult QueryLogout()
        {
            string returnUrl = Constants.MAIN_SITE;
            OperateResult = new PageResult();
            if (CurrentUser != null)
            {
                OperateResult.Data = new { ReturnUrl = returnUrl };

                // OperateResult.Data = CurrentUser.School.CodeNumber;

                WebHelper.RemoveCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN, Constants.COOKIE_DOMAIN);
                WebHelper.RemoveCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN);
                Session.Remove(Constants.SESSION_KEY_CURRENT_USER);
                OperateResult.Result = PageResultType.Success;
                OperateResult.Message = "成功退出登录";
            }
            else
            {
                OperateResult.Result = PageResultType.Failed;
                OperateResult.ErrorCode = -50;
            }
            return Json(OperateResult, JsonRequestBehavior.AllowGet);
        }

        
    }
}
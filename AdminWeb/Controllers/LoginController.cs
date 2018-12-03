using Engine.AdminWeb.Models;
using Engine.Application;
using Engine.Application.Model;
using Engine.Domain.Entity;
using Engine.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace Engine.AdminWeb.Controllers
{
    public class LoginController : BaseController
    {
        public LoginController(IUserServer userServer)
            : base(userServer)
        {

        }
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 请求登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QueryLogin(LoginViewModel model)
        {
            OperateResult = new PageResult();
            model.LoginPassword = SecurityHelper.DESCryptoEncode(model.LoginPassword).ToLower();
            model.UserIp = Request.UserHostAddress;
            model.UserAgent = Request.UserAgent;
            UserAccessToken accessToken = userServer.GetJsonWebToken(model);
            #region 调用API方法 示例
            //UserAccessToken accessToken  = RestApiClient.Get<UserAccessToken>("/Account/GrantUserAccessToken"  new { model = model });
            #endregion
            if (accessToken.Result > 0)
            {
                WebHelper.SetCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN, accessToken.AuthToken, Constants.COOKIE_DOMAIN, DateTime.Now.AddDays(30));
                OperateResult.Result = PageResultType.Success;
                OperateResult.Message = "登录成功";
            }
            else
            {
                OperateResult.Result = PageResultType.Failed;
                OperateResult.ErrorCode = accessToken.Result.Value;
                OperateResult.Message = accessToken.Message;
            }

            return Json(OperateResult);
        }

        public JsonResult QueryLogout()
        {

            OperateResult = new PageResult();
            WebHelper.RemoveCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN, Constants.COOKIE_DOMAIN);
            OperateResult.Result = PageResultType.Success;
            OperateResult.Message = "退出成功";
            return Json(OperateResult);
        }


    }
}
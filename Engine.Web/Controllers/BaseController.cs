
using Engine.Web.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Engine.Web.Models;
using Engine.Infrastructure.Utils;
using Engine.Domain.Entity;
using Engine.Application;
using Engine.Application.Model;

namespace Engine.Web.Controllers
{
       
    public class BaseController : Controller
    {

       protected IUserServer userServer;
        public BaseController(IUserServer userServer)
        {
            this.userServer = userServer;
        }
        #region Properties

        /// <summary>
        /// API客户端
        /// </summary>
       protected RestApiClient RestApiClient
        {
            get;
            set;
        }

        /// <summary>
        /// 操作结果，所有JsonResult均以此结果返回
        /// </summary>
        protected PageResult OperateResult
        {
            get;
            set;
        }

        /// <summary>
        /// 当前登陆的用户
        /// </summary>
        protected User CurrentUser
        {
            get;
            set;
        }

        /// <summary>
        /// 重写的Session
        /// </summary>
        protected new WebSession Session
        {
            get;
            private set;
        }
      
        #endregion Properties
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this.Session = WebSession.Current;
            int status;
            string accessToken = "";
            UserAccessToken refreshAccessToken = null;
            accessToken = Request["__UserAccessToken"]; //flash上传的cookie bug,用此方式解决
            if (string.IsNullOrEmpty(accessToken))
            {
                HttpCookie accessTokenCookie = WebHelper.GetCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN);
                if (accessTokenCookie != null)
                {
                    accessToken = accessTokenCookie.Value;
                }
            }
            if (!string.IsNullOrEmpty(accessToken))
            {
                CurrentUser = userServer.GetSessionUserFromJsonWebToken(accessToken, out status, out refreshAccessToken);
                if (refreshAccessToken != null)
                {
                    WebHelper.SetCookie(Constants.COOKIEKEY_USER_ACCESS_TOKEN, refreshAccessToken.AuthToken, Constants.COOKIE_DOMAIN, refreshAccessToken.InvalidTime);

                }
            }
            
            ViewBag.CurrentUser = CurrentUser;
            ViewBag.IsLogin = CurrentUser != null && CurrentUser.Id > 0;
        }


    }
}
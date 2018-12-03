using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Configuration;

using Engine.API.Filter;
using Engine.Infrastructure.Utils;
using Engine.Domain.Entity;
using Engine.Doman.Cache;
using Engine.Application;
using System.Net.Http;
using System.Net.Http.Headers;
using Engine.Application.Model;
namespace Engine.API.Controllers
{
    /// <summary>
    /// API接口基类控制器
    /// </summary>
    [ApiExceptionFilter]
    public class BaseController : ApiController
    {
        protected IUserServer userServer;
        public BaseController(IUserServer userServer)
        {
            this.userServer = userServer;
            //var s = this.User;
        }
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public User CurrentUser
        {
            get;
            set;
        }
        /// <summary>
        /// 客户端会话
        /// </summary>
        protected WebSession Session
        {
            get;
            set;
        }
        /// <summary>
        /// 刷新后的令牌
        /// </summary>
        protected string NewToken
        {
            get;
            set;
        }
 
        /// <summary>
        /// 客户端IP
        /// </summary>
        protected string UserIp
        {
            get;
            set;
        }
        /// <summary>
        /// 客户端浏览器代理
        /// </summary>
        protected string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// 控制器初始化
        /// </summary>
        /// <param name="controllerContext"></param>
        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            //初始化时会自动根据头部数据中的授权令牌获取相应的CurrentUser，以及客户端的Session

            var RequestContext = this.RequestContext;
            if (controllerContext.Request.Headers.Contains("UserIp"))
            {
                UserIp = controllerContext.Request.Headers.GetValues("UserIp").FirstOrDefault();

            }
            if (controllerContext.Request.Headers.Contains("UserAgent"))
            {
                UserAgent = controllerContext.Request.Headers.GetValues("UserAgent").FirstOrDefault();

            }
            #region 获取客户端Session
            if (controllerContext.Request.Headers.Contains("sessionId"))
            {
                string sessionId = controllerContext.Request.Headers.GetValues("sessionId").FirstOrDefault();
                if (!string.IsNullOrEmpty(sessionId))
                {
                    Session = new WebSession(sessionId);
                }
            }
            #endregion

            #region 获取授权用户AccessToken
            string accessToken = string.Empty;
            if (controllerContext.Request.Headers.Contains("token"))
            {
                accessToken = controllerContext.Request.Headers.GetValues("token").FirstOrDefault();

            }
            if (!string.IsNullOrEmpty(accessToken))
            {
                int status = 0; 
                UserAccessToken newAccessToken = null;
                CurrentUser = userServer.GetSessionUserFromJsonWebToken(accessToken, out status, out newAccessToken);
                HttpContext.Current.Items["CurrentUser"] = CurrentUser;
                if (newAccessToken != null)
                {
                    NewToken = newAccessToken.AuthToken;
                }
                else
                {
                    NewToken = "";
                }
            }
            //if (ConfigurationManager.AppSettings["Environment"] != null && ConfigurationManager.AppSettings["Environment"] == "development")
            //{
            //    CurrentUser = UserCache.GetUserById(1);
            //    if (CurrentUser == null)
            //    {
            //        CurrentUser = userServer.GetSingle(m => m.Id == 1);
            //        UserCache.SetUserById(CurrentUser);
            //    }
            //}
            #endregion
            base.Initialize(controllerContext);
        }

    }
}

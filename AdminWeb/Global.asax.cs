using Engine.AdminWeb.Models;
using Engine.Application;
using Engine.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Engine.AdminWeb
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.RegisterMappings();
            //ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory("defaultContainer"));//不再使用
            UnityContainerManager.Initialise();//IOC容器初始化
        }
 
        protected void Session_Start(object sender, EventArgs e)
        {
            //WebSession.InitSessionId();
        }
 

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception lastError = HttpContext.Current.Server.GetLastError();
            if (lastError == null)
                return;

            Exception ex = lastError.GetBaseException();

            if (System.Web.HttpContext.Current != null)
            {
                if (ex is HttpException && ((HttpException)ex).GetHttpCode() == 404)
                {
                    //nothing
                }
                else
                {
                    LogHelper.WriteLog(ex);
                }
            }
        }
    }
}

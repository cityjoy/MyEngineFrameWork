using Engine.Web.Common;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using StackExchange.Profiling.EntityFramework6;
using System.Configuration;
using Engine.Infrastructure.Utils;
using Engine.Application;
using Engine.Web.Models;
using Engine.Infrastructure.Repository;
using System.Data.Entity;
namespace Engine.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.RegisterMappings();
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory("defaultContainer"));

#if DEBUG
            //MiniProfilerEF6.Initialize();

#endif
        }
        protected void Session_Start(object sender, EventArgs e)
        {
           
        }
        protected void Application_BeginRequest()
        {
            if (Request.IsLocal)//这里是允许本地访问启动监控
            {
#if DEBUG
                MiniProfiler.Start();
#endif


            }
        }

        protected void Application_EndRequest()
        {
#if DEBUG
            //MiniProfiler.Stop();
#endif
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

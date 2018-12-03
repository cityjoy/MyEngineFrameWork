using Engine.Infrastructure.Utils;
using Engine.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
namespace Engine.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            UnityConfig.RegisterComponents(config);
            EnableCorsAttribute corsSet = new EnableCorsAttribute( "*" ,  "*", "*");
            corsSet.SupportsCredentials = true;
            //跨域配置
            config.EnableCors(corsSet);


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

using Autofac;
using Autofac.Integration.Mvc;
using Engine.API.Common;
using Engine.Application;
using Engine.Domain.Entity;
using Engine.Domain.IRepository;
using Engine.Infrastructure.Data;
using Engine.Infrastructure.Repository;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
namespace Engine.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var builder = new ContainerBuilder();
            SetupDependencyResolver(builder);
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AutoMapperConfig.RegisterMappings();
        }

        private void SetupDependencyResolver(ContainerBuilder builder)
        {
            // Scan an assembly for components
            //注册仓储
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                  .Where(t => t.Name.StartsWith("IRepository"))
                  .AsImplementedInterfaces();
            //注册服务
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                   .Where(t => t.Name.EndsWith("Server"))
                   .AsImplementedInterfaces();
        }
        /// <summary>
        /// 使用指定的依赖关系解析程序（不再使用）
        /// </summary>
        private void InitialiseDependencyResolver()
        {
            var container = new UnityContainer();
            container.RegisterType<IEFDbContext, EFDbContext>().RegisterSingleton<EFDbContext>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IRepository<User>, EFRepository<User>>();

            //注册服务
            container.RegisterType<IUserServer, UserServer>();
            container.RegisterInstance<IFilterProvider>("FilterProvider", new UnityFilterAttributeFilterProvider(container));
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}

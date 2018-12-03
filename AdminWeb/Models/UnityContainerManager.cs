using Engine.Application;
using Engine.Domain.Entity;
using Engine.Domain.IRepository;
using Engine.Infrastructure.Data;
using Engine.Infrastructure.Repository;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Engine.AdminWeb.Models
{
    public static class UnityContainerManager
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            //注册仓储
            container.RegisterType<IEFDbContext, EFDbContext>(new PerWebRequestLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IRepository<User>, EFRepository<User>>();
           
            //注册服务
            container.RegisterType<IUserServer, UserServer>();
          
        }
    }
}
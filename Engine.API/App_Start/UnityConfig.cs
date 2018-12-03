using Engine.Application;
using Engine.Domain.Entity;
using Engine.Domain.IRepository;
using Engine.Infrastructure.Data;
using Engine.Infrastructure.Repository;
using Engine.API.Common;
using Engine.API.Filter;
using System.Web.Http;
using Unity;

namespace Engine.WebAPI
{
    public static class UnityConfig
    {
        /// <summary>
        /// ×¢²á½Ó¿Ú
        /// </summary>
        /// <param name="config"></param>
        public static void RegisterComponents(HttpConfiguration config)
        {
           var container = new UnityContainer();
            
            //×¢²á²Ö´¢
           container.RegisterType<IEFDbContext, EFDbContext>(new PerWebRequestLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IRepository<User>, EFRepository<User>>();
           
            //×¢²á·þÎñ
            container.RegisterType<IUserServer, UserServer>();
           

            
            config.DependencyResolver = new UnityResolver(container);

        }
    }
}

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
        /// ע��ӿ�
        /// </summary>
        /// <param name="config"></param>
        public static void RegisterComponents(HttpConfiguration config)
        {
           var container = new UnityContainer();
            
            //ע��ִ�
           container.RegisterType<IEFDbContext, EFDbContext>(new PerWebRequestLifetimeManager());
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IRepository<User>, EFRepository<User>>();
           
            //ע�����
            container.RegisterType<IUserServer, UserServer>();
           

            
            config.DependencyResolver = new UnityResolver(container);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Unity;

namespace Engine.API.Common
{
    public class UnityDependencyResolver : IDependencyResolver
    {
        private IUnityContainer container;
        public UnityDependencyResolver(IUnityContainer container)
        {
            this.container = container;
        }
        public object GetService(Type serviceType)
        {
            return container.Resolve(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.ResolveAll(serviceType);
        }
    }
}
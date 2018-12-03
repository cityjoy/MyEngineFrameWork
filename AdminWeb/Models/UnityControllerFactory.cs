using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Engine.AdminWeb.Models
{
    /// <summary>
    /// Unity工厂
    /// </summary>
    public class UnityControllerFactory : DefaultControllerFactory
    {
        static object syncHelper = new object();
        static Dictionary<string, IUnityContainer> containers = new Dictionary<string, IUnityContainer>();
        public IUnityContainer UnityContainer { get; private set; }
        public UnityControllerFactory(string containerName = "")
        {
            if (containers.ContainsKey(containerName))
            {
                this.UnityContainer = containers[containerName];
                return;
            }
            lock (syncHelper)
            {
                if (containers.ContainsKey(containerName))
                {
                    this.UnityContainer = containers[containerName];
                    return;
                }
                IUnityContainer container = new UnityContainer();
                UnityConfigurationSection configSection = ConfigurationManager.GetSection(UnityConfigurationSection.SectionName) as UnityConfigurationSection;
                if (null == configSection && !string.IsNullOrEmpty(containerName))
                {
                    throw new ConfigurationErrorsException("The <unity> configuration section does not exist.");
                }
                if (null != configSection)
                {
                    if (string.IsNullOrEmpty(containerName))
                    {
                        configSection.Configure(container);
                    }
                    else
                    {
                        configSection.Configure(container, containerName);
                    }
                }
                containers.Add(containerName, container);
                this.UnityContainer = containers[containerName];
            }
        }
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (null == controllerType)
            {
                return null;
            }
            return (IController)this.UnityContainer.Resolve(controllerType);
        }
        public override void ReleaseController(IController controller)
        {
            this.UnityContainer.Teardown(controller);
        }
    }
}
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Engine.API.Common
{
    /// <summary>
    /// Unity依赖注入的筛选器提供程序。
    /// </summary>
    public class UnityFilterAttributeFilterProvider : FilterAttributeFilterProvider
    {
        private readonly IUnityContainer container;
        public UnityFilterAttributeFilterProvider(IUnityContainer container)
        {
            this.container = container;
        }
        protected override IEnumerable<FilterAttribute> GetControllerAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetControllerAttributes(controllerContext, actionDescriptor);
            this.BuildUpAttributes(attributes);
            return attributes;
        }
        protected override IEnumerable<FilterAttribute> GetActionAttributes(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var attributes = base.GetActionAttributes(controllerContext, actionDescriptor);
            this.BuildUpAttributes(attributes);
            return attributes;
        }
        private void BuildUpAttributes(IEnumerable attributes)
        {
            foreach (FilterAttribute attribute in attributes)
            {
                container.BuildUp(attribute.GetType(), attribute);
            }
        }
    }
}

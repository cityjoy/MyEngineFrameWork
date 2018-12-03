using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using Unity.Lifetime;

namespace Engine.API.Common
{
    /// <summary>
    /// Web应用每一次Request都只创建一个实例
    /// </summary>
    public class PerWebRequestLifetimeManager : LifetimeManager
    {

        private string Key = "PerWebRequestLifetimeManager" + Guid.NewGuid().ToString();
        public override object GetValue(ILifetimeContainer container = null)
        {
            return   HttpContext.Current.Items[Key]; 
        }

        public override void RemoveValue(ILifetimeContainer container = null)
        {
            HttpContext.Current.Items[Key] = null;
        }

        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            HttpContext.Current.Items[Key] = newValue;
        }
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerWebRequestLifetimeManager();
        }
    }


}
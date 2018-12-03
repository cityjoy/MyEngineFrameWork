using Microsoft.Practices.ObjectBuilder2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
namespace Engine.AdminWeb.Models
{
    /// <summary>
    /// Web应用每一次Request都只创建一个实例
    /// </summary>
    public class PerWebRequestLifetimeManager : Microsoft.Practices.Unity.LifetimeManager
    {

        private string Key = "PerRequest_" + Guid.NewGuid().ToString();
        public override object GetValue()
        {
            return   HttpContext.Current.Items[Key]; 
        }

        public override void RemoveValue()
        {
            HttpContext.Current.Items[Key] = null;
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[Key] = newValue;
        }
    }


}
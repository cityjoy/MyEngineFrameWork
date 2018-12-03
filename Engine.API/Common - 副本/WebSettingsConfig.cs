using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
    /// <summary>
    /// 配置
    /// </summary>
    public class WebSettingsConfig
    { 
        public static string UrlExpireTime
        {
            get
            {
                return AppSettingValue();
            }
        }

        private static string AppSettingValue(string key = null)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}

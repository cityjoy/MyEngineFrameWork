using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils
{
    public class ValidateCodeManager
    {
        /// <summary>
        /// 创建一个验证码实例
        /// </summary>
        /// <returns></returns>
        public static T CreateValidateCode<T>() where T : ValidateCodeBase, new()
        {
            return new T();
        }

    }
}

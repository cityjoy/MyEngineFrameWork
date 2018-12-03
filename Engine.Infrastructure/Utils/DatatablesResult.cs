using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Utils
{
    public class DatatablesResult
    {
        /// <summary>
        /// 总数
        /// </summary>
        public virtual int Total
        {
            get;
            set;
        }
        /// <summary>
        /// 数据
        /// </summary>
        public virtual object Data
        {
            get;
            set;
        }
    }
}

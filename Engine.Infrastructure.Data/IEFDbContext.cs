using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Data
{
    public interface IEFDbContext
    {

        /// <summary>
        /// 泛型返回值类型IDbSet<T >方法Set，并且T 要继承class
        /// </summary>
        /// <typeparam name="T "></typeparam>
        /// <returns></returns>
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        void Dispose();
    }
}

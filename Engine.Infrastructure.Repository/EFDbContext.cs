using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Infrastructure.Repository.ModelMap;
using Engine.Infrastructure.Data;
using System.Configuration;
using System.Reflection;
using System.Data.Entity.ModelConfiguration;
namespace Engine.Infrastructure.Repository
{
    public class EFDbContext : DbContext, IEFDbContext
    {
        /// <summary>
        /// 模型映射类程序集名称
        /// </summary>
        public EFDbContext()
            : base("name=DefaultConnection")
        {
            Database.SetInitializer<EFDbContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
        }
        public override  DbSet<T> Set<T>()
        {
            return base.Set<T>();
        }


        #region Protected Methods
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //使用反射加载程序集中的Fluent API配置项
            var assembly = Assembly.GetExecutingAssembly().GetTypes();
            if (assembly != null)
            {
                var typeList = assembly.Where(t => t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)).ToList();
                if (typeList != null && typeList.Any())
                {
                    foreach (var type in typeList)
                    {
                        dynamic configurationInstance = Activator.CreateInstance(type);
                        modelBuilder.Configurations.Add(configurationInstance
                          );
                    }
                }
            }

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

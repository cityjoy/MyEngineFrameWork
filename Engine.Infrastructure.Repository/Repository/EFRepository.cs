using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Extensions;
using EntityFramework.Future;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity.Infrastructure;
using Engine.Infrastructure.Data;
using Engine.Infrastructure.Utils;
using Engine.Domain.IRepository;
namespace Engine.Infrastructure.Repository
{
    /// <summary>
    /// EF仓储基类
    /// </summary>
    /// <typeparam name="T">实体模型</typeparam>
    public class EFRepository<T> : IRepository<T> where T : class
    {
        protected readonly EFDbContext dbContext;
        private DbSet<T> entities;

        public EFRepository(IEFDbContext context)
        {
            this.dbContext = (EFDbContext)context;
        }
        /// <summary>
        /// 实体集合
        /// </summary>
        public DbSet<T> Entities
        {
            get
            {
                if (entities == null)
                {
                    entities = dbContext.Set<T>();
                }
                return entities;
            }
        }

        #region  CRUD 操作
        /// <summary>
        /// 通过Lamda表达式获取实体列表
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null)
        {

            if (predicate != null)
            {
                return Entities.AsNoTracking().Where(predicate).AsQueryable();
            }
            return Entities;
        }
        /// <summary>  
        /// 分页查询 + 条件查询 + 排序  
        /// </summary>  
        /// <typeparam name="Tkey">泛型</typeparam>  
        /// <param name="pageSize">每页大小</param>  
        /// <param name="pageIndex">当前页码</param>  
        /// <param name="total">总数量</param>  
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）查询条件</param>  
        /// <param name="orderbyPredicate">Lamda表达式（p=>p.Id）排序条件</param>  
        /// <param name="isAsc">是否升序</param>  
        /// <returns>IQueryable 泛型集合</returns> 

        public IQueryable<T> GetPageList<Tkey>(int pageSize, int pageIndex, out int total, Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderbyPredicate, bool isAsc)
        {
            IQueryable<T> result = null;
            //total = Entities.Where(predicate).Count();
            if (isAsc)
            {
                total = Entities.Where(predicate).Count();
                var temp = Entities
                        .Where(predicate)
                        .OrderBy<T, Tkey>(orderbyPredicate)
                        .Skip(pageSize * (pageIndex - 1))
                        .Take(pageSize);
                result = temp;
                return result;
            }
            else
            {

                total = Entities.Where(predicate).Count();
                var temp = Entities
                   .Where(predicate)
                   .OrderByDescending<T, Tkey>(orderbyPredicate)
                   .Skip(pageSize * (pageIndex - 1))
                   .Take(pageSize);
                result = temp;

                return result;
            }
        }

        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public void Add(T entity)
        {
            Entities.Add(entity);
            //int i = dbContext.SaveChanges();
        }
        /// <summary>
        /// 增加多个实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public void AddRange(ICollection<T> entities)
        {
            Entities.AddRange(entities);
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="destinationTableName">表名称</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <summary>

        public void BulkInsert<T>(IList<T> entities, string destinationTableName, string connectionString)
        {
            if (entities == null || !entities.Any()) return;
            if (string.IsNullOrEmpty(destinationTableName))
            {
                return;
            }
            using (var dt = entities.ToDataTable())
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (var tran = conn.BeginTransaction())
                    {
                        try
                        {
                            var bulk = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, tran);
                            bulk.BatchSize = entities.Count;
                            bulk.DestinationTableName = destinationTableName;
                            bulk.EnableStreaming = true;
                            bulk.WriteToServerAsync(dt);
                            tran.Commit();
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 根据条件更新记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        public int Update(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T>> updateExpression = null)
        {
            int result = Entities.Where(predicate).Update(updateExpression);
            return result;
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public void Update(T entity)
        {
            RemoveHoldingEntityInContext(entity);
            Entities.Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
        }


        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
            {
                return Entities.AsNoTracking().FirstOrDefault();
            }
            else
            {
                return Entities.Where(predicate).AsNoTracking().FirstOrDefault();
            }
        }

        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <param name="includes">预加载的属性名称</param>
        /// <returns></returns>
        public T GetSingle<TProperty>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, TProperty>>[] includes)
        {
            if (includes == null)
            {
                return GetSingle(predicate);
            }
            var query = Entities.AsNoTracking().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            if (predicate == null)
            {
                return query.SingleOrDefault();
            }
            return query.Where(predicate).SingleOrDefault();
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public void Delete(Expression<Func<T, bool>> predicate = null)
        {
            Entities.Where(predicate).Delete();
        }
        /// <summary>
        /// 用于监测Context中的Entity是否存在，如果存在，将其Detach，防止出现问题。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Boolean RemoveHoldingEntityInContext(T entity)
        {
            var objContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            var objSet = objContext.CreateObjectSet<T>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);

            if (exists)
            {
                objContext.Detach(foundEntity);
            }

            return (exists);
        }
        /// <summary>
        /// 使用sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public   int ExecuteSqlWithNonQuery(string sql, params object[] parameters)
        {
            return dbContext.Database.ExecuteSqlCommand(sql, parameters);
        }
        #endregion



    }
}

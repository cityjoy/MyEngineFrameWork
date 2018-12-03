using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Engine.Domain.Entity;
namespace Engine.Domain.IRepository
{
    /// <summary>
    /// 泛型仓储接口
    /// </summary>
    /// <typeparam name="T">实体模型</typeparam>
    public interface IRepository<T> where T : class
    {
        #region  CRUD 操作
        /// <summary>
        /// 通过Lamda表达式获取实体列表
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null);

       /// <summary>  
       /// 分页查询 + 条件查询 + 排序  
       /// </summary>  
       /// <typeparam name="Tkey">泛型</typeparam>  
       /// <param name="pageSize">每页大小</param>  
       /// <param name="pageIndex">当前页码</param>  
       /// <param name="total">总数量</param>  
        /// <param name="predicate">查询条件</param>  
       /// <param name="orderbyLambda">排序条件</param>  
       /// <param name="isAsc">是否升序</param>  
       /// <returns>IQueryable 泛型集合</returns> 
        IQueryable<T> GetPageList<Tkey>(int pageSize, int pageIndex, out int total,Expression<Func<T, bool>> predicate, Expression<Func<T, Tkey>> orderbyPredicate, bool isAsc);

        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        T GetSingle(Expression<Func<T, bool>> predicate = null);

        T GetSingle<TProperty>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, TProperty>>[] includes);

        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        void Add(T entity);
        /// <summary>
        /// 增加多个实体
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        void AddRange(ICollection<T> entities);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="destinationTableName">表名称</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <summary>
        void BulkInsert<T>(IList<T> entities, string destinationTableName, string connectionString);

        /// <summary>
        /// 根据条件更新记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        int Update(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T>> updateExpression = null);

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        void Update(T entity);

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <returns></returns>
        void Delete(Expression<Func<T, bool>> predicate = null);


        /// <summary>
        /// 用于监测Context中的Entity是否存在，如果存在，将其Detach，防止出现问题。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Boolean RemoveHoldingEntityInContext(T entity);

        /// <summary>
        /// 使用sql语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteSqlWithNonQuery(string sql, params object[] parameters);

        #endregion
    }
}

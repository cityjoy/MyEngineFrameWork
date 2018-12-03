using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Application
{
    /// <summary>
    ///  泛型服务接口
    /// </summary>
    /// <typeparam name="T">实体模型</typeparam>
    public interface IServer<T> where T : class
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
        /// <param name="whereLambda">查询条件</param>  
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
      
        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <param name="includes">预加载的属性名称</param>
        /// <returns></returns>
        T GetSingle<TProperty>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, TProperty>>[] includes);

        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        bool Add(T entity, bool isSave = true);

        /// <summary>
        /// 增加多个实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        bool AddRange(ICollection<T> entities, bool isSave = true);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="destinationTableName"></param>
        void BulkInsert(IList<T> entities, string destinationTableName, string connectionString);

        /// <summary>
        /// 根据条件更新记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        bool Update(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T>> updateExpression = null, bool isSave = true);

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        bool Update(T entity, bool isSave = true);
        
         /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <returns></returns>
        bool Delete(Expression<Func<T, bool>> predicate = null ,bool isSave = true);

        #endregion
    }
}

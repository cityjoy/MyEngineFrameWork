using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Engine.Application.DTO;
using Engine.Domain.IRepository;
using Engine.Infrastructure.Data;

namespace Engine.Application
{
    public class BaseServer<T> : IServer<T> where T : class
    {

        protected readonly IRepository<T> repository;
        protected readonly IUnitOfWork unitOfWork;

        public BaseServer(IRepository<T> repository, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.unitOfWork = unitOfWork;
        }
        #region  CRUD 操作
        /// <summary>
        /// 通过Lamda表达式获取实体列表
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null)
        {

            return repository.GetList(predicate);
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
            return repository.GetPageList(pageSize, pageIndex, out  total, predicate, orderbyPredicate, isAsc);
        }

        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> predicate = null)
        {

            return repository.GetSingle(predicate);

        }
        /// <summary>
        /// 通过Lamda表达式获取实体
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <param name="includes">预加载的属性名称</param>
        /// <returns></returns>
        public T GetSingle<TProperty>(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, TProperty>>[] includes)
        {
            return repository.GetSingle<TProperty>(predicate,includes);

        }

        /// <summary>
        /// 增加一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public bool Add(T entity, bool isSave = true)
        {
            bool result = false;
            repository.Add(entity);
            result = isSave ? unitOfWork.Commit() : false;
            return result;
        }

        /// <summary>
        /// 增加多个实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public bool AddRange(ICollection<T> entities, bool isSave = true)
        {
            bool result = false;
            repository.AddRange(entities);
            result = isSave ? unitOfWork.Commit() : false;
            return result;
        }
        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="destinationTableName">表名称</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <summary>
        public void BulkInsert(IList<T> entities, string destinationTableName, string connectionString)
        {
            repository.BulkInsert(entities, destinationTableName, connectionString);
        }

        /// <summary>
        /// 根据条件更新记录
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="updateExpression"></param>
        /// <returns></returns>
        public bool Update(Expression<Func<T, bool>> predicate = null, Expression<Func<T, T>> updateExpression = null, bool isSave = true)
        {
            bool result = repository.Update(predicate, updateExpression) > 0;
            return result;
        }
        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="entity">实体模型</param>
        /// <returns></returns>
        public bool Update(T entity, bool isSave = true)
        {
            bool result = false;
            repository.Update(entity);
            result = isSave ? unitOfWork.Commit() : false;
            return result;
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> predicate = null, bool isSave = true)
        {
            bool result = false;
            repository.Delete(predicate);
            result = isSave ? unitOfWork.Commit() : false;
            return result;
        }

        #endregion


    }
}

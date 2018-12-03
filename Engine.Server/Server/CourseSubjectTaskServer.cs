
using Course.Domain.Entity;
using Course.Domain.IRepository;
using Course.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
 
 

namespace Course.Server
{
    public class CourseSubjectTaskServer : BaseServer<CourseSubjectTask>, ICourseSubjectTaskServer
    {

        public CourseSubjectTaskServer(ICourseSubjectTaskRepository courseSubjectTaskRepository,IUnitOfWork unitOfWork)
            : base(courseSubjectTaskRepository, unitOfWork)
        {

        }
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <param name="include1"></param>
        /// <param name="include2"></param>
        /// <param name="predicate"></param>
        /// <param name="orderbyPredicate"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        //public IQueryable<CourseSubjectTaskDto> GetSubjectTaskPageList<Tkey>(
        //    int pageSize,
        //    int pageIndex,
        //    out int total,
        //    string include1,
        //    string include2,
        //    Expression<Func<CourseSubjectTask, bool>> predicate,
        //    Expression<Func<CourseSubjectTask, Tkey>> orderbyPredicate,
        //    bool isAsc)
        //{

        //    IQueryable<CourseSubjectTask> result = this.GetPageList(pageSize, pageIndex, out total, include1, include2, predicate, orderbyPredicate, isAsc);
        //    IQueryable<CourseSubjectTaskDto> resultDto = result.ProjectTo<CourseSubjectTaskDto>();
        //    return resultDto;
        //}
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <param name="include1"></param>
        /// <param name="predicate"></param>
        /// <param name="orderbyPredicate"></param>
        /// <param name="isAsc"></param>
        /// <returns></returns>
        //public IQueryable<CourseSubjectTaskDto> GetSubjectTaskPageList<Tkey>(
        //    int pageSize,
        //    int pageIndex,
        //    out int total,
        //    string include1,
        //    Expression<Func<CourseSubjectTask, bool>> predicate,
        //    Expression<Func<CourseSubjectTask, Tkey>> orderbyPredicate,
        //    bool isAsc)
        //{

        //    IQueryable<CourseSubjectTask> result = this.GetPageList(pageSize, pageIndex, out total, include1, predicate, orderbyPredicate, isAsc);
        //    IQueryable<CourseSubjectTaskDto> resultDto = result.ProjectTo<CourseSubjectTaskDto>();
        //    return resultDto;
        //}
    }
}

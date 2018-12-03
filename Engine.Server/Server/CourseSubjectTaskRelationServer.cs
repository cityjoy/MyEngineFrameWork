
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
    public class CourseSubjectTaskRelationServer : BaseServer<CourseSubjectTaskRelation>, ICourseSubjectTaskRelationServer
    {


        public CourseSubjectTaskRelationServer(ICourseSubjectTaskRelationRepository courseSubjectTaskRelationRepository,IUnitOfWork unitOfWork)
            : base(courseSubjectTaskRelationRepository, unitOfWork)
        {

        }
        /// <summary>
        /// 获取任务关系列表
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
        //public IQueryable<CourseSubjectTaskRelationDto> GetSubjectTaskRelationPageList<Tkey>(
        //    int pageSize,
        //    int pageIndex,
        //    out int total,
        //    string include1,
        //    string include2,
        //    Expression<Func<CourseSubjectTaskRelation, bool>> predicate,
        //    Expression<Func<CourseSubjectTaskRelation, Tkey>> orderbyPredicate,
        //    bool isAsc)
        //{

        //    IQueryable<CourseSubjectTaskRelation> result = this.GetPageList(pageSize, pageIndex, out total, include1, include2, predicate, orderbyPredicate, isAsc);
        //    IQueryable<CourseSubjectTaskRelationDto> resultDto = result.ProjectTo<CourseSubjectTaskRelationDto>();
        //    return resultDto;
        //}
        /// <summary>
        /// 获取任务关系列表
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
        //public IQueryable<CourseSubjectTaskRelationDto> GetSubjectTaskRelationPageList<Tkey>(
        //    int pageSize,
        //    int pageIndex,
        //    out int total,
        //    string include1,
        //    Expression<Func<CourseSubjectTaskRelation, bool>> predicate,
        //    Expression<Func<CourseSubjectTaskRelation, Tkey>> orderbyPredicate,
        //    bool isAsc)
        //{

        //    IQueryable<CourseSubjectTaskRelation> result = this.GetPageList(pageSize, pageIndex, out total, include1, predicate, orderbyPredicate, isAsc);
        //    IQueryable<CourseSubjectTaskRelationDto> resultDto = result.ProjectTo<CourseSubjectTaskRelationDto>();
        //    return resultDto;
        //}
    }
}

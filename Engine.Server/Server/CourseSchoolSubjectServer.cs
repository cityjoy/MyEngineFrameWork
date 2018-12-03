
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
    public class CourseSchoolSubjectServer : BaseServer<CourseSchoolSubject>, ICourseSchoolSubjectServer
    {

        public CourseSchoolSubjectServer(ICourseSchoolSubjectRepository courseSchoolSubjectRepository,IUnitOfWork unitOfWork)
            : base(courseSchoolSubjectRepository,unitOfWork)
        {

        }

        /// <summary>
        /// 通过Lamda表达式获取实体列表
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        //public IQueryable<CourseSchoolSubjectDto> GetSubjectList(Expression<Func<CourseSchoolSubject, bool>> predicate = null)
        //{
        //    IQueryable<CourseSchoolSubject> result = this.GetList(predicate);
        //    IQueryable<CourseSchoolSubjectDto> resultDto = result.ProjectTo<CourseSchoolSubjectDto>();
        //    return resultDto;
        //}
    }
}

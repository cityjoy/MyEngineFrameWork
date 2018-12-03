 
using Course.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Course.Server
{
    /// <summary>
    /// 学校科目服务接口
    /// </summary>
    public interface ICourseSchoolSubjectServer : IServer<CourseSchoolSubject>
    {
        /// <summary>
        /// 通过Lamda表达式获取实体列表
        /// </summary>
        /// <param name="predicate">Lamda表达式（p=>p.Id==Id）</param>
        /// <returns></returns>
        //IQueryable<CourseSchoolSubjectDto> GetSubjectList(Expression<Func<CourseSchoolSubject, bool>> predicate = null);
    }
}

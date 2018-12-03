
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
    /// <summary>
    /// 学年学期班级的学生服务实现
    /// </summary>

    public class CourseGradeStudentServer : BaseServer<CourseGradeStudent>, ICourseGradeStudentServer
    {


        public CourseGradeStudentServer(ICourseGradeStudentRepository repository, IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

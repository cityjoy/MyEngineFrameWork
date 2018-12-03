
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
    /// 分班任务服务实现
    /// </summary>
	
    public class CourseGradeTaskServer :BaseServer<CourseGradeTask>, ICourseGradeTaskServer
    {


        public CourseGradeTaskServer(ICourseGradeTaskRepository repository,IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

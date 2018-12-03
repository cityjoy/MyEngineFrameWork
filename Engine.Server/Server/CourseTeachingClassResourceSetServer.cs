
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
    /// 排选课教学班资源分配表服务实现
    /// </summary>
	
    public class CourseTeachingClassResourceSetServer :BaseServer<CourseTeachingClassResourceSetting>, ICourseTeachingClassResourceSetServer
    {


        public CourseTeachingClassResourceSetServer(ICourseTeachingClassResourceSetRepository repository, IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}


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
    /// 排课任务节次设置服务实现
    /// </summary>
	
    public class CourseAssignmentSectionServer :BaseServer<CourseAssignmentSection>, ICourseAssignmentSectionServer
    {


        public CourseAssignmentSectionServer(ICourseAssignmentSectionRepository repository,IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}


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
    /// CourseAssignmentTask服务实现
    /// </summary>
	
    public class CourseAssignmentTaskServer :BaseServer<CourseAssignmentTask>, ICourseAssignmentTaskServer
    {


        public CourseAssignmentTaskServer(ICourseAssignmentTaskRepository repository,IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

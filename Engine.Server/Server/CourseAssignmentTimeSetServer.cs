
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
    /// CourseAssignmentTimeSetting服务实现
    /// </summary>
	
    public class CourseAssignmentTimeSetServer :BaseServer<CourseAssignmentTimeSetting>, ICourseAssignmentTimeSetServer
    {


        public CourseAssignmentTimeSetServer(ICourseAssignmentTimeSetRepository repository, IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

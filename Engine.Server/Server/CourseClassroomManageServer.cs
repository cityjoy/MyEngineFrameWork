
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
    /// 选排课系统教室管理表服务实现
    /// </summary>
	
    public class CourseClassroomManageServer :BaseServer<CourseClassroomManage>, ICourseClassroomManageServer
    {


        public CourseClassroomManageServer(ICourseClassroomManageRepository repository,IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

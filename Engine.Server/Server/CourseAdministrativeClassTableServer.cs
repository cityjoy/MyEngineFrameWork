
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
    /// 行政班课表信息服务实现
    /// </summary>
	
    public class CourseAdministrativeClassTableServer :BaseServer<CourseAdministrativeClassTable>, ICourseAdministrativeClassTableServer
    {


        public CourseAdministrativeClassTableServer(ICourseAdministrativeClassTableRepository repository,IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

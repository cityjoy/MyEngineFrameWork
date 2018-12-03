
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
    ///  教学班分班服务实现
    /// </summary>
	
    public class CourseTechingClassRoomServer :BaseServer<CourseTechingClassRoom>, ICourseTechingClassRoomServer
    {


        public CourseTechingClassRoomServer(ICourseTechingClassRoomRepository repository,IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

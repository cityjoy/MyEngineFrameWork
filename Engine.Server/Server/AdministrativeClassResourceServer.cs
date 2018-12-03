
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
    /// 行政班资源服务接口实现
    /// </summary>

    public class AdministrativeClassResourceServer : BaseServer<CourseAdministrativeClassResourceSetting>, IAdministrativeClassResourceServer
    {
        public AdministrativeClassResourceServer(IAdministrativeClassResourceRepository rep, IUnitOfWork unitOfWork)
            : base(rep, unitOfWork)
        {
        }

    }
}

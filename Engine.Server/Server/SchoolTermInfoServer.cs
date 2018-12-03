
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
    /// 学校学期日期信息服务实现
    /// </summary>
	
    public class SchoolTermInfoServer :BaseServer<SchoolTermInfo>, ISchoolTermInfoServer
    {


        public SchoolTermInfoServer(ISchoolTermInfoRepository repository,IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

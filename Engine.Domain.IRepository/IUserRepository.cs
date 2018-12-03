using Engine.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Domain.IRepository
{
	/// <summary>
    /// 用户仓储接口
    /// </summary>
	
    public interface IUserRepository : IRepository<User>
    {

    }
}

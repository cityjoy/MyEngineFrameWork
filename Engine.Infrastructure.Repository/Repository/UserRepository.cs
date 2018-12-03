using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Domain.IRepository;
using Engine.Domain.Entity;
using Engine.Infrastructure.Data;

namespace Engine.Infrastructure.Repository
{
	/// <summary>
    /// 用户仓储实现
    /// </summary>
	
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
     public UserRepository(IEFDbContext dbContext)
            : base(dbContext)
        {

        }

    }
}

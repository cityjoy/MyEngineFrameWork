using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Infrastructure.Data;
using Engine.Domain.Entity;
using Engine.Domain.IRepository;

namespace Engine.Infrastructure.Repository
{
	 
	
    public class StudentRepository : EFRepository<Student>, IStudentRepository
    {
        public StudentRepository(IEFDbContext dbContext)
            : base(dbContext)
        {

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Engine.Domain.Entity;
using Engine.Domain.IRepository;
using Engine.Application.DTO;
using Engine.Infrastructure.Data;


namespace Engine.Application
{
	
    public class StudentServer :BaseServer<Student>, IStudentServer
    {


        public StudentServer(IRepository<Student> repository, IUnitOfWork unitOfWork)
            : base(repository, unitOfWork)
        {


        }
    }
}

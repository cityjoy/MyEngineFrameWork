using Course.Domain.Entity;
using Course.Domain.IRepository;
using Course.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Server
{
    public class CourseSubjectServer : BaseServer<CourseSubject>, ICourseSubjectServer
    {

        public CourseSubjectServer(ICourseSubjectRepository courseSubjectRepository,IUnitOfWork unitOfWork)
            : base(courseSubjectRepository, unitOfWork)
        {

        }
    }
}

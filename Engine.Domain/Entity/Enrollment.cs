using Engine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Engine.Domain.Entity
{
 

    public class Enrollment
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Grade? Grade { get; set; }

        public Courses Course { get; set; }
        public Student Student { get; set; }
    }
}
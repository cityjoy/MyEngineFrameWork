using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Engine.Domain.Entity
{
    public class Courses
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
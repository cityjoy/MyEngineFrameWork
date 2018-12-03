using Engine.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Infrastructure.Repository.ModelMap
{
    public class CoursesMap : EntityTypeConfiguration<Courses>
    {
        public CoursesMap()
        {
            ToTable("Courses");
            HasKey(t => t.ID);
            Property(t => t.ID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            HasMany(t => t.Enrollments);
        }
    }
}

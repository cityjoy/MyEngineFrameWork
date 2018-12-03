﻿using Engine.Domain.Entity;
using Engine.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Engine.Infrastructure.Repository
{
    public static class DbInitializer
    {
        public static void Initialize(EFDbContext context)
        {

            // Look for any students.
            if (context.Set<Student>().Any())
            {
                return;   // DB has been seeded
            }

            var students = new Student[]
            {
        new Student{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01")},
        new Student{FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=DateTime.Parse("2002-09-01")},
        new Student{FirstMidName="Arturo",LastName="Anand",EnrollmentDate=DateTime.Parse("2003-09-01")},
        new Student{FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=DateTime.Parse("2002-09-01")},
        new Student{FirstMidName="Yan",LastName="Li",EnrollmentDate=DateTime.Parse("2002-09-01")},
        new Student{FirstMidName="Peggy",LastName="Justice",EnrollmentDate=DateTime.Parse("2001-09-01")},
        new Student{FirstMidName="Laura",LastName="Norman",EnrollmentDate=DateTime.Parse("2003-09-01")},
        new Student{FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=DateTime.Parse("2005-09-01")}
            };
            foreach (Student s in students)
            {
                context.Set<Student>().Add(s);
            }
            context.SaveChanges();

            var courses = new Courses[]
            {
        new Courses{ID=1050,Title="Chemistry",Credits=3,},
        new Courses{ID=4022,Title="Microeconomics",Credits=3,},
        new Courses{ID=4041,Title="Macroeconomics",Credits=3,},
        new Courses{ID=1045,Title="Calculus",Credits=4,},
        new Courses{ID=3141,Title="Trigonometry",Credits=4,},
        new Courses{ID=2021,Title="Composition",Credits=3,},
        new Courses{ID=2042,Title="Literature",Credits=4,}
            };
            foreach (Courses c in courses)
            {
                context.Set<Courses>().Add(c);
            }
            context.SaveChanges();

            var enrollments = new Enrollment[]
            {
        new Enrollment{StudentID=1,CourseID=1050,Grade=Grade.A},
        new Enrollment{StudentID=1,CourseID=4022,Grade=Grade.C},
        new Enrollment{StudentID=1,CourseID=4041,Grade=Grade.B},
        new Enrollment{StudentID=2,CourseID=1045,Grade=Grade.B},
        new Enrollment{StudentID=2,CourseID=3141,Grade=Grade.F},
        new Enrollment{StudentID=2,CourseID=2021,Grade=Grade.F},
        new Enrollment{StudentID=3,CourseID=1050},
        new Enrollment{StudentID=4,CourseID=1050,},
        new Enrollment{StudentID=4,CourseID=4022,Grade=Grade.F},
        new Enrollment{StudentID=5,CourseID=4041,Grade=Grade.C},
        new Enrollment{StudentID=6,CourseID=1045},
        new Enrollment{StudentID=7,CourseID=3141,Grade=Grade.A},
            };
            foreach (Enrollment e in enrollments)
            {
                context.Set<Enrollment>().Add(e);
            }
            context.SaveChanges();
        }
    }
}
using StudentInformation.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentInformation.Data
{
    //Creates a new database context named StudentInformationContext
    public class StudentInformationContext : DbContext
    {
        public StudentInformationContext(DbContextOptions<StudentInformationContext> options) : base(options)
        {
        }

        public StudentInformationContext() {}

        //This is where we register our models as entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Student> Students { get; set; }
    }
}
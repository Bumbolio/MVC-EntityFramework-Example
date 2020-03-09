using System;
using System.Collections.Generic;

namespace StudentInformation.Models
{
    public class Course
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
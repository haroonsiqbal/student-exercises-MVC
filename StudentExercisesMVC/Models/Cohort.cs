using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models
{
    public class Cohort
    {
        public int Id { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 5)]
        public string CohortName { get; set; }
        List<Cohort> cohortlist = new List<Cohort>();
        public Student Student { get; set; }
        public Instructor Instructor { get; set; }
        public List<Student> students { get; set; } = new List<Student>();
        public List<Instructor> instructors { get; set; } = new List<Instructor>();
    }
}

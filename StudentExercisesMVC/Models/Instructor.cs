using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        [Required]
        public string InstFirstName { get; set; }
        [Required]
        public string InstLastName { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 3)]
        public string InstSlackHandle { get; set; }
        public int InstCohort { get; set; }
        public Cohort Cohort { get; set; }
        public string
            InstSpeciality
        { get; set; }
        public List<Instructor> instructorlist { get; set; } = new List<Instructor>();
    }
}

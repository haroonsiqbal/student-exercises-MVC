using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class StudentEditViewModel
    {
        public List<Cohort> cohorts { get; set; } = new List<Cohort>();
        public Student student { get; set; } = new Student();

        public List<SelectListItem>CohortOptions
        {
            get
            {
                if (cohorts == null) return null;
                return cohorts
                    .Select(c => new SelectListItem(c.CohortName, c.Id.ToString()))
                    .ToList();
            }
        }

    }
}
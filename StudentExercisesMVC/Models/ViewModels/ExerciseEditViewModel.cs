using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class ExerciseEditViewModel
    {
        //public List<Cohort> cohorts { get; set; } = new List<Cohort>();
        public Exercise exercise { get; set; } = new Exercise();

        //public List<SelectListItem>CohortOptions
        //{
        //    get
        //    {
        //        if (cohorts == null) return null;
        //        return cohorts
        //            .Select(c => new SelectListItem(c.CohortName, c.Id.ToString()))
        //            .ToList();
        //    }
        //}

    }
}
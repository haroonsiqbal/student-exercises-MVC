﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentExercisesMVC.Models.ViewModels
{
    public class CohortCreateViewModel
    {
        //public List<SelectListItem> Cohorts { get; set; }
        public Cohort cohort { get; set; } = new Cohort();

    }
}

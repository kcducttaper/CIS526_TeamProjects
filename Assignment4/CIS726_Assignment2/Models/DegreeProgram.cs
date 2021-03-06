﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CIS726_Assignment2.Models
{
    /// <summary>
    /// This class represents a single degree program. It has a list of required courses and a list of elective courses
    /// </summary>
    public class DegreeProgram : IModel
    {
        [ScaffoldColumn(false)]
        public override int ID { get; set; }

        [DisplayName("Degree Program Name")]
        [Required(ErrorMessage = "A Degree Program Name is required")]
        public String degreeProgramName { get; set; }

        [DisplayName("Degree Program Description")]
        [DataType(DataType.MultilineText)]
        public String degreeProgramDescription { get; set; }

        public virtual List<RequiredCourse> requiredCourses { get; set; }
        public virtual List<ElectiveCourse> electiveCourses { get; set; }
    }
}
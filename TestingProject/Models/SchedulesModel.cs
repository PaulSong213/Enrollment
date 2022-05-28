using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class SchedulesModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "courseId")]
        public int courseId { get; set; }

        [Display(Name = "Year")]
        public int year { get; set; }

        [Display(Name = "Section")]
        public int section { get; set; }


        [Display(Name = "Scheadule Link")]
        public string link { get; set; }

        [Display(Name = "CourseModel")]
        public CoursesModel CoursesModel { get; set; }

    }
}
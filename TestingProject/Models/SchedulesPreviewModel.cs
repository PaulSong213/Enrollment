using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class SchedulesPreviewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "course")]
        public string course { get; set; }

        [Display(Name = "year")]
        public int year { get; set; }

        [Display(Name = "section")]
        public int section { get; set; }


        [Display(Name = "link")]
        public string link { get; set; }

    }
}
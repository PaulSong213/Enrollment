using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class CoursesModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Acronym")]
        public string Acronym { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Slots")]
        public int Slots { get; set; }
    }
}
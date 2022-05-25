using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class RegistrarsModelPreview
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }


        [Display(Name = "ProfileFileName")]
        public string ProfileFileName { get; set; }


    }
}
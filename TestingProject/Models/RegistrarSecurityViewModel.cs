using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class RegistrarSecurityViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "System Password")]
        public string Password { get; set; }
    }
}
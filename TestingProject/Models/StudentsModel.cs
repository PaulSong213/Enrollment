using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace EnrollmentSystem.Models
{
    public class StudentsModel
    {
        [Key]
        public int id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Middle name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Contact number")]
        public string ContactNumber { get; set; }

        [Required]
        [Display(Name = "Account ID")]
        public string AccountId { get; set; }


        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Status ID")]
        public int StatusId { get; set; }

        [Display(Name = "Profile File Name")]
        public string ProfileFileName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}

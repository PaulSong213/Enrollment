using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class RegistrarsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Firebase Account Id")]
        public string AccountId { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public int IsActive { get; set; }

        [Display(Name = "Profile File Name")]
        public string ProfileFileName { get; set; }

        public HttpPostedFileBase UploadedProfileFileName { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class EnrollmentPreviewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student Id")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Course")]
        public string CourseName { get; set; }
    }
}
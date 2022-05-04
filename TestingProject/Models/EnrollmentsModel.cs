using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EnrollmentSystem.Models
{
    public class EnrollmentsModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Student Id")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Course Id")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "School Year")]
        public string SchoolYearStart { get; set; }

        [Required]
        [Display(Name = "Status Id")]
        public int TypeId { get; set; }

        [Required]
        [Display(Name = "ProfileFileName")]
        public string ProfileFileName { get; set; }

        [Display(Name = "Report Card File Name")]
        public string ReportCardFileName { get; set; }

        [Display(Name = "Birth Certificate File Name")]
        public string BirthCertificateFileName { get; set; }

        [Display(Name = "Good Moral Certificate File Name")]
        public string GoodMoralCertificateFileName { get; set; }

        [Display(Name = "Certificate Of Transfer File Name")]
        public string CertificateOfTransferFileName { get; set; }

        [Display(Name = "Honorable Dismissal File Name")]
        public string HonorableDismissalFileName { get; set; }

        [Display(Name = "Is Active")]
        public string IsActive { get; set; }




    }
}
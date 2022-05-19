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
        public string Type { get; set; }

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

        [Display(Name = "Date Enrolled")]
        public string DateEnrolled { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Student Model")]
        public StudentsModel studentsModel { get; set; }

        [Display(Name = "Course Model")]
        public CoursesModel coursesModel { get; set; }

        [Display(Name = "Registrar Evaluator Model")]
        public RegistrarsModel registrarsModel { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Section")]
        public int Section { get; set; }

        [Display(Name = "Registrar Evaluator Id")]
        public int RegistrarEvaluatorId { get; set; }

    }
}
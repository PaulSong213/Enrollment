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

        [Display(Name = "Student Id")]
        public int StudentId { get; set; }

        [Display(Name = "Course Id")]
        public int CourseId { get; set; }

        [Display(Name = "School Year")]
        public DateTime SchoolYearStart { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Report Card File Name")]
        public string ReportCardFileName { get; set; }

        [Display(Name = "ReportCardFileNameUpload")]
        public HttpPostedFileBase ReportCardFileNameUpload { get; set; }

        [Display(Name = "Birth Certificate File Name")]
        public string BirthCertificateFileName { get; set; }

        [Display(Name = "BirthCertificateFileNameUpload")]
        public HttpPostedFileBase BirthCertificateFileNameUpload { get; set; }

        [Display(Name = "Good Moral Certificate File Name")]
        public string GoodMoralCertificateFileName { get; set; }

        [Display(Name = "GoodMoralCertificateFileNameUpload")]
        public HttpPostedFileBase GoodMoralCertificateFileNameUpload { get; set; }

        [Display(Name = "Certificate Of Transfer File Name")]
        public string CertificateOfTransferFileName { get; set; }

        [Display(Name = "CertificateOfTransferFileNameUpload")]
        public HttpPostedFileBase CertificateOfTransferFileNameUpload { get; set; }

        [Display(Name = "Honorable Dismissal File Name")]
        public string HonorableDismissalFileName { get; set; }

        [Display(Name = "HonorableDismissalFileNameUpload")]
        public HttpPostedFileBase HonorableDismissalFileNameUpload { get; set; }

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


        [Display(Name = "Enrollment Notification Email Receipient")]
        public string EmailRecipient { get; set; }

    }
}
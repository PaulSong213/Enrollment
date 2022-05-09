using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using EnrollmentSystem.Models;
using Newtonsoft.Json;

namespace EnrollmentSystem.Controllers
{
    public class EnrollmentsController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        [HttpGet]
        public ActionResult Index()
        {
            List<EnrollmentsModel> enrollments = new List<EnrollmentsModel>();
            List<EnrollmentPreviewModel> enrollmentPreviews = new List<EnrollmentPreviewModel>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [enrollment_system].[dbo].[enrollments] INNER JOIN [enrollment_system].[dbo].[courses] ON [enrollment_system].[dbo].[enrollments].[courseId] = [enrollment_system].[dbo].[courses].[id]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    //Preview of enrollment
                    EnrollmentPreviewModel enrollmentPreview = new EnrollmentPreviewModel();
                    enrollmentPreview.Id = (int)dr["id"];
                    enrollmentPreview.StudentId = (int)dr["StudentId"];
                    enrollmentPreview.CourseName = dr["name"].ToString();
                    enrollmentPreviews.Add(enrollmentPreview);

                    //Save the whole enrollment information
                    EnrollmentsModel enrollment = new EnrollmentsModel();
                    enrollment.Id = (int)dr["id"];
                    enrollment.BirthCertificateFileName = dr["BirthCertificateFileName"] != DBNull.Value ? dr["BirthCertificateFileName"].ToString() : "";
                    enrollment.CertificateOfTransferFileName = dr["CertificateOfTransferFileName"] != DBNull.Value ? dr["CertificateOfTransferFileName"].ToString() : "" ;
                    enrollment.GoodMoralCertificateFileName = dr["goodMoralCertificateFileName"] != DBNull.Value ? dr["GoodMoralFileName"].ToString() : "";
                    enrollment.HonorableDismissalFileName = dr["HonorableDismissalFileName"] != DBNull.Value ? dr["HonorableDismissalFileName"].ToString() : "" ;
                    enrollment.ProfileFileName = dr["ProfileFileName"].ToString();
                    enrollment.ReportCardFileName = dr["ReportCardFileName"].ToString();
                    enrollment.SchoolYearStart = dr["SchoolYearStart"].ToString();
                    enrollment.CourseId = (int)dr["CourseId"];
                    enrollment.IsActive = dr["IsActive"].ToString();
                    enrollment.StudentId = (int)dr["StudentId"];
                    enrollment.TypeId = (int)dr["TypeId"];
                    enrollments.Add(enrollment);
                }
            }
            con.Close();
            ViewBag.Enrollments = JsonConvert.SerializeObject(enrollments);
            ViewBag.EnrollmentPreviews = JsonConvert.SerializeObject(enrollmentPreviews);
            return View();
        }

        [HttpGet]
        public ActionResult Regular()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Transferee()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Incoming()
        {
            return View();
        }

    }
}

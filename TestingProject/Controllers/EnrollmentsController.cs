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

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [enrollment_system].[dbo].[enrollments]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    EnrollmentsModel enrollment = new EnrollmentsModel();
                    enrollment.Id = (int)dr["id"];
                    enrollment.BirthCertificateFileName = dr["BirthCertificateFileName"].ToString();
                    enrollment.CertificateOfTransferFileName = dr["CertificateOfTransferFileName"].ToString();
                    enrollment.GoodMoralFileName = dr["GoodMoralFileName"].ToString();
                    enrollment.HonorableDismissalFileName = dr["HonorableDismissalFileName"].ToString();
                    enrollment.ProfileFileName = dr["ProfileFileName"].ToString();
                    enrollment.ReportCardFileName = dr["ReportCardFileName"].ToString();
                    enrollment.SchoolYearStart = dr["SchoolYearStart"].ToString();
                    enrollment.CourseId = (int)dr["CourseId"];
                    enrollment.IsActive = (int)dr["IsActive"];
                    enrollment.StudentId = (int)dr["StudentId"];
                    enrollment.TypeId = (int)dr["TypeId"];

                    enrollments.Add(enrollment);
                }
            }
            con.Close();
            ViewBag.Enrollments = JsonConvert.SerializeObject(enrollments);
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

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
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
            string enrollmentType = "regular";

            string url = Request.Url.AbsoluteUri;
            Uri myUri = new Uri(url);
            string type = HttpUtility.ParseQueryString(myUri.Query).Get("type");
            if (type != null) enrollmentType = type;

            List<EnrollmentsModel> enrollments = new List<EnrollmentsModel>();
            List<EnrollmentPreviewModel> enrollmentPreviews = new List<EnrollmentPreviewModel>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[enrollments] AS enrollments INNER JOIN (select id, name as courseName, acronym as courseAcronym from courses) [courses] ON [enrollments].[courseId] = [courses].[id] INNER JOIN (select id, firstName as studentFirstName, middleName as studentMiddleName, lastName as studentLastName, gender as studentGender, age as studentAge, address as studentAddress, contactNumber as studentContactNumber, email as studentEmail, profileFileName as studentProfileFileName from students)  [students] ON [enrollments].[studentId] = students.id WHERE type = '{enrollmentType}' AND status = 'pending'";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    //Preview of enrollment
                    EnrollmentPreviewModel enrollmentPreview = new EnrollmentPreviewModel();
                    enrollmentPreview.Id = (int)dr["id"];
                    enrollmentPreview.StudentId = (int)dr["StudentId"];
                    enrollmentPreview.CourseName = dr["courseAcronym"].ToString();
                    enrollmentPreviews.Add(enrollmentPreview);

                    //Save the whole enrollment information
                    EnrollmentsModel enrollment = new EnrollmentsModel();
                    enrollment.Id = (int)dr["id"];
                    enrollment.BirthCertificateFileName = dr["BirthCertificateFileName"] != DBNull.Value ? dr["BirthCertificateFileName"].ToString() : "";
                    enrollment.DateEnrolled = dr["DateEnrolled"] != DBNull.Value ? dr["DateEnrolled"].ToString() : "";
                    enrollment.CertificateOfTransferFileName = dr["CertificateOfTransferFileName"] != DBNull.Value ? dr["CertificateOfTransferFileName"].ToString() : "" ;
                    enrollment.GoodMoralCertificateFileName = dr["goodMoralCertificateFileName"] != DBNull.Value ? dr["GoodMoralFileName"].ToString() : "";
                    enrollment.HonorableDismissalFileName = dr["HonorableDismissalFileName"] != DBNull.Value ? dr["HonorableDismissalFileName"].ToString() : "" ;
                    enrollment.ProfileFileName = dr["ProfileFileName"].ToString();
                    enrollment.ReportCardFileName = dr["ReportCardFileName"].ToString();
                    enrollment.SchoolYearStart = dr["SchoolYearStart"].ToString();
                    enrollment.CourseId = (int)dr["CourseId"];
                    enrollment.Status = dr["Status"].ToString();
                    enrollment.StudentId = (int)dr["StudentId"];
                    enrollment.Type =dr["Type"].ToString();

                    //Save the student enrolled
                    StudentsModel student = new StudentsModel();
                    student.FirstName = dr["studentFirstName"].ToString();
                    student.MiddleName = dr["studentMiddleName"].ToString();
                    student.LastName = dr["studentLastName"].ToString();
                    student.Address = dr["studentAddress"].ToString();
                    student.ContactNumber = dr["studentContactNumber"].ToString();
                    student.Email = dr["studentEmail"].ToString();
                    student.Gender = dr["studentGender"].ToString() == "2" ? "Female" : "Male";
                    student.Age = (int)dr["studentAge"];
                    student.ProfileFileName = dr["studentProfileFileName"].ToString();

                    enrollment.studentModel = student;
                    enrollments.Add(enrollment);

                }
            }
            con.Close();
            ViewBag.Enrollments = JsonConvert.SerializeObject(enrollments);
            ViewBag.EnrollmentPreviews = JsonConvert.SerializeObject(enrollmentPreviews);
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

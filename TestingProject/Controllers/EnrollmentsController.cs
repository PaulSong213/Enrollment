using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            com.CommandText = $"SELECT * FROM [dbo].[enrollments] AS enrollments INNER JOIN (select id, name as courseName, acronym as courseAcronym from courses) [courses] ON [enrollments].[courseId] = [courses].[id] INNER JOIN (select id, birthDate as studentBirthDate, firstName as studentFirstName, middleName as studentMiddleName, lastName as studentLastName, gender as studentGender, age as studentAge, address as studentAddress, contactNumber as studentContactNumber, email as studentEmail, profileFileName as studentProfileFileName from students)  [students] ON [enrollments].[studentId] = students.id WHERE type = '{enrollmentType}' AND status = 'pending'";
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
                    enrollment.CertificateOfTransferFileName = dr["CertificateOfTransferFileName"] != DBNull.Value ? dr["CertificateOfTransferFileName"].ToString() : "";
                    enrollment.GoodMoralCertificateFileName = dr["goodMoralCertificateFileName"] != DBNull.Value ? dr["GoodMoralFileName"].ToString() : "";
                    enrollment.HonorableDismissalFileName = dr["HonorableDismissalFileName"] != DBNull.Value ? dr["HonorableDismissalFileName"].ToString() : "";
                    enrollment.ProfileFileName = dr["ProfileFileName"].ToString();
                    enrollment.ReportCardFileName = dr["ReportCardFileName"].ToString();
                    enrollment.SchoolYearStart = (System.DateTime)dr["SchoolYearStart"];
                    enrollment.CourseId = (int)dr["CourseId"];
                    enrollment.Status = dr["Status"].ToString();
                    enrollment.StudentId = (int)dr["StudentId"];
                    enrollment.Year = (int)dr["Year"];
                    enrollment.Type = dr["Type"].ToString();

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
                    student.BirthDate = (System.DateTime)dr["studentBirthDate"];

                    enrollment.studentsModel = student;
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

        [HttpGet]
        public ActionResult Confirmation(int id)
        {
            EnrollmentsModel enrollment = new EnrollmentsModel();


            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[enrollments] AS enrollments INNER JOIN (select id, name as courseName, acronym as courseAcronym from courses) [courses] ON [enrollments].[courseId] = [courses].[id] INNER JOIN (select id, birthDate as studentBirthDate ,firstName as studentFirstName, middleName as studentMiddleName, lastName as studentLastName, gender as studentGender, age as studentAge, address as studentAddress, contactNumber as studentContactNumber, email as studentEmail, profileFileName as studentProfileFileName from students)  [students] ON [enrollments].[studentId] = students.id INNER JOIN (select id, firstName as registrarFirstName, middleName as registrarMiddleName, lastName as registrarLastName, profileFileName as registrarProfileFileName from registrars) [registrars] ON [enrollments].[registrarEvaluatorId] = [registrars].[id] WHERE enrollments.id = {id} AND status != 'pending'";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {

                    //Save the whole enrollment information
                    enrollment.Id = (int)dr["id"];
                    enrollment.BirthCertificateFileName = dr["BirthCertificateFileName"] != DBNull.Value ? dr["BirthCertificateFileName"].ToString() : "";
                    enrollment.DateEnrolled = dr["DateEnrolled"] != DBNull.Value ? dr["DateEnrolled"].ToString() : "";
                    enrollment.CertificateOfTransferFileName = dr["CertificateOfTransferFileName"] != DBNull.Value ? dr["CertificateOfTransferFileName"].ToString() : "";
                    enrollment.GoodMoralCertificateFileName = dr["goodMoralCertificateFileName"] != DBNull.Value ? dr["GoodMoralFileName"].ToString() : "";
                    enrollment.HonorableDismissalFileName = dr["HonorableDismissalFileName"] != DBNull.Value ? dr["HonorableDismissalFileName"].ToString() : "";
                    enrollment.ProfileFileName = dr["ProfileFileName"].ToString();
                    enrollment.ReportCardFileName = dr["ReportCardFileName"].ToString();
                    enrollment.SchoolYearStart = (System.DateTime)dr["SchoolYearStart"];
                    enrollment.CourseId = (int)dr["CourseId"];
                    enrollment.Status = dr["Status"].ToString();
                    enrollment.StudentId = (int)dr["StudentId"];
                    enrollment.Year = (int)dr["Year"];
                    enrollment.Type = dr["Type"].ToString();
                    enrollment.Section = (int)dr["Section"];

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
                    student.BirthDate = (System.DateTime)dr["studentBirthDate"];

                    //save registrar
                    RegistrarsModel registrar = new RegistrarsModel();
                    registrar.FirstName = dr["registrarFirstName"].ToString();
                    registrar.MiddleName = dr["registrarMiddleName"].ToString();
                    registrar.LastName = dr["registrarLastName"].ToString();
                    registrar.ProfileFileName = dr["registrarProfileFileName"].ToString();

                    //save course information
                    CoursesModel course = new CoursesModel();
                    course.Acronym = dr["courseAcronym"].ToString();
                    course.Name = dr["courseName"].ToString();

                    enrollment.coursesModel = course;
                    enrollment.registrarsModel = registrar;
                    enrollment.studentsModel = student;

                }
            }
            else
            {
                return this.RedirectToAction("Index", "Home");
            }
            con.Close();
            ViewBag.Enrollment = enrollment;
            return View();
        }

        [HttpPost]
        public ActionResult Accept(EnrollmentsModel model)
        {
            var htmlString = "<h1>test</h1>";
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("enrollmentsystemproject@gmail.com");
                message.To.Add(new MailAddress("greatpaul321@gmail.com"));
                message.Subject = "Test";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = htmlString;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("enrollmentsystemproject@gmail.com", "P@$$w0rd12345!");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception e)
            {
                throw e;
            }
            return View("Index");
        }





    }
}

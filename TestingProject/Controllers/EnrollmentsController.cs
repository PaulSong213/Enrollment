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
                    enrollment.EmailRecipient = dr["EmailRecipient"].ToString();

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
            com.CommandText = $"SELECT * FROM [dbo].[enrollments] AS enrollments INNER JOIN (select id, name as courseName, acronym as courseAcronym from courses) [courses] ON [enrollments].[courseId] = [courses].[id] INNER JOIN (select id, birthDate as studentBirthDate , firstName as studentFirstName, middleName as studentMiddleName, lastName as studentLastName, gender as studentGender, age as studentAge, address as studentAddress, contactNumber as studentContactNumber, email as studentEmail, profileFileName as studentProfileFileName from students)  [students] ON [enrollments].[studentId] = students.id INNER JOIN (select id, firstName as registrarFirstName, middleName as registrarMiddleName, lastName as registrarLastName, profileFileName as registrarProfileFileName from registrars) [registrars] ON [enrollments].[registrarEvaluatorId] = [registrars].[id] WHERE enrollments.id = 1 AND enrollments.[status] != 'pending';";
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
        [ValidateInput(false)]
        public ActionResult Accept(FormCollection collection)
        {
            var htmlString = getEmailString(true, collection);
            try
            {
                MailMessage message = new MailMessage();
                using (SmtpClient smtp = new SmtpClient())
                {
                    message.From = new MailAddress("enrollmentsystemproject@gmail.com");
                    message.To.Add(new MailAddress(collection["emailRecipient"]));
                    message.Subject = "Accepted Enrollment Application - CvSU Bacoor ";
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

                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE enrollments SET registrarEvaluatorId = 1, section = {collection["section"]}, status = 'accepted' WHERE id= {collection["enrollmentId"]};";

                dr = com.ExecuteReader();
                con.Close();
                TempData["MessageResult"] = $"Enrollment application with id: {collection["enrollmentId"]} was marked as accepted and Email notification was sent to the student.";
            }
            catch (Exception e)
            {
                TempData["MessageResult"] = e.Message;
            }
            return RedirectToAction("Index", "Enrollments");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Reject(FormCollection collection)
        {
            var htmlString = getEmailString(false, collection);
            try
            {
                MailMessage message = new MailMessage();
                using (SmtpClient smtp = new SmtpClient())
                {
                    message.From = new MailAddress("enrollmentsystemproject@gmail.com");
                    message.To.Add(new MailAddress(collection["emailRecipient"]));
                    message.Subject = "Accepted Enrollment Application - CvSU Bacoor ";
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

                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE enrollments SET registrarEvaluatorId = 1, status = 'rejected' WHERE id= {collection["enrollmentId"]};";

                dr = com.ExecuteReader();
                con.Close();
                TempData["MessageResult"] = $"Enrollment application with id: {collection["enrollmentId"]} was wasmarked as rejection and Email notification was sent to the student.";
            }
            catch (Exception e)
            {
                TempData["MessageResult"] = e.Message;
            }
            return RedirectToAction("Index", "Enrollments");
        }

        public ActionResult Test()
        {
            return View();
        }

        public string getEmailString(Boolean isAccepted, FormCollection collection)
        {
            var studentName = collection["studentName"];
            var enrollmentId = collection["enrollmentId"];
            var registrarMessage = collection["registrarMessage"];
            if(registrarMessage == "")
            {
                registrarMessage = "None";
            }
            string domainName = Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
            //var domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            var registrationCertificateLinks = $"{domainName}/Enrollments/Confirmation/{enrollmentId}";
            var classScheaduleLink = "https://docs.google.com/document/d/13b08LolVaE7YlGe0U7pu2bJyJKDgIQs71LdqoO6s_r4/edit?usp=sharing";
            if (isAccepted)
            {
                return "<!DOCTYPE html>\n<html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>\n\n<head>\n    <meta charset='utf-8' />\n    <!-- utf-8 works for most cases -->\n    <meta name='viewport' content='width=device-width' />\n    <!-- Forcing initial-scale shouldn't be necessary -->\n    <meta http-equiv='X-UA-Compatible' content='IE=edge' />\n    <!-- Use the latest (edge) version of IE rendering engine -->\n    <meta name='x-apple-disable-message-reformatting' />\n    <!-- Disable auto-scale in iOS 10 Mail entirely -->\n    <title></title>\n    <!-- The title tag shows in email notifications, like Android 4.4. -->\n\n    <link href='https://fonts.googleapis.com/css?family=Lato:300,400,700' rel='stylesheet' />\n\n    <!-- CSS Reset : BEGIN -->\n    <style>\n        /* What it does: Remove spaces around the email design added by some email clients. */\n        /* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */\n        \n        html,\n        body {\n            margin: 0 auto !important;\n            padding: 0 !important;\n            height: 100% !important;\n            width: 100% !important;\n            background: #f1f1f1;\n        }\n        /* What it does: Stops email clients resizing small text. */\n        \n        * {\n            -ms-text-size-adjust: 100%;\n            -webkit-text-size-adjust: 100%;\n        }\n        /* What it does: Centers email on Android 4.4 */\n        \n        div[style*='margin: 16px 0'] {\n            margin: 0 !important;\n        }\n        /* What it does: Stops Outlook from adding extra spacing to tables. */\n        \n        table,\n        td {\n            mso-table-lspace: 0pt !important;\n            mso-table-rspace: 0pt !important;\n        }\n        /* What it does: Fixes webkit padding issue. */\n        \n        table {\n            border-spacing: 0 !important;\n            border-collapse: collapse !important;\n            table-layout: fixed !important;\n            margin: 0 auto !important;\n        }\n        /* What it does: Uses a better rendering method when resizing images in IE. */\n        \n        img {\n            -ms-interpolation-mode: bicubic;\n        }\n        /* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */\n        \n        a {\n            text-decoration: none;\n        }\n        /* What it does: A work-around for email clients meddling in triggered links. */\n        \n        *[x-apple-data-detectors],\n        /* iOS */\n        \n        .unstyle-auto-detected-links *,\n        .aBn {\n            border-bottom: 0 !important;\n            cursor: default !important;\n            color: inherit !important;\n            text-decoration: none !important;\n            font-size: inherit !important;\n            font-family: inherit !important;\n            font-weight: inherit !important;\n            line-height: inherit !important;\n        }\n        /* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */\n        \n        .a6S {\n            display: none !important;\n            opacity: 0.01 !important;\n        }\n        /* What it does: Prevents Gmail from changing the text color in conversation threads. */\n        \n        .im {\n            color: inherit !important;\n        }\n        /* If the above doesn't work, add a .g-img class to any image in question. */\n        \n        img.g-img+div {\n            display: none !important;\n        }\n        \n        .primary {\n            background: #30e3ca;\n        }\n        \n        .bg_white {\n            background: #ffffff;\n        }\n        \n        .bg_light {\n            background: #fafafa;\n        }\n        \n        .bg_black {\n            background: #000000;\n        }\n        \n        .bg_dark {\n            background: rgba(0, 0, 0, 0.8);\n        }\n        \n        .email-section {\n            padding: 2.5em;\n        }\n        /*BUTTON*/\n        \n        .btn {\n            padding: 10px 15px;\n            display: inline-block;\n        }\n        \n        .btn.btn-primary {\n            border-radius: 5px;\n            background: #1f8709;\n            color: #ffffff;\n        }\n        \n        .btn.btn-secondary {\n            border-radius: 5px;\n            background: #73716d;\n            color: #ffffff;\n        }\n        \n        .btn.btn-white {\n            border-radius: 5px;\n            background: #ffffff;\n            color: #000000;\n        }\n        \n        .btn.btn-white-outline {\n            border-radius: 5px;\n            background: transparent;\n            border: 1px solid #fff;\n            color: #fff;\n        }\n        \n        .btn.btn-black-outline {\n            border-radius: 0px;\n            background: transparent;\n            border: 2px solid #000;\n            color: #000;\n            font-weight: 700;\n        }\n        \n        h1,\n        h2,\n        h3,\n        h4,\n        h5,\n        h6 {\n            font-family: 'Lato', sans-serif;\n            color: #000000;\n            margin-top: 0;\n            font-weight: 400;\n        }\n        \n        body {\n            font-family: 'Lato', sans-serif;\n            font-weight: 400;\n            font-size: 15px;\n            line-height: 1.8;\n            color: rgba(0, 0, 0, 0.4);\n        }\n        \n        a {\n            color: #30e3ca;\n        }\n        \n        table {}\n        /*LOGO*/\n        \n        .logo h1 {\n            margin: 0;\n        }\n        \n        .logo h1 a {\n            color: #30e3ca;\n            font-size: 24px;\n            font-weight: 700;\n            font-family: 'Lato', sans-serif;\n        }\n        /*HERO*/\n        \n        .hero {\n            position: relative;\n            z-index: 0;\n        }\n        \n        .hero .text {\n            color: rgba(0, 0, 0, 0.3);\n        }\n        \n        .hero .text h2 {\n            color: #000;\n            font-size: 36px;\n            margin-bottom: 0;\n            font-weight: 400;\n            line-height: 1.4;\n        }\n        \n        .hero .text h3 {\n            font-size: 20px;\n            font-weight: 300;\n        }\n        \n        .hero .text h2 span {\n            font-weight: 600;\n            color: #2dad13;\n        }\n        /*HEADING SECTION*/\n        \n        .heading-section {}\n        \n        .heading-section h2 {\n            color: #000000;\n            font-size: 28px;\n            margin-top: 0;\n            line-height: 1.4;\n            font-weight: 400;\n        }\n        \n        .heading-section .subheading {\n            margin-bottom: 20px !important;\n            display: inline-block;\n            font-size: 13px;\n            text-transform: uppercase;\n            letter-spacing: 2px;\n            color: rgba(0, 0, 0, 0.4);\n            position: relative;\n        }\n        \n        .heading-section .subheading::after {\n            position: absolute;\n            left: 0;\n            right: 0;\n            bottom: -10px;\n            content: '';\n            width: 100%;\n            height: 2px;\n            background: #30e3ca;\n            margin: 0 auto;\n        }\n        \n        .heading-section-white {\n            color: rgba(255, 255, 255, 0.8);\n        }\n        \n        .heading-section-white h2 {\n            color: #ffffff;\n        }\n        \n        .heading-section-white .subheading {\n            margin-bottom: 0;\n            display: inline-block;\n            font-size: 13px;\n            text-transform: uppercase;\n            letter-spacing: 2px;\n            color: rgba(255, 255, 255, 0.4);\n        }\n        \n        ul.social {\n            padding: 0;\n        }\n        \n        ul.social li {\n            display: inline-block;\n            margin-right: 10px;\n        }\n        /*FOOTER*/\n        \n        .footer {\n            border-top: 1px solid rgba(0, 0, 0, 0.05);\n            color: rgba(0, 0, 0, 0.5);\n        }\n        \n        .footer .heading {\n            color: #000;\n            font-size: 20px;\n        }\n        \n        .footer ul {\n            margin: 0;\n            padding: 0;\n        }\n        \n        .footer ul li {\n            list-style: none;\n            margin-bottom: 10px;\n        }\n        \n        .footer ul li a {\n            color: rgba(0, 0, 0, 1);\n        }\n    </style>\n</head>\n\n<body width='100%' style='\n      margin: 0;\n      padding: 0 !important;\n      mso-line-height-rule: exactly;\n      background-color: #f1f1f1;\n    '>\n    <center style='width: 100%; background-color: #f1f1f1'>\n        <div style='\n          display: none;\n          font-size: 1px;\n          max-height: 0px;\n          max-width: 0px;\n          opacity: 0;\n          overflow: hidden;\n          mso-hide: all;\n          font-family: sans-serif;\n        '>\n            &zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;\n        </div>\n        <div style='max-width: 600px; margin: 0 auto' class='email-container'>\n            <!-- BEGIN BODY -->\n            <table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto'>\n                <tr>\n                    <td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em'>\n                        <table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>\n                            <tr>\n                                <td class='logo' style='text-align: center'>\n                                    <img src='https://drive.google.com/uc?export=view&id=1atfI0j52zn39I2F07LLfeZz7zLAbYJtH' ' alt='Logo ' title='Logo ' style='display:block; margin: auto ' width='400 ' height='200 ' />\n                  </td>\n                </tr>\n              </table>\n            </td>\n          </tr>\n          <!-- end tr -->\n          <tr></tr>\n          <!-- end tr -->\n          <tr>\n            <td\n              valign='middle '\n              class='hero bg_white '\n              style='padding: 2em 0 4em 0 '\n            >\n              <table>\n                <tr>\n                  <td>\n                    <div class='text ' style='padding: 1rem; text-align: center '>\n                      <h2>\n                        <bold style='font-weight: 600; margin-right: 3px '>\n                          " + studentName + "\n                        </bold>\n                        your Enrollment Application has been\n                        <span>APPROVED</span>.\n                      </h2>\n                      <h3>\n                        You can download your enrollment confirmation,\n                        enrollment registration, and scheadule below.\n                      </h3>\n                      <div\n                        style=' padding-top: 2rem; '\n                      >\n                        <p style='margin-top: 0; display: block '>\n                          <a\n                            href=' "+registrationCertificateLinks+"?download=true '\n                            style='width: 80% '\n                            class='btn btn-primary '\n                            >Download your Enrollment Confirmation &\n                            Registration Form</a\n                          >\n                        </p>\n                        <p style='margin-top: 0;display: block '>\n                          <a\n                            href=' "+registrationCertificateLinks+" '\n                            style='width: 80% '\n                            class='btn btn-secondary '\n                            >View your Enrollment Confirmation & Registration\n                            Form</a\n                          >\n                        </p>\n                        <p style='margin-top: 0;display: block '>\n                          <a\n                            href=' "+classScheaduleLink+" \" '\n                            style='width: 80%; background-color: #1f69d1 '\n                            class='btn btn-primary '\n                            >View Class Scheadule</a\n                          >\n                        </p>\n                      </div>\n\n                      <div style='background-color: rgba(26, 142, 219, 0.1);padding: 15px;margin-top: 50px; text-align:left;color:rgba(0, 0, 0, 0.8)'>\n                        <h3 style='font-weight: 700'>Registar Message:</h3>\n                        <h4>"+registrarMessage+ "</h4>\n                      </div>\n\n                      <div style='background-color: rgba(224, 227, 64, 0.4);padding: 15px; text-align:left;color:rgba(0, 0, 0, 0.8); margin-top: 20px'>\n                        <div>\n                          <h3 style='font-weight: 700'>NOTE: If the buttons above does not work. You may follow this links:</h3>\n                        </div>\n                        <div>\n                          <label style='font-weight: 700'>Download your enrolment Confirmation & Registration form:</label>\n                          <h4>"+registrationCertificateLinks+ "?download=true</h4>\n                        </div>\n                        <div >\n                          <label style='font-weight: 700'>Download your Class Scheadule</label>\n                          <h4>"+classScheaduleLink+ "</h4>\n                        </div>\n                        <div >\n                          <label style='font-weight: 700'>View your Class Scheadule</label>\n                          <h4>"+registrationCertificateLinks+ "</h4>\n                        </div>\n                      </div>\n                    </div>\n                  </td>\n                </tr>\n              </table>\n            </td>\n          </tr>\n          <!-- end tr -->\n          <!-- 1 Column Text + Button : END -->\n        </table>\n        <table\n          align='center '\n          role='presentation '\n          cellspacing='0 '\n          cellpadding='0 '\n          border='0 '\n          width='100% '\n          style='margin: auto '\n        >\n          <tr>\n            <td valign='middle ' class='bg_light footer email-section '>\n              <table>\n                <tr>\n                  <td valign='top ' width='33.333% ' style='padding-top: 20px '>\n                    <table\n                      role='presentation '\n                      cellspacing='0 '\n                      cellpadding='0 '\n                      border='0 '\n                      width='100% '\n                    >\n                      <tr>\n                        <td style='text-align: left; padding-right: 10px '>\n                          <h3 class='heading '>About</h3>\n                          <p>\n                            Cavite State University is the premier university of\n                            the province of Cavite in the Philippines. Its\n                            70-hectare main campus, known as the Don Severino\n                            delas Alas Campus.\n                          </p>\n                        </td>\n                      </tr>\n                    </table>\n                  </td>\n                  <td valign='top ' width='33.333% ' style='padding-top: 20px '>\n                    <table\n                      role='presentation '\n                      cellspacing='0 '\n                      cellpadding='0 '\n                      border='0 '\n                      width='100% '\n                    >\n                      <tr>\n                        <td\n                          style=' text-align: left; padding-left: 5px; padding-right: 5px; '\n                        >\n                          <h3 class='heading '>Contact Info</h3>\n                          <ul>\n                            <li>\n                              <span class='text '\n                                >Phase II Soldiers Hills IV, Molino VI, City of\n                                Bacoor</span\n                              >\n                            </li>\n                            <li><span class='text '>(046) 476 – 5029</span></li>\n                          </ul>\n                        </td>\n                      </tr>\n                    </table>\n                  </td>\n                  <td valign='top ' width='33.333% ' style='padding-top: 20px '>\n                    <table\n                      role='presentation '\n                      cellspacing='0 '\n                      cellpadding='0 '\n                      border='0 '\n                      width='100% '\n                    >\n                      <tr>\n                        <td style='text-align: left; padding-left: 10px '>\n                          <h3 class='heading '>Useful Links</h3>\n                          <ul>\n                            <li>\n                              <a\n                                href='https://web.facebook.com/CvSU.B.Admission/?_rdc=1&_rdr '\n                                >Facebook</a\n                              >\n                            </li>\n                            <li>\n                              <a\n                                href='mailto:enrollmentsystemproject@gmail.com '\n                                >Email</a\n                              >\n                            </li>\n                            <li><a href='https://cvsu.edu.ph/ '>Website</a></li>\n                          </ul>\n                        </td>\n                      </tr>\n                    </table>\n                  </td>\n                </tr>\n              </table>\n            </td>\n          </tr>\n          <!-- end: tr -->\n        </table>\n      </div>\n    </center>\n  </body>\n</html>";
            }
            else
            {
                return "<!DOCTYPE html>\n<html lang='en' xmlns='http://www.w3.org/1999/xhtml' xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office'>\n\n<head>\n    <meta charset='utf-8' />\n    <!-- utf-8 works for most cases -->\n    <meta name='viewport' content='width=device-width' />\n    <!-- Forcing initial-scale shouldn't be necessary -->\n    <meta http-equiv='X-UA-Compatible' content='IE=edge' />\n    <!-- Use the latest (edge) version of IE rendering engine -->\n    <meta name='x-apple-disable-message-reformatting' />\n    <!-- Disable auto-scale in iOS 10 Mail entirely -->\n    <title></title>\n    <!-- The title tag shows in email notifications, like Android 4.4. -->\n\n    <link href='https://fonts.googleapis.com/css?family=Lato:300,400,700' rel='stylesheet' />\n\n    <!-- CSS Reset : BEGIN -->\n    <style>\n        /* What it does: Remove spaces around the email design added by some email clients. */\n        /* Beware: It can remove the padding / margin and add a background color to the compose a reply window. */\n        \n        html,\n        body {\n            margin: 0 auto !important;\n            padding: 0 !important;\n            height: 100% !important;\n            width: 100% !important;\n            background: #f1f1f1;\n        }\n        /* What it does: Stops email clients resizing small text. */\n        \n        * {\n            -ms-text-size-adjust: 100%;\n            -webkit-text-size-adjust: 100%;\n        }\n        /* What it does: Centers email on Android 4.4 */\n        \n        div[style*='margin: 16px 0'] {\n            margin: 0 !important;\n        }\n        /* What it does: Stops Outlook from adding extra spacing to tables. */\n        \n        table,\n        td {\n            mso-table-lspace: 0pt !important;\n            mso-table-rspace: 0pt !important;\n        }\n        /* What it does: Fixes webkit padding issue. */\n        \n        table {\n            border-spacing: 0 !important;\n            border-collapse: collapse !important;\n            table-layout: fixed !important;\n            margin: 0 auto !important;\n        }\n        /* What it does: Uses a better rendering method when resizing images in IE. */\n        \n        img {\n            -ms-interpolation-mode: bicubic;\n        }\n        /* What it does: Prevents Windows 10 Mail from underlining links despite inline CSS. Styles for underlined links should be inline. */\n        \n        a {\n            text-decoration: none;\n        }\n        /* What it does: A work-around for email clients meddling in triggered links. */\n        \n        *[x-apple-data-detectors],\n        /* iOS */\n        \n        .unstyle-auto-detected-links *,\n        .aBn {\n            border-bottom: 0 !important;\n            cursor: default !important;\n            color: inherit !important;\n            text-decoration: none !important;\n            font-size: inherit !important;\n            font-family: inherit !important;\n            font-weight: inherit !important;\n            line-height: inherit !important;\n        }\n        /* What it does: Prevents Gmail from displaying a download button on large, non-linked images. */\n        \n        .a6S {\n            display: none !important;\n            opacity: 0.01 !important;\n        }\n        /* What it does: Prevents Gmail from changing the text color in conversation threads. */\n        \n        .im {\n            color: inherit !important;\n        }\n        /* If the above doesn't work, add a .g-img class to any image in question. */\n        \n        img.g-img+div {\n            display: none !important;\n        }\n        \n        .primary {\n            background: #30e3ca;\n        }\n        \n        .bg_white {\n            background: #ffffff;\n        }\n        \n        .bg_light {\n            background: #fafafa;\n        }\n        \n        .bg_black {\n            background: #000000;\n        }\n        \n        .bg_dark {\n            background: rgba(0, 0, 0, 0.8);\n        }\n        \n        .email-section {\n            padding: 2.5em;\n        }\n        /*BUTTON*/\n        \n        .btn {\n            padding: 10px 15px;\n            display: inline-block;\n        }\n        \n        .btn.btn-primary {\n            border-radius: 5px;\n            background: #1f8709;\n            color: #ffffff;\n        }\n        \n        .btn.btn-secondary {\n            border-radius: 5px;\n            background: #73716d;\n            color: #ffffff;\n        }\n        \n        .btn.btn-white {\n            border-radius: 5px;\n            background: #ffffff;\n            color: #000000;\n        }\n        \n        .btn.btn-white-outline {\n            border-radius: 5px;\n            background: transparent;\n            border: 1px solid #fff;\n            color: #fff;\n        }\n        \n        .btn.btn-black-outline {\n            border-radius: 0px;\n            background: transparent;\n            border: 2px solid #000;\n            color: #000;\n            font-weight: 700;\n        }\n        \n        h1,\n        h2,\n        h3,\n        h4,\n        h5,\n        h6 {\n            font-family: 'Lato', sans-serif;\n            color: #000000;\n            margin-top: 0;\n            font-weight: 400;\n        }\n        \n        body {\n            font-family: 'Lato', sans-serif;\n            font-weight: 400;\n            font-size: 15px;\n            line-height: 1.8;\n            color: rgba(0, 0, 0, 0.4);\n        }\n        \n        a {\n            color: #30e3ca;\n        }\n        \n        table {}\n        /*LOGO*/\n        \n        .logo h1 {\n            margin: 0;\n        }\n        \n        .logo h1 a {\n            color: #30e3ca;\n            font-size: 24px;\n            font-weight: 700;\n            font-family: 'Lato', sans-serif;\n        }\n        /*HERO*/\n        \n        .hero {\n            position: relative;\n            z-index: 0;\n        }\n        \n        .hero .text {\n            color: rgba(0, 0, 0, 0.3);\n        }\n        \n        .hero .text h2 {\n            color: #000;\n            font-size: 36px;\n            margin-bottom: 0;\n            font-weight: 400;\n            line-height: 1.4;\n        }\n        \n        .hero .text h3 {\n            font-size: 20px;\n            font-weight: 300;\n        }\n        \n        .hero .text h2 span {\n            font-weight: 600;\n            color: #aa3f18;\n        }\n        /*HEADING SECTION*/\n        \n        .heading-section h2 {\n            color: #000000;\n            font-size: 28px;\n            margin-top: 0;\n            line-height: 1.4;\n            font-weight: 400;\n        }\n        \n        .heading-section .subheading {\n            margin-bottom: 20px !important;\n            display: inline-block;\n            font-size: 13px;\n            text-transform: uppercase;\n            letter-spacing: 2px;\n            color: rgba(0, 0, 0, 0.4);\n            position: relative;\n        }\n        \n        .heading-section .subheading::after {\n            position: absolute;\n            left: 0;\n            right: 0;\n            bottom: -10px;\n            content: '';\n            width: 100%;\n            height: 2px;\n            background: #30e3ca;\n            margin: 0 auto;\n        }\n        \n        .heading-section-white {\n            color: rgba(255, 255, 255, 0.8);\n        }\n        \n        .heading-section-white h2 {\n            color: #ffffff;\n        }\n        \n        .heading-section-white .subheading {\n            margin-bottom: 0;\n            display: inline-block;\n            font-size: 13px;\n            text-transform: uppercase;\n            letter-spacing: 2px;\n            color: rgba(255, 255, 255, 0.4);\n        }\n        \n        ul.social {\n            padding: 0;\n        }\n        \n        ul.social li {\n            display: inline-block;\n            margin-right: 10px;\n        }\n        /*FOOTER*/\n        \n        .footer {\n            border-top: 1px solid rgba(0, 0, 0, 0.05);\n            color: rgba(0, 0, 0, 0.5);\n        }\n        \n        .footer .heading {\n            color: #000;\n            font-size: 20px;\n        }\n        \n        .footer ul {\n            margin: 0;\n            padding: 0;\n        }\n        \n        .footer ul li {\n            list-style: none;\n            margin-bottom: 10px;\n        }\n        \n        .footer ul li a {\n            color: rgba(0, 0, 0, 1);\n        }\n    </style>\n</head>\n\n<body width='100%' style='\n      margin: 0;\n      padding: 0 !important;\n      mso-line-height-rule: exactly;\n      background-color: #f1f1f1;\n    '>\n    <center style='width: 100%; background-color: #f1f1f1'>\n        <div style='\n          display: none;\n          font-size: 1px;\n          max-height: 0px;\n          max-width: 0px;\n          opacity: 0;\n          overflow: hidden;\n          mso-hide: all;\n          font-family: sans-serif;\n        '>\n            &zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;&zwnj;&nbsp;\n        </div>\n        <div style='max-width: 600px; margin: 0 auto' class='email-container'>\n            <!-- BEGIN BODY -->\n            <table align='center' role='presentation' cellspacing='0' cellpadding='0' border='0' width='100%' style='margin: auto'>\n                <tr>\n                    <td valign='top' class='bg_white' style='padding: 1em 2.5em 0 2.5em'>\n                        <table role='presentation' border='0' cellpadding='0' cellspacing='0' width='100%'>\n                            <tr>\n                                <td class='logo' style='text-align: center'>\n                                    <img src='https://drive.google.com/uc?export=view&id=1atfI0j52zn39I2F07LLfeZz7zLAbYJtH' ' alt='Logo ' title='Logo ' style='display:block; margin: auto ' width='400 ' height='200 ' />\n                  </td>\n                </tr>\n              </table>\n            </td>\n          </tr>\n          <!-- end tr -->\n          <tr></tr>\n          <!-- end tr -->\n          <tr>\n            <td\n              valign='middle '\n              class='hero bg_white '\n              style='padding: 2em 0 4em 0 '\n            >\n              <table>\n                <tr>\n                  <td>\n                    <div class='text ' style='padding: 1rem; text-align: center '>\n                      <h2>\n                        <bold style='font-weight: 600; margin-right: 3px '>\n                          " + studentName + "\n                        </bold>\n                        your Enrollment Application has been\n                        <span>REJECTED</span>.\n                      </h2>\n                      <h3>\n                        For further question email us at : enrollmentsystemproject@gmail.com \n                      </h3>\n                      \n                      <div style='background-color: rgba(26, 142, 219, 0.1);padding: 15px;margin-top: 50px; text-align:left;color:rgba(0, 0, 0, 0.8) '>\n                        <h3 style='font-weight: 700 '>Reason of Rejection: </h3>\n                        <h4>" + registrarMessage + " </h4>\n                      </div>\n                      \n                    </div>\n                  </td>\n                </tr>\n              </table>\n            </td>\n          </tr>\n          <!-- end tr -->\n          <!-- 1 Column Text + Button : END -->\n        </table>\n        <table\n          align='center '\n          role='presentation '\n          cellspacing='0 '\n          cellpadding='0 '\n          border='0 '\n          width='100% '\n          style='margin: auto '\n        >\n          <tr>\n            <td valign='middle ' class='bg_light footer email-section '>\n              <table>\n                <tr>\n                  <td valign='top ' width='33.333% ' style='padding-top: 20px '>\n                    <table\n                      role='presentation '\n                      cellspacing='0 '\n                      cellpadding='0 '\n                      border='0 '\n                      width='100% '\n                    >\n                      <tr>\n                        <td style='text-align: left; padding-right: 10px '>\n                          <h3 class='heading '>About</h3>\n                          <p>\n                            Cavite State University is the premier university of\n                            the province of Cavite in the Philippines. Its\n                            70-hectare main campus, known as the Don Severino\n                            delas Alas Campus.\n                          </p>\n                        </td>\n                      </tr>\n                    </table>\n                  </td>\n                  <td valign='top ' width='33.333% ' style='padding-top: 20px '>\n                    <table\n                      role='presentation '\n                      cellspacing='0 '\n                      cellpadding='0 '\n                      border='0 '\n                      width='100% '\n                    >\n                      <tr>\n                        <td\n                          style=' text-align: left; padding-left: 5px; padding-right: 5px; '\n                        >\n                          <h3 class='heading '>Contact Info</h3>\n                          <ul>\n                            <li>\n                              <span class='text '\n                                >Phase II Soldiers Hills IV, Molino VI, City of\n                                Bacoor</span\n                              >\n                            </li>\n                            <li><span class='text '>(046) 476 – 5029</span></li>\n                          </ul>\n                        </td>\n                      </tr>\n                    </table>\n                  </td>\n                  <td valign='top ' width='33.333% ' style='padding-top: 20px '>\n                    <table\n                      role='presentation '\n                      cellspacing='0 '\n                      cellpadding='0 '\n                      border='0 '\n                      width='100% '\n                    >\n                      <tr>\n                        <td style='text-align: left; padding-left: 10px '>\n                          <h3 class='heading '>Useful Links</h3>\n                          <ul>\n                            <li>\n                              <a\n                                href='https://web.facebook.com/CvSU.B.Admission/?_rdc=1&_rdr '\n                                >Facebook</a\n                              >\n                            </li>\n                            <li>\n                              <a\n                                href='mailto:enrollmentsystemproject@gmail.com '\n                                >Email</a\n                              >\n                            </li>\n                            <li><a href='https://cvsu.edu.ph/ '>Website</a></li>\n                          </ul>\n                        </td>\n                      </tr>\n                    </table>\n                  </td>\n                </tr>\n              </table>\n            </td>\n          </tr>\n          <!-- end: tr -->\n        </table>\n      </div>\n    </center>\n  </body>\n</html>";
            }
        }



    }
}

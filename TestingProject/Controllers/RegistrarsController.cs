using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EnrollmentSystem.Models;
using Firebase.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;

namespace EnrollmentSystem.Controllers
{
    public class RegistrarsController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        public ActionResult Analytics()
        {
            List<CoursesModel> courses = new List<CoursesModel>();
            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[courses]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    CoursesModel course = new CoursesModel();
                    course.Id = (int)dr["id"];
                    course.Acronym = dr["Acronym"].ToString();
                    course.Name = dr["Name"].ToString();
                    course.Slots = (int)dr["Slots"];
                    courses.Add(course);
                }
            }
            con.Close();
            ViewBag.Courses = JsonConvert.SerializeObject(courses);
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<RegistrarsModel> registrars= new List<RegistrarsModel>();
            List<RegistrarsModelPreview> registrarsPreview = new List<RegistrarsModelPreview>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[registrars]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    //save preview of registrar
                    RegistrarsModelPreview registrarPreview = new RegistrarsModelPreview();
                    registrarPreview.Id = (int)dr["id"];
                    registrarPreview.FullName = $"{ dr["FirstName"].ToString()} { dr["MiddleName"].ToString()[0]}. { dr["LastName"].ToString()}";
                    registrarPreview.Email = dr["Email"].ToString();
                    registrarPreview.ProfileFileName = dr["ProfileFileName"].ToString();
                    registrarsPreview.Add(registrarPreview);

                    //save whole data of registrar
                    RegistrarsModel registrar = new RegistrarsModel();
                    registrar.Id = (int)dr["id"];
                    registrar.FirstName = dr["FirstName"].ToString();
                    registrar.LastName = dr["LastName"].ToString();
                    registrar.MiddleName = dr["MiddleName"].ToString();
                    registrar.Email = dr["Email"].ToString();
                    registrar.ProfileFileName = dr["ProfileFileName"].ToString();
                    registrars.Add(registrar);
                }
            }
            con.Close();
            ViewBag.Registrars = JsonConvert.SerializeObject(registrars);
            ViewBag.RegistrarPreview = JsonConvert.SerializeObject(registrarsPreview);
            return View();
        }

        [HttpGet]
        public ActionResult Security()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Security(RegistrarSecurityViewModel model)
        {
            if(model.Password == "12345")
            {
                Session["isSecuredRegistrar"] = "Secured";
                return RedirectToAction("SignUp");
            }
            ModelState.AddModelError(string.Empty, "Incorrect password.");
            return View();
        }

        [HttpPost]
        public ActionResult Add(RegistrarsModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"INSERT INTO [dbo].[registrars] (firstName,middleName,lastName,isActive,email,profileFileName) VALUES ('{model.FirstName}' , '{model.MiddleName}', '{model.LastName}', 1, '{model.Email}', 'person.jpg' )";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Course added successfully.";
                }
                else
                {
                    TempData["ErrorResult"] = "There was a problem adding course. Please try again.";
                    return View();
                }
                con.Close();

            }
            catch (Exception e)
            {
                TempData["ErrorResult"] = e.Message;
                return View();
            }
            return RedirectToAction("Index", "Registrars");
        }


        private void SignInUser(string email, string token, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();

            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Email, email));
                claims.Add(new Claim(ClaimTypes.Authentication, token));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }


    }
}
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
            List<RegistrarsModelPreview> registrars = new List<RegistrarsModelPreview>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[registrars]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    RegistrarsModelPreview registrar = new RegistrarsModelPreview();
                    registrar.Id = (int)dr["id"];
                    registrar.FirstName = dr["FirstName"].ToString();
                    registrar.LastName = dr["LastName"].ToString();
                    registrar.MiddleName = dr["MiddleName"].ToString();
                    registrars.Add(registrar);
                }
            }
            con.Close();
            ViewBag.Registrars = JsonConvert.SerializeObject(registrars);
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

        [HttpGet]
        public ActionResult SignUp()
        {

            try
            {
                // Verification.
                if (this.Request.IsAuthenticated)
                {
                    return this.RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (System.Web.HttpContext.Current.Session["isSecuredRegistrar"] == null) return RedirectToAction("Security");
            Session["isSecuredRegistrar"] = null;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(RegistrarsModel model, HttpPostedFileBase UploadedProfileFileName)
        {

            try
            {
                con.ConnectionString = new AccountController().getConnectionString();


                if (UploadedProfileFileName != null && UploadedProfileFileName.ContentLength > 0)
                {
                    //save data to database
                    con.Open();
                    com.Connection = con;
                    com.CommandText = $"INSERT INTO [dbo].[registrars] ([firstName] ,[middleName] ,[lastName] ,[email],[profileFileName] ) VALUES ('{model.FirstName}' ,'{model.MiddleName}' , '{model.LastName}' , '{model.Email}','blank.jpg' )";
                    dr = com.ExecuteReader();
                    con.Close();

                    //create firebase account
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(AccountController.ApiKey));
                    var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.FirstName, true);
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var user = ab.User;
                    this.Session["tokenEmail"] = model.Email;
                    this.Session["tokenPassword"] = model.Password;
                    this.Session["userAccountID"] = user.LocalId;
                    //Upload file
                    string imagesPath = HttpContext.Server.MapPath("~/images");
                    string extension = Path.GetExtension(UploadedProfileFileName.FileName);
                    string newFileName = user.LocalId + extension;
                    string saveToPath = Path.Combine(imagesPath, newFileName);
                    UploadedProfileFileName.SaveAs(saveToPath);

                    //save profile file and firebase account id
                    con.Open();

                    com.Connection = con;
                    com.CommandText = $"UPDATE [dbo].[registrars] SET [profileFileName] = '{newFileName}', [accountId] = '{user.LocalId}'  WHERE [email] = '{user.Email}'";
                    com.ExecuteNonQuery();
                    con.Close();

                    //redirect to verify page
                    if (token != "")
                    {
                        this.SignInUser(user.Email, token, false);
                        return this.RedirectToAction("Verify", "Account");
                    }
                    else
                    {

                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "File not selected.");
                }

            }
            catch (Exception e)
            {
                string firebaseError = "Email is invalid or already used";
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
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
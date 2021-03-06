using Firebase.Auth;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EnrollmentSystem.Models;
using System.Web.Security;
using System.ComponentModel;
using System.IO;
using System.Data.SqlClient;
using System.Web.Hosting;

namespace EnrollmentSystem.Controllers
{
    public class AccountController : Controller
    {
        public static string ApiKey = "AIzaSyB3kuislzQfHVOr-3Zw5ecu_l4QBT3KP2Q";
        public static string Bucket = "enrollment-system-fc139.appspot.com";

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        void connectionString()
        {
            con.ConnectionString = this.getConnectionString();
        }

        public string getConnectionString()
        {
            //deploy conn string
           return "Data Source=SQL8003.site4now.net;Initial Catalog=db_a8752f_enrollmentsystem;User Id=db_a8752f_enrollmentsystem_admin;Password=pogiako213";
            //return "data source=paul;initial catalog=enrollment_system;user id=sa;password=Relente1";
            //return "Data Source=DESKTOP-9R1M64D\\SQLEXPRESS;Initial Catalog=enrollment;Integrated Security=True";
            //return DotNetEnv.Env.GetString("CONN_STRING", "Data Source=DESKTOP-9R1M64D\\SQLEXPRESS;Initial Catalog=enrollment)_final;Integrated Security=True");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword()
        {
            // Info.
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                await auth.SendPasswordResetEmailAsync(model.Email);
                ViewBag.SaveResult = true;
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Email not registered.");
            }

            return this.View(model);
        }


        // GET: Verify
        [Authorize]
        public async Task<ActionResult> Verify()
        {

            if (this.Session["userType"] != null && this.Session["userType"].ToString() == "registrar")
            {
                return this.RedirectToAction("Index", "Enrollments");
            }
            else
            {

                var tokenEmail = this.Session["tokenEmail"];
                var tokenPassword = this.Session["tokenPassword"];
                if (tokenEmail == null)
                {
                    return this.RedirectToAction("LogOff", "Account");
                }
                try
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                    var ab = await auth.SignInWithEmailAndPasswordAsync(tokenEmail.ToString(), tokenPassword.ToString());
                    string token = ab.FirebaseToken;
                    await ab.RefreshUserDetails();
                    var user = ab.User;
                    ViewBag.IsEmailVerified = user.IsEmailVerified;
                    ViewBag.CurrentEmail = user.Email;
                    if (user.IsEmailVerified)
                    {
                        return this.RedirectToAction("Portal", "Student");
                    }
                    else
                    {
                        var fAuth = await ab.GetFreshAuthAsync();
                        await auth.SendEmailVerificationAsync(fAuth);
                    }
                }
                catch (Exception)
                {
                    //return this.RedirectToAction("LogOff", "Account");
                }
            }



            return View();
        }


        // GET: Account
        [HttpGet]
        public ActionResult SignUp()
        {
            if (this.Session["userType"] != null && this.Session["userType"].ToString() == "registrar")
            {
                return this.RedirectToAction("Index", "Enrollments");
            }
            else
            {
                try
                {
                    // Verification.
                    if (this.Request.IsAuthenticated)
                    {
                        return this.RedirectToAction("Verify", "Account");
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUpModel model, HttpPostedFileBase UploadedProfileFileName)
        {

                try
                {
                    this.connectionString();


                    if (UploadedProfileFileName != null && UploadedProfileFileName.ContentLength > 0)
                    {
                        //save data to database
                        con.Open();
                        com.Connection = con;

                        string sqlFormattedBirthDate = model.BirthDate.ToString("yyyy-MM-dd");

                    com.CommandText = $"INSERT INTO [dbo].[students] ([firstName] ,[middleName] ,[lastName] ,[gender] ,[age] ,[address] ,[contactNumber] ,[accountId] ,[email],[profileFileName], [isActive], [birthDate] ) VALUES ('{model.FirstName}' ,'{model.MiddleName}' , '{model.LastName}' , '{model.Gender}' , 21 , '{model.Address}' , '{model.ContactNumber}' ,'' , '{model.Email}','blank.jpg', 1, '{sqlFormattedBirthDate}' )";
                        dr = com.ExecuteReader();
                        con.Close();

                    //create firebase account
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                            var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.FirstName, true);
                            var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                            string token = ab.FirebaseToken;
                            var user = ab.User;
                            this.Session["tokenEmail"] = model.Email;
                            this.Session["tokenPassword"] = model.Password;
                            this.Session["userAccountID"] = user.LocalId;
                            //Upload file
                            string imagesPath = HttpContext.Server.MapPath("~/Content/img");
                            string extension = Path.GetExtension(UploadedProfileFileName.FileName);
                            string newFileName = user.LocalId + extension;
                            string saveToPath = Path.Combine(imagesPath, newFileName);
                            UploadedProfileFileName.SaveAs(saveToPath);

                    //save profile file and firebase account id
                            con.Open();

                            com.Connection = con;
                            com.CommandText = $"UPDATE [dbo].[students] SET [profileFileName] = '{newFileName}', [accountId] = '{user.LocalId}'  WHERE [email] = '{user.Email}'";
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            if (this.Session["userType"] != null && this.Session["userType"].ToString() == "registrar")
            {
                return this.RedirectToAction("Index", "Enrollments");
            }
            else
            {
                try
                {
                    // Verification.
                    if (this.Request.IsAuthenticated)
                    {
                        return this.RedirectToAction("Verify", "Account");
                    }
                }
                catch (Exception ex)
                {
                    // Info
                    Console.Write(ex);
                }

                // Info.
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            
            try
            {
                this.connectionString();

                //check first if registrar
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"SELECT * FROM [dbo].[registrars] where  email ='{model.Email}'";
                dr = com.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        this.Session["userType"] = "registrar";
                        this.Session["userId"] = dr["id"];
                        this.Session["userEmail"] = dr["email"];
                        this.Session["userFirstName"] = dr["firstName"];
                        this.Session["userMiddleName"] = dr["middleName"];
                        this.Session["userLastName"] = dr["lastName"];
                        this.Session["userProfileFileName"] = dr["profileFileName"];
                        return this.RedirectToAction("Index", "Enrollments");
                    }
                }
                else
                {
                    con.Close();

                    // Verification.
                    if (ModelState.IsValid)
                    {
                        var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                        var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                        string token = ab.FirebaseToken;
                        var user = ab.User;
                        this.Session["tokenEmail"] = model.Email;
                        this.Session["tokenPassword"] = model.Password;
                        this.Session["userAccountID"] = user.LocalId;
                        if (token != "")
                        {
                            this.SignInUser(user.Email, token, false);
                            return this.RedirectToAction("Verify", "Account");
                        }
                        else
                        {
                            // Setting.
                            ModelState.AddModelError(string.Empty, "Invalid email or password.");
                        }
                    }

                   
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "Invalid email or password..");
            return this.View(model);
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


                    //save user session
                    con.Open();
                    com.Connection = con;
                    com.CommandText = $"SELECT *  FROM [dbo].[students] WHERE email='{email}'";
                    dr = com.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            this.Session["userId"] = dr["id"];
                            this.Session["userEmail"] = dr["email"];
                            this.Session["userFirstName"] = dr["firstName"];
                            this.Session["userMiddleName"] = dr["middleName"];
                            this.Session["userLastName"] = dr["lastName"];
                            this.Session["userGender"] = dr["gender"].ToString() == "1" ? "Male" : "Female";
                            this.Session["userAddress"] = dr["address"];
                            this.Session["userContactNumber"] = dr["contactNumber"];
                            this.Session["userProfileFileName"] = dr["profileFileName"];
                        }
                    }
                    con.Close();

            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }

        private void ClaimIdentities(string username, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();
            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogOff()
        {
            try
            {
                con.Close();
            }
            catch (Exception)
            {

                throw;
            }
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            this.Session.Clear();
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
            
        }

    }
}
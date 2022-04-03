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

namespace EnrollmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private static string ApiKey = "AIzaSyB3kuislzQfHVOr-3Zw5ecu_l4QBT3KP2Q";
        private static string Bucket = "enrollment-system-fc139.appspot.com";

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

            var tokenEmail = this.Session["tokenEmail"];
            var tokenPassword = this.Session["tokenPassword"];
            if(tokenEmail == null)
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

            return View();
        }


        // GET: Account
        public ActionResult SignUp()
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(SignUpModel model, HttpPostedFileBase UploadedProfileFileName)
        {
            try
            {
                
                if (UploadedProfileFileName != null && UploadedProfileFileName.ContentLength > 0)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

                    var a = await auth.CreateUserWithEmailAndPasswordAsync(model.Email, model.Password, model.FirstName , true);
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var user = ab.User;
                    this.Session["tokenEmail"] = model.Email;
                    this.Session["tokenPassword"] = model.Password;
                    

                    //Upload file

                    string imagesPath = HttpContext.Server.MapPath("~/images"); // Or file save folder, etc.
                    string extension = Path.GetExtension(UploadedProfileFileName.FileName);
                    string newFileName = user.LocalId.ToString() + extension;
                    string saveToPath = Path.Combine(imagesPath, newFileName);
                    UploadedProfileFileName.SaveAs(saveToPath);

                    //redirect to verify page
                    if (token != "")
                    {
                        this.SignInUser(user.Email, token, false);
                        return this.RedirectToAction("Verify", "Account");
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
                ModelState.AddModelError(string.Empty, firebaseError);
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
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
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model)
        {

            try
            {
                // Verification.
                if (ModelState.IsValid)
                {
                    var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                    var ab = await auth.SignInWithEmailAndPasswordAsync(model.Email, model.Password);
                    string token = ab.FirebaseToken;
                    var user = ab.User;
                    this.Session["tokenEmail"] = model.Email;
                    this.Session["tokenPassword"] = model.Password;

                    if (token != "")
                    {
                        this.SignInUser(user.Email, token, false);
                        return this.RedirectToAction("Verify", "Account");
                    }
                    else
                    {
                        // Setting.
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
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
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            this.Session["tokenEmail"] = "";
            this.Session["tokenPassword"] = "";
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

    }
}
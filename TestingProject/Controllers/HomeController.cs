using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;


namespace EnrollmentSystem.Controllers
{
    public class HomeController : Controller
    {
      
        public ActionResult Index()
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
}
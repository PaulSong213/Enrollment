using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnrollmentSystem.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult Portal()
        {
            ViewBag.UserType = "student";
            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnrollmentSystem.Models;
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
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<RegistrarsModel> registrars = new List<RegistrarsModel>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [enrollment_system].[dbo].[registrars] WHERE IsActive = 1";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    RegistrarsModel registrar = new RegistrarsModel();
                    registrar.Id = (int)dr["id"];
                    registrar.FirstName = dr["FirstName"].ToString();
                    registrar.LastName = dr["LastName"].ToString();
                    registrar.MiddleName = dr["MiddleName"].ToString();
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
                return RedirectToAction("SignUp");
            }
            ModelState.AddModelError(string.Empty, "Incorrect password.");
            return View();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }


    }
}
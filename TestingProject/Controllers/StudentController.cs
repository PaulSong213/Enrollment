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
using Newtonsoft.Json;


namespace EnrollmentSystem.Controllers
{
    public class StudentController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        // GET: Student
        public ActionResult Portal()
        {
            ViewBag.UserType = "student";

            return View();
        }

        // GET: Student
        [HttpGet]
        public ActionResult Index()
        {

            List<StudentsModel> students = new List<StudentsModel>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [enrollment_system].[dbo].[students]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    String currentGender = "Male";
                    if (dr["Gender"].ToString() == "2") currentGender = "Female";
                    StudentsModel student = new StudentsModel
                    {
                        id = (int)dr["id"],
                        FirstName = dr["FirstName"].ToString(),
                        MiddleName = dr["MiddleName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        Age = (int)dr["Age"],
                        Gender = currentGender,
                        Address = dr["Address"].ToString(),
                        Email = dr["Email"].ToString(),
                        AccountId = dr["AccountId"].ToString(),
                        ProfileFileName = dr["ProfileFileName"].ToString(),
                    };
                    students.Add(student);
                }
            }
            con.Close();
            ViewBag.Students = JsonConvert.SerializeObject(students);
            return View();
        }

        public ActionResult Edit(int id, StudentsModel model)
        {
            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [enrollment_system].[dbo].[students] WHERE id = '{id}'; ";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    model.id = (int)dr["id"];
                    model.FirstName = dr["FirstName"].ToString();
                    model.MiddleName = dr["MiddleName"].ToString();
                    model.LastName = dr["LastName"].ToString();
                    model.Age = (int)dr["Age"];
                    model.Address = dr["Address"].ToString();
                    model.Gender = dr["Address"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.AccountId = dr["AccountId"].ToString();
                    model.ProfileFileName = dr["ProfileFileName"].ToString();
                }
            }

            con.Close();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(StudentsModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE [enrollment_system].[dbo].[students] SET FirstName = '{model.FirstName.ToString()}', MiddleName = '{model.MiddleName.ToString()}', LastName = '{model.LastName.ToString()}', Gender = '{model.Gender.ToString()}', Address = '{model.Address.ToString()}', ContactNumber = '{model.ContactNumber.ToString()}', CourseId = '{model.CourseId.ToString()}', StatusId = '{model.StatusId.ToString()}',    WHERE id = '{model.id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    ViewBag.SaveResult = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There was a problem updating course. Please try again.");
                }
                con.Close();

            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
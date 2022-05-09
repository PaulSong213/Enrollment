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
    public class CoursesController : Controller
    {

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        [HttpGet]
        public ActionResult Index()
        {

            List <CoursesModel> courses = new List<CoursesModel>();

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
                    course.Name = dr["Name"].ToString();
                    course.Description = dr["Description"].ToString();
                    courses.Add(course);
                }
            }
            con.Close();
            ViewBag.Courses = JsonConvert.SerializeObject(courses);
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id, CoursesModel model)
        {
            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[courses] WHERE id = '{id}'";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    model.Name = dr["Name"].ToString();
                    model.Description = dr["Description"].ToString();
                    model.Id = (int)dr["id"];
                }
            }
            con.Close();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CoursesModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE [dbo].[courses] SET name = '{model.Name.ToString()}', description = '{model.Description.ToString()}'  WHERE id = '{model.Id}'";
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
            return View();
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"DELETE FROM [dbo].[courses] WHERE id = '{id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Course with id:" +  id +   "deleted successfully.";
                }
                else
                {
                    TempData["ErrorResult"] = "There was a problem deleting course. Please try again.";
                }
                con.Close();

            }
            catch (Exception e)
            {
                TempData["ErrorResult"] = e.Message;
            }
            return RedirectToAction("Index", "Courses");
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(CoursesModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"INSERT INTO [dbo].[courses] (name,description) VALUES ('{model.Name}' , '{model.Description}' ) ";
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
            return RedirectToAction("Index", "Courses");
        }
    
    }
}

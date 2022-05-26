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
                    course.Acronym = dr["Acronym"].ToString();
                    course.Slots = (int)dr["Slots"];
                    courses.Add(course);
                }
            }
            con.Close();
            ViewBag.Courses = JsonConvert.SerializeObject(courses);
            return View();
        }


        [HttpPost]
        public ActionResult Edit(CoursesModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE [dbo].[courses] SET name = '{model.Name.ToString()}', acronym = '{model.Acronym.ToString()}', slots = {(int)model.Slots}  WHERE id = '{model.Id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Course with id:" + model.Id + " edited successfully.";

                }
                else
                {
                    TempData["ErrorResult"] = "Course not edited. Please try again.";
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
                    TempData["MessageResult"] = "Course with id:" +  id +   " deleted successfully.";
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
                com.CommandText = $"INSERT INTO [dbo].[courses] (name,acronym,slots) VALUES ('{model.Name}' , '{model.Acronym}','{model.Slots}' ) ";
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

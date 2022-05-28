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
    public class SchedulesController : Controller
    {

        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        [HttpGet]
        public ActionResult Index()
        {

            List<SchedulesModel> schedules = new List<SchedulesModel>();
            List<SchedulesPreviewModel> schedulePreviews = new List<SchedulesPreviewModel>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[schedules] INNER JOIN (select id as courseId, acronym as courseAcronym, name as courseName from courses) [courses] ON [schedules].[courseId] = [courses].[courseId]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    //save scheadule preview
                    SchedulesPreviewModel schedulePreview = new SchedulesPreviewModel();
                    schedulePreview.Id = (int)dr["id"];
                    schedulePreview.year = (int)dr["year"];
                    schedulePreview.section = (int)dr["section"];
                    schedulePreview.course = dr["courseAcronym"].ToString();
                    schedulePreview.link = dr["link"].ToString();
                    schedulePreviews.Add(schedulePreview);

                    //save whole sched
                    SchedulesModel schedule = new SchedulesModel();
                    schedule.Id = (int)dr["id"];
                    schedule.year = (int)dr["year"];
                    schedule.section = (int)dr["section"];
                    schedule.courseId = (int)dr["courseId"];
                    schedule.link = dr["link"].ToString();

                    CoursesModel course = new CoursesModel();
                    course.Id = (int)dr["courseId"];
                    course.Name = dr["courseName"].ToString();
                    course.Acronym = dr["courseAcronym"].ToString();

                    schedule.CoursesModel = course;
                    schedules.Add(schedule);

                }
            }
            con.Close();

            //save courses
            List<CoursesModel> courses = new List<CoursesModel>();
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
                    course.Name = dr["name"].ToString();
                    course.Acronym = dr["acronym"].ToString();
                    courses.Add(course);

                }
            }
            con.Close();


            ViewBag.SchedulesPreview = JsonConvert.SerializeObject(schedulePreviews);
            ViewBag.Schedules = JsonConvert.SerializeObject(schedules);
            ViewBag.Courses = JsonConvert.SerializeObject(courses);
            return View();
        }


        [HttpPost]
        public ActionResult Edit(SchedulesModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE [dbo].[schedules] SET year = {model.year}, section = {model.section}, courseId = {model.courseId}, link= '{model.link}'  WHERE id = '{model.Id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Schedule with id:" + model.Id + " edited successfully.";

                }
                else
                {
                    TempData["ErrorResult"] = "Schedule not edited. Please try again.";
                }
                con.Close();

            }
            catch (Exception e)
            {
                TempData["ErrorResult"] = e.Message;

            }
            return RedirectToAction("Index", "Schedules");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"DELETE FROM [dbo].[schedules] WHERE id = '{id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Schedule with id:" +  id +   " deleted successfully.";
                }
                else
                {
                    TempData["ErrorResult"] = "There was a problem deleting schedule. Please try again.";
                }
                con.Close();

            }
            catch (Exception e)
            {
                TempData["ErrorResult"] = e.Message;
            }
            return RedirectToAction("Index", "Schedules");
        }

        [HttpPost]
        public ActionResult Add(SchedulesModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"INSERT INTO [dbo].[schedules] (year,section,courseId, link) VALUES ( {model.year} , {model.section} , {model.courseId} , '{model.link}' );";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Schedule added successfully.";
                }
                else
                {
                    TempData["ErrorResult"] = "There was a problem adding schedule. Please try again.";
                    return View();
                }
                con.Close();

            }
            catch (Exception e)
            {
                TempData["ErrorResult"] = e.Message;
            }
            return RedirectToAction("Index", "Schedules");
        }
    
    }
}

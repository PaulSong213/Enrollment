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
        [Authorize]
        public ActionResult Portal()
        {
            int year = 1;
            int section = 1;
            var studentId = 1060;
            if (this.Session["userId"] != null)
            {
                studentId = (int)this.Session["userId"];
            }
            var status = "empty";
            var enrollmentId = 1;
            StudentsModel student = new StudentsModel();
            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[enrollments] WHERE [enrollments].[studentId] = {studentId};";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    status = dr["status"].ToString();
                    enrollmentId = (int)dr["id"];
                    year = dr["year"] != DBNull.Value ? (int)dr["year"] : 1;
                    section = dr["section"] != DBNull.Value ? (int)dr["section"] : 1;
                }
            }
            con.Close();

            var scheduleLink = "https://docs.google.com/document/d/13b08LolVaE7YlGe0U7pu2bJyJKDgIQs71LdqoO6s_r4/edit";
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT link FROM [dbo].[schedules] WHERE year = {year} AND section = { section }";
            dr = com.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    scheduleLink = dr["link"].ToString();
                }
            }

            con.Close();

            ViewBag.Status = status;
            ViewBag.Student = student;
            ViewBag.scheduleLink = scheduleLink;
            ViewBag.EnrollmentId = enrollmentId;
            return View();
        }


        // GET: Student
        [HttpGet]
        public ActionResult Index()
        {

            List<StudentsModel> students = new List<StudentsModel>();
            List<StudentsPreviewModel> studentsPreview = new List<StudentsPreviewModel>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[students] INNER JOIN (select id, name as courseName, acronym as courseAcronym from courses) [courses] ON [students].[courseId] = [courses].[id]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    String currentGender = "Male";
                    if (dr["Gender"].ToString() == "2") currentGender = "Female";

                    //previews of students
                    StudentsPreviewModel studentPreview = new StudentsPreviewModel
                    {
                        id = (int)dr["id"],
                        FirstName = $"{dr["FirstName"].ToString()} {dr["LastName"].ToString()}",
                        Age = 21,
                        Gender = currentGender,
                        Email = dr["Email"].ToString(),
                        Course = dr["CourseAcronym"].ToString(),
                    };
                    studentsPreview.Add(studentPreview);
                    //full data of student
                    StudentsModel student = new StudentsModel
                    {
                        id = (int)dr["id"],
                        FirstName = dr["FirstName"].ToString(),
                        MiddleName = dr["MiddleName"] != DBNull.Value ? dr["MiddleName"].ToString() : "",
                        LastName = dr["LastName"].ToString(),
                        Age = 21,
                        Gender = currentGender,
                        Address = dr["Address"].ToString(),
                        Email = dr["Email"].ToString(),
                        AccountId = dr["AccountId"].ToString(),
                        StatusId = dr["StatusId"] != DBNull.Value ? (int)dr["StatusId"] : 1,
                        CourseId = dr["CourseId"] != DBNull.Value ? (int)dr["CourseId"] : 1,
                        ProfileFileName = dr["ProfileFileName"].ToString(),
                        ContactNumber = dr["ContactNumber"].ToString(),
                    };
                    students.Add(student);
                }
            }
            else
            {
                con.Close();
                dr.Close();
                con.Open();
                com.CommandText = $"SELECT * FROM [dbo].[students]";
                dr = com.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        String currentGender = "Male";
                        if (dr["Gender"].ToString() == "2") currentGender = "Female";

                        //previews of students
                        StudentsPreviewModel studentPreview = new StudentsPreviewModel
                        {
                            id = (int)dr["id"],
                            FirstName = $"{dr["FirstName"].ToString()} {dr["LastName"].ToString()}",
                            Age = 21,
                            Gender = currentGender,
                            Email = dr["Email"].ToString(),
                            Course = "None",
                        };
                        studentsPreview.Add(studentPreview);
                        //full data of student
                        StudentsModel student = new StudentsModel
                        {
                            id = (int)dr["id"],
                            FirstName = dr["FirstName"].ToString(),
                            MiddleName = dr["MiddleName"] != DBNull.Value ? dr["MiddleName"].ToString() : "",
                            LastName = dr["LastName"].ToString(),
                            Age = 21,
                            Gender = currentGender,
                            Address = dr["Address"].ToString(),
                            Email = dr["Email"].ToString(),
                            AccountId = dr["AccountId"].ToString(),
                            StatusId = 1,
                            CourseId = 1,
                            ProfileFileName = dr["ProfileFileName"].ToString(),
                            ContactNumber = dr["ContactNumber"].ToString(),
                        };
                        students.Add(student);
                    }
                }

            }

            con.Close();
            ViewBag.Students = JsonConvert.SerializeObject(students);
            ViewBag.studentsPreview = JsonConvert.SerializeObject(studentsPreview);

            return View();
        }

        public ActionResult Edit(int id, StudentsModel model)
        {
            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[students] WHERE id = '{id}'; ";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    model.id = (int)dr["id"];
                    model.FirstName = dr["FirstName"].ToString();
                    model.MiddleName = dr["MiddleName"] != DBNull.Value ? dr["MiddleName"].ToString() : "";
                    model.LastName = dr["LastName"].ToString();
                    model.Age = 21;
                    model.Address = dr["Address"].ToString();
                    model.Gender = dr["Address"].ToString();
                    model.Email = dr["Email"].ToString();
                    model.AccountId = dr["AccountId"].ToString();
                    model.ProfileFileName = dr["ProfileFileName"].ToString();
                    model.ContactNumber = dr["ContactNumber"].ToString();
                }
            }
            dr.Close();
            //Get courses
            List<CoursesModel> courses = new List<CoursesModel>();
            com.CommandText = $"SELECT * FROM [dbo].[courses]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    CoursesModel course = new CoursesModel
                    {
                        Id = (int)dr["Id"],
                        Name = dr["Name"].ToString(),
                        Acronym = dr["Acronym"].ToString(),
                        Slots = (int)dr["Acronym"]
                    };
                    courses.Add(course);
                }
            }
            ViewBag.Courses = courses;
            dr.Close();

            //Get courses
            List<StatusModel> status = new List<StatusModel>();
            com.CommandText = $"SELECT * FROM [dbo].[status]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    StatusModel newStatus = new StatusModel
                    {
                        Id = (int)dr["Id"],
                        Name = dr["Name"].ToString(),
                    };
                    status.Add(newStatus);
                }
            }
            ViewBag.Status = status;
            dr.Close();


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
                com.CommandText = $"UPDATE [dbo].[students] SET FirstName = '{model.FirstName}', MiddleName = '{model.MiddleName}' , LastName = '{model.LastName}', Address='{model.Address}', CourseId = {model.CourseId}, StatusId= {model.StatusId}, ContactNumber= '{model.ContactNumber}', Age={model.Age}, Gender= '{model.Gender}'  WHERE id = '{model.id}' ";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    ViewBag.SaveResult = true;
                    TempData["MessageResult"] = "Student with id: " + model.id + " edited successfully";
                }
                con.Close();

            }
            catch (Exception)
            {
                TempData["ErrorResult"] = "There was a problem editing the student. Please try again";
                //TempData["ErrorResult"] = model.CourseId;
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE [dbo].[students] SET isActive = 0  WHERE id = '{id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Student with id: "+ id +" marked as inactive successfully.";
                }
                else
                {
                    TempData["ErrorResult"] = "There was a problem making the student inactive. Please try again.";
                }
                con.Close();

            }
            catch (Exception e)
            {
                TempData["ErrorResult"] = e.Message;
            }
            return RedirectToAction("Index");
        }
    }
}

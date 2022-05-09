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
    public class StatusController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        public ActionResult Index()
        {
            List<StatusModel> status = new List<StatusModel>();

            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[status]";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    StatusModel newStatus = new StatusModel();
                    newStatus.Id = (int)dr["id"];
                    newStatus.Name = dr["Name"].ToString();
                    status.Add(newStatus);
                }
            }
            con.Close();
            ViewBag.Status = JsonConvert.SerializeObject(status);
            return View();
        }


        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(StatusModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"INSERT INTO [dbo].[status] (name) VALUES ('{model.Name}' ) ";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Status added successfully.";
                }
                else
                {
                    TempData["ErrorResult"] = "There was a problem adding status. Please try again.";
                    return View();
                }
                con.Close();

            }
            catch (Exception e)
            {
                TempData["ErrorResult"] = e.Message;
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id, StatusModel model)
        {
            con.ConnectionString = new AccountController().getConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = $"SELECT * FROM [dbo].[status] WHERE id = '{id}'";
            dr = com.ExecuteReader();
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    model.Name = dr["Name"].ToString();
                    model.Id = (int)dr["id"];
                }
            }
            con.Close();
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(StatusModel model)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"UPDATE [dbo].[status] SET name = '{model.Name.ToString()}' WHERE id = '{model.Id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    ViewBag.SaveResult = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "There was a problem updating status. Please try again.");
                }
                con.Close();

            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }
            return View();
        }

        public ActionResult Delete(int id)
        {
            try
            {
                con.ConnectionString = new AccountController().getConnectionString();
                con.Open();
                com.Connection = con;
                com.CommandText = $"DELETE FROM [dbo].[status] WHERE id = '{id}'";
                Boolean isUpdated = com.ExecuteNonQuery() > 0;
                if (isUpdated)
                {
                    TempData["MessageResult"] = "Status with id:" + id + " deleted successfully.";
                }
                else
                {
                    TempData["ErrorResult"] = "There was a problem deleting status. Please try again.";
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
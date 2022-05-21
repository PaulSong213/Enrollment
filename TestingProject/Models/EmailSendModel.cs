using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnrollmentSystem.Models
{
    public class EmailSendModel
    {
        [Key]
        public int Id { get; set; }

        [AllowHtml]
        [Display(Name = "EmailString")]
        public string EmailString { get; set; }

    }
}
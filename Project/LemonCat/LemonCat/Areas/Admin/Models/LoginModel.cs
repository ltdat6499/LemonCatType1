﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LemonCat.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please check your User Name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please check your Password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Helpers;

namespace WebApp.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }


    public class RopcLoginModel : LoginModel
    {
        public AuthResult Result { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class AuthLoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
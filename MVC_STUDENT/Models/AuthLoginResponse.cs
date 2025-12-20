using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class AuthLoginResponse
    {
        public int MaNguoiDung { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public int MaVaiTro { get; set; }
        public string Token { get; set; }
    }
}
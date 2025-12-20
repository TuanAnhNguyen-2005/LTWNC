using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class CreateNguoiDungRequest
    {
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string MatKhau { get; set; }
        public int MaVaiTro { get; set; }
        public bool IsLocked { get; set; } // nếu API có cột khóa
    }
}

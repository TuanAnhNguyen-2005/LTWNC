using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class TraLoiDto
    {
        public int MaCauHoi { get; set; }
        public string TraLoi { get; set; } // text hoặc "1,3" cho multiple, hoặc MaLuaChon
    }
}
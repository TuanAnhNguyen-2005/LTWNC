using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Teacher.Models
{
    public class CauHoiDetailDto
    {
        public int MaCauHoi { get; set; }
        public string NoiDung { get; set; }
        public string LoaiCauHoi { get; set; }
        public double Diem { get; set; }
        public List<LuaChonDetailDto> LuaChons { get; set; } = new List<LuaChonDetailDto>();
        public int SoHocSinhTraLoi { get; set; } // Số học sinh đã trả lời câu này
    }

    public class LuaChonDetailDto
    {
        public int MaLuaChon { get; set; }
        public string NoiDung { get; set; }
        public bool LaDapAnDung { get; set; }
    }
}
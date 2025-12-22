using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Teacher.Models
{
    public class QuizDto
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; }
        public string MoTa { get; set; } // Thêm
        public string TrangThai { get; set; }
        public int? ThoiGianLamBai { get; set; }
        public int? MaKhoaHoc { get; set; } // Sửa thành nullable
        public DateTime NgayTao { get; set; } // Thêm
        public List<CauHoiDto> CauHois { get; set; } = new List<CauHoiDto>(); // Thêm
    }

    public class CauHoiDto
    {
        public int MaCauHoi { get; set; }
        public string NoiDung { get; set; }
        public string LoaiCauHoi { get; set; }
        public double Diem { get; set; }
        public List<LuaChonDto> LuaChons { get; set; } = new List<LuaChonDto>();
    }

    public class LuaChonDto
    {
        public int MaLuaChon { get; set; }
        public string NoiDung { get; set; }
        public bool LaDapAnDung { get; set; }
    }
}
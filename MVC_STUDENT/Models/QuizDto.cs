using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class QuizDto
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; } = string.Empty;
        public string MoTa { get; set; }
        public int? ThoiGianLamBai { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public int SoCauHoi => CauHois?.Count ?? 0;
        public List<CauHoiDto> CauHois { get; set; } = new List<CauHoiDto>();
    }

    public class CauHoiDto
    {
        public int MaCauHoi { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public string LoaiCauHoi { get; set; } = string.Empty; // MultipleChoice, TrueFalse, ShortAnswer
        public float Diem { get; set; }
        public List<LuaChonDto> LuaChons { get; set; } = new List<LuaChonDto>();
    }

    public class LuaChonDto
    {
        public int MaLuaChon { get; set; }
        public string NoiDung { get; set; } = string.Empty;
    }
}
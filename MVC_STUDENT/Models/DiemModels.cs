using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class QuizDiemDto
    {
        public int MaKetQua { get; set; }
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; } = "";
        public double Diem { get; set; }
        public double TongDiem { get; set; }
        public DateTime NgayNop { get; set; }
        public TimeSpan ThoiGianLam { get; set; }

    }

    public class QuizDiemChiTietDto
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; } = "";
        public double Diem { get; set; }
        public double TongDiem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCau { get; set; }
        public TimeSpan ThoiGianLam { get; set; }
        public int MaKetQua { get; set; }
        public DateTime NgayNop { get; set; }
        public List<ChiTietCauHoiDto> ChiTietCauHoi { get; set; } = new List<ChiTietCauHoiDto>();
    }

    public class ChiTietCauHoiDto
    {
        public string NoiDung { get; set; } = "";
        public double Diem { get; set; }
        public string TraLoi { get; set; } = "";
        public string DapAn { get; set; } = "";
        public bool DungSai { get; set; }
    }
}
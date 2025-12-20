using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class QuizResultDto
    {
        public string TenQuiz { get; set; } = string.Empty;

        public double Diem { get; set; }          // đổi thành double
        public double TongDiem { get; set; }       // đổi thành double

        public int SoCauDung { get; set; }
        public int TongCau { get; set; }
        public TimeSpan ThoiGianLam { get; set; }

        public DateTime NgayNop { get; set; }     // THÊM DÒNG NÀY
    }
}
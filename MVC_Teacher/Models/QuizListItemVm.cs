using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Teacher.Models
{
    public class QuizListItemVm
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; }
        public string TrangThai { get; set; }
        public int? SoCauHoi { get; set; } // Có thể dùng để hiển thị số câu hỏi
        public int? ThoiGianLamBai { get; set; }
    }
}
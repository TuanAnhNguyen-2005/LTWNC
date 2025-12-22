using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Teacher.Models
{
    // QuizDeleteViewModel.cs
    public class QuizDeleteViewModel
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; }
        public string TenKhoaHoc { get; set; }
        public int SoCauHoi { get; set; }
        public int SoHocSinhLamBai { get; set; }
    }
}
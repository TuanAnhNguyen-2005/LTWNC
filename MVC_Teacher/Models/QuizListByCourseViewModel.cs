using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Teacher.Models
{
    public class QuizListByCourseViewModel
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public List<QuizListItemVm> Quizzes { get; set; } = new List<QuizListItemVm>();
    }
}
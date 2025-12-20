using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_STUDENT.Models
{
    public class QuizSubmitDto
    {
        public int MaQuiz { get; set; }
        public int MaHocSinh { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public List<TraLoiDto> TraLois { get; set; } = new List<TraLoiDto>();
    }
}
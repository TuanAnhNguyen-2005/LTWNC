using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Teacher.Models
{
    public class QuizDetailViewModel
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; }
        public string MoTa { get; set; }
        public string TrangThai { get; set; }
        public int? ThoiGianLamBai { get; set; }
        public DateTime NgayTao { get; set; }
        public string TenKhoaHoc { get; set; }
        public string TenGiaoVien { get; set; }
        public int SoCauHoi { get; set; }
        public double TongDiem { get; set; }
        public int SoHocSinhLamBai { get; set; }
        public double DiemTrungBinh { get; set; }
        public int PhanTramHoanThanh { get; set; }
        public int PhanTramDat { get; set; }

        // Thêm dòng này
        public int MaKhoaHoc { get; set; }  // Hoặc int? MaKhoaHoc nếu có thể null

        public List<CauHoiDetailDto> CauHois { get; set; }

        public QuizDetailViewModel()
        {
            CauHois = new List<CauHoiDetailDto>();
        }
    }
}
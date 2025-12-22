// Models/QuizCreateViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MVC_Teacher.Models
{
    public class QuizCreateViewModel
    {
        public int MaQuiz { get; set; } // Thêm dòng này

        [Required(ErrorMessage = "Vui lòng nhập tên quiz")]
        public string TenQuiz { get; set; }
        public string MoTa { get; set; }
        public int? MaKhoaHoc { get; set; }
        public int MaGiaoVien { get; set; }
        [Range(1, 300, ErrorMessage = "Thời gian từ 1 đến 300 phút")]
        public int? ThoiGianLamBai { get; set; }
        public List<CauHoiViewModel> CauHois { get; set; } = new List<CauHoiViewModel>();

        // Chuyển sang DTO để gửi API
        public object ToDto()
        {
            return new
            {
                maQuiz = MaQuiz, // Thêm nếu API cần
                tenQuiz = TenQuiz,
                moTa = MoTa,
                maKhoaHoc = MaKhoaHoc,
                maGiaoVien = MaGiaoVien,
                thoiGianLamBai = ThoiGianLamBai,
                cauHois = CauHois.Select(ch => new
                {
                    noiDung = ch.NoiDung,
                    loaiCauHoi = ch.LoaiCauHoi,
                    diem = ch.Diem,
                    luaChons = ch.LuaChons?.Select(lc => new
                    {
                        noiDung = lc.NoiDung,
                        laDapAnDung = lc.LaDapAnDung
                    })
                })
            };
        }
    }

    public class CauHoiViewModel
    {
        [Required]
        public string NoiDung { get; set; }

        public string LoaiCauHoi { get; set; } = "MultipleChoice"; // MultipleChoice, TrueFalse, ShortAnswer

        [Range(0.5, 50)]
        public float Diem { get; set; } = 1;

        public List<LuaChonViewModel> LuaChons { get; set; } = new List<LuaChonViewModel>();
    }

    public class LuaChonViewModel
    {
        public string NoiDung { get; set; }
        public bool LaDapAnDung { get; set; }
    }
}
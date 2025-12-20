namespace RestfullAPI_NTHL.Models
{
    public class Quiz
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; } = null!;
        public string? MoTa { get; set; }
        public int? MaKhoaHoc { get; set; }
        public int MaGiaoVien { get; set; }
        public int? ThoiGianLamBai { get; set; } // phút
        public string TrangThai { get; set; } = "Draft"; // Draft, Published, Closed
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public KhoaHoc? KhoaHoc { get; set; }
        public NguoiDung GiaoVien { get; set; } = null!;
        public ICollection<CauHoi> CauHois { get; set; } = new List<CauHoi>();
    }
}

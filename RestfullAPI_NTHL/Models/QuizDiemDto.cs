namespace RestfullAPI_NTHL.Models
{
    public class QuizDiemDto
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; } = string.Empty;
        public double Diem { get; set; }
        public double TongDiem { get; set; }
        public DateTime NgayNop { get; set; }
        public TimeSpan ThoiGianLam { get; set; }
        public int MaKetQua { get; set; }
    }

    public class QuizDiemChiTietDto
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; } = string.Empty;
        public double Diem { get; set; }
        public double TongDiem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCau { get; set; }
        public TimeSpan ThoiGianLam { get; set; }
        public int MaKetQua { get; set; }
        public DateTime NgayNop { get; set; }
        public List<ChiTietCauHoiDto> ChiTietCauHoi { get; set; } = new();
    }

    public class ChiTietCauHoiDto
    {
        public string NoiDung { get; set; } = string.Empty;
        public double Diem { get; set; }
        public string TraLoi { get; set; } = string.Empty;
        public string DapAn { get; set; } = string.Empty;
        public bool DungSai { get; set; }
    }
}

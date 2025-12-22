namespace RestfullAPI_NTHL.Models
{
    public class QuizResultDto
    {
        public string TenQuiz { get; set; } = string.Empty;
        public double Diem { get; set; }
        public double TongDiem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCau { get; set; }
        public TimeSpan ThoiGianLam { get; set; }
        public DateTime NgayNop { get; set; }
    }
}

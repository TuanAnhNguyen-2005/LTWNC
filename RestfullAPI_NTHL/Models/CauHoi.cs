namespace RestfullAPI_NTHL.Models
{
    public class CauHoi
    {
        public int MaCauHoi { get; set; }
        public int MaQuiz { get; set; }
        public string NoiDung { get; set; } = null!;
        public string LoaiCauHoi { get; set; } = null!; // MultipleChoice, TrueFalse, ShortAnswer
        public double Diem { get; set; } = 1;
        public Quiz Quiz { get; set; } = null!;
        public ICollection<LuaChon> LuaChons { get; set; } = new List<LuaChon>();
    }
}

namespace RestfullAPI_NTHL.Models
{
    public class LuaChon
    {
        public int MaLuaChon { get; set; }
        public int MaCauHoi { get; set; }
        public string NoiDung { get; set; } = null!;
        public bool LaDapAnDung { get; set; }

        public CauHoi CauHoi { get; set; } = null!;
    }
}

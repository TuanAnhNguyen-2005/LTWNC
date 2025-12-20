public class AuthLoginResponse
{
    public int MaNguoiDung { get; set; }
    public string HoTen { get; set; }
    public string Email { get; set; }
    public int MaVaiTro { get; set; }
    public string Token { get; set; } // nếu bạn làm JWT, còn không thì tạm để ""
}

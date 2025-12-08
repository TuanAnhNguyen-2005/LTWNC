namespace RestFullAPI.Models
{
    public class User
    {
        public int MaNguoiDung { get; set; }
        public string? HoTen { get; set; }
        public string Email { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public int? MaVaiTro { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; } = true;

        // Navigation property
        public Role? Role { get; set; }
    }
}


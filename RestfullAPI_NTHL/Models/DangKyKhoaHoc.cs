using System;

namespace RestfullAPI_NTHL.Models;

public partial class DangKyKhoaHoc
{
    public int MaDangKy { get; set; }

    public int MaKhoaHoc { get; set; }

    // Học viên đăng ký (FK -> NguoiDung)
    public int? MaNguoiDung { get; set; }

    public DateTime? NgayDangKy { get; set; }

    // Navigation
    public virtual KhoaHoc MaKhoaHocNavigation { get; set; } = null!;
    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}

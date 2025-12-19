using System;
using System.Collections.Generic;

namespace RestfullAPI_NTHL.Models;

public partial class KhoaHoc
{
    public int MaKhoaHoc { get; set; }

    public string TenKhoaHoc { get; set; } = null!;

    public string? Slug { get; set; }

    public string? MoTa { get; set; }

    // =========================
    // FK giáo viên tạo khóa học
    // =========================
    public int MaGiaoVien { get; set; }

    // =========================
    // Workflow duyệt
    // Draft | ChoDuyet | DaDuyet | TuChoi
    // =========================
    public string TrangThaiDuyet { get; set; } = "Draft";

    public DateTime NgayTao { get; set; }

    public DateTime? NgayGuiDuyet { get; set; }

    public DateTime? NgayDuyet { get; set; }

    public int? NguoiDuyetId { get; set; }

    public string? AnhBia { get; set; }  // Đường dẫn ảnh, ví dụ: /images/courses/abcd123.jpg

    public string? LyDoTuChoi { get; set; }

    // =========================
    // Active / Soft delete
    // =========================
    public bool IsActive { get; set; } = true;

    // =========================
    // Navigation
    // =========================
    public virtual NguoiDung MaGiaoVienNavigation { get; set; } = null!;
    public virtual NguoiDung? NguoiDuyetNavigation { get; set; }

    public virtual ICollection<DangKyKhoaHoc> DangKyKhoaHoc { get; set; }
        = new List<DangKyKhoaHoc>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestfullAPI_NTHL.Models;

[Index("Email", Name = "UQ__NguoiDun__A9D105346E6FF363", IsUnique = true)]
public partial class NguoiDung
{
    [Key]
    public int MaNguoiDung { get; set; }

    [StringLength(100)]
    public string? HoTen { get; set; }

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    [StringLength(255)]
    public string? AnhDaiDien { get; set; }

    [StringLength(15)]
    public string? SoDienThoai { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    public int? MaVaiTro { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LanDangNhapCuoi { get; set; }

    public bool? TrangThai { get; set; }

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<BinhLuan> BinhLuan { get; set; } = new List<BinhLuan>();

    [InverseProperty("MaNguoiDungNavigation")]
    public virtual ICollection<HocLieu> HocLieu { get; set; } = new List<HocLieu>();

    [ForeignKey("MaVaiTro")]
    [InverseProperty("NguoiDung")]
    public virtual VaiTro? MaVaiTroNavigation { get; set; }
}

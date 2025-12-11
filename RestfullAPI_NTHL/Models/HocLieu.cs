using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestfullAPI_NTHL.Models;

public partial class HocLieu
{
    [Key]
    public int MaHocLieu { get; set; }

    [StringLength(200)]
    public string TieuDe { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    [StringLength(255)]
    public string? DuongDanTep { get; set; }

    [StringLength(50)]
    public string? LoaiTep { get; set; }

    public long? KichThuocTep { get; set; }

    [StringLength(20)]
    public string? DoKho { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDang { get; set; }

    public int? MaChuDe { get; set; }

    public int? MaNguoiDung { get; set; }

    public int? MaMonHoc { get; set; }

    public int? MaLopHoc { get; set; }

    public bool? DaDuyet { get; set; }

    [StringLength(20)]
    public string? TrangThaiDuyet { get; set; }

    public int? NguoiDuyet { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDuyet { get; set; }

    [StringLength(500)]
    public string? LyDoTuChoi { get; set; }

    public int? LuotXem { get; set; }

    public int? SoLuotTai { get; set; }

    public double? DiemTrungBinh { get; set; }

    [InverseProperty("MaHocLieuNavigation")]
    public virtual ICollection<BinhLuan> BinhLuan { get; set; } = new List<BinhLuan>();

    [ForeignKey("MaChuDe")]
    [InverseProperty("HocLieu")]
    public virtual ChuDe? MaChuDeNavigation { get; set; }

    [ForeignKey("MaLopHoc")]
    [InverseProperty("HocLieu")]
    public virtual LopHoc? MaLopHocNavigation { get; set; }

    [ForeignKey("MaMonHoc")]
    [InverseProperty("HocLieu")]
    public virtual MonHoc? MaMonHocNavigation { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("HocLieu")]
    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}

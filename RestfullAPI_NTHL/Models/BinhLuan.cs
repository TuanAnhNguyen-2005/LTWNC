using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestfullAPI_NTHL.Models;

public partial class BinhLuan
{
    [Key]
    public int MaBinhLuan { get; set; }

    public int? MaHocLieu { get; set; }

    public int? MaNguoiDung { get; set; }

    [StringLength(500)]
    public string? NoiDung { get; set; }

    public int? DanhGia { get; set; }

    public int? MaBinhLuanCha { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDang { get; set; }

    public bool? TrangThai { get; set; }

    [InverseProperty("MaBinhLuanChaNavigation")]
    public virtual ICollection<BinhLuan> InverseMaBinhLuanChaNavigation { get; set; } = new List<BinhLuan>();

    [ForeignKey("MaBinhLuanCha")]
    [InverseProperty("InverseMaBinhLuanChaNavigation")]
    public virtual BinhLuan? MaBinhLuanChaNavigation { get; set; }

    [ForeignKey("MaHocLieu")]
    [InverseProperty("BinhLuan")]
    public virtual HocLieu? MaHocLieuNavigation { get; set; }

    [ForeignKey("MaNguoiDung")]
    [InverseProperty("BinhLuan")]
    public virtual NguoiDung? MaNguoiDungNavigation { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestfullAPI_NTHL.Models;

public partial class ChuDe
{
    [Key]
    public int MaChuDe { get; set; }

    [StringLength(100)]
    public string TenChuDe { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [InverseProperty("MaChuDeNavigation")]
    public virtual ICollection<HocLieu> HocLieu { get; set; } = new List<HocLieu>();
}

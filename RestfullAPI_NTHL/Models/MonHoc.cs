using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestfullAPI_NTHL.Models;

public partial class MonHoc
{
    [Key]
    public int MaMonHoc { get; set; }

    [StringLength(100)]
    public string TenMonHoc { get; set; } = null!;

    [InverseProperty("MaMonHocNavigation")]
    public virtual ICollection<HocLieu> HocLieu { get; set; } = new List<HocLieu>();
}

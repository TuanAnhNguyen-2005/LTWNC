using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RestfullAPI_NTHL.Models;

public partial class LopHoc
{
    [Key]
    public int MaLopHoc { get; set; }

    [StringLength(50)]
    public string TenLopHoc { get; set; } = null!;

    [InverseProperty("MaLopHocNavigation")]
    public virtual ICollection<HocLieu> HocLieu { get; set; } = new List<HocLieu>();
}

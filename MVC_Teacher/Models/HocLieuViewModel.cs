using System;

namespace MVC_Teacher.Models
{
    public class HocLieuViewModel
    {
        public int MaHocLieu { get; set; }
        public string TieuDe { get; set; } = string.Empty;
        public string MoTa { get; set; }
        public string DuongDanTep { get; set; } = string.Empty;
        public string LoaiTep { get; set; } = string.Empty;
        public long? KichThuocTep { get; set; }
        public DateTime NgayDang { get; set; }

        public string KichThuocHienThi => KichThuocTep.HasValue
            ? KichThuocTep.Value >= 1048576
                ? $"{(KichThuocTep.Value / 1048576.0):F1} MB"
                : $"{(KichThuocTep.Value / 1024.0):F1} KB"
            : "N/A";
    }
}
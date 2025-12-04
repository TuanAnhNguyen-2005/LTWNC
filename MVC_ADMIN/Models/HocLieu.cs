using System;

namespace MVC_ADMIN.Models
{
    public class HocLieu
    {
        public int MaHocLieu { get; set; }
        public string TieuDe { get; set; }
        public string MoTa { get; set; }
        public string DuongDanTep { get; set; }
        public string LoaiTep { get; set; }
        public long? KichThuocTep { get; set; }
        public string DoKho { get; set; }
        public DateTime? NgayDang { get; set; }
        public int? MaChuDe { get; set; }
        public int? MaNguoiDung { get; set; }
        public int? MaMonHoc { get; set; }
        public int? MaLopHoc { get; set; }
        public bool DaDuyet { get; set; }
        public string TrangThaiDuyet { get; set; }
        public int? NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string LyDoTuChoi { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
        public virtual MonHoc MonHoc { get; set; }
        public virtual ChuDe ChuDe { get; set; }
        public virtual LopHoc LopHoc { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RestfullAPI_NTHL.Models
{
    [Table("KetQuaQuiz")]  // Quan trọng: chỉ định đúng tên bảng trong DB
    public class KetQuaQuiz
    {
        [Key]
        public int MaKetQua { get; set; }

        public int MaQuiz { get; set; }

        public int MaHocSinh { get; set; }

        public double? Diem { get; set; }

        public double? TongDiem { get; set; }

        public int SoCauDung { get; set; } = 0;

        public int TongCau { get; set; }

        public DateTime? ThoiGianBatDau { get; set; }

        public DateTime? ThoiGianKetThuc { get; set; }

        public DateTime? NgayNop { get; set; }

        // Navigation properties
        [ForeignKey("MaQuiz")]
        public virtual Quiz Quiz { get; set; }

        [ForeignKey("MaHocSinh")]
        public virtual NguoiDung HocSinh { get; set; }
    }
}

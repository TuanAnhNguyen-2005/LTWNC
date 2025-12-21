using System.ComponentModel.DataAnnotations.Schema;

namespace RestfullAPI_NTHL.Models
{
    public partial class HocLieu
    {
        /// <summary>
        /// Liên kết với khóa học (tài liệu thuộc khóa học nào)
        /// </summary>
        public int? MaKhoaHoc { get; set; }

        // XÓA 2 DÒNG NÀY ĐI:
        // [ForeignKey("MaKhoaHoc")]
        // public virtual KhoaHoc? MaKhoaHocNavigation { get; set; }
    }
}
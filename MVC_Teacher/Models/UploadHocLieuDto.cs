using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MVC_Teacher.Models
{
    public class UploadHocLieuDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề tài liệu")]
        [Display(Name = "Tiêu đề")]
        public string TieuDe { get; set; } = string.Empty;

        [Display(Name = "Mô tả (tùy chọn)")]
        public string MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn file tài liệu")]
        [Display(Name = "File tài liệu")]
        public HttpPostedFileBase FileHocLieu { get; set; } = null;

        public int MaKhoaHoc { get; set; }
    }
}
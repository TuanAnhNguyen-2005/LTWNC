using System.ComponentModel.DataAnnotations;

namespace RestfullAPI_NTHL.Models
{
    public class CreateCourseDto
    {
        [Required]
        public string TenKhoaHoc { get; set; }

        // Bỏ [Required], cho phép null hoặc empty
        public string? Slug { get; set; }

        public string? MoTa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaGiaoVien không hợp lệ")]
        public int MaGiaoVien { get; set; }
        public string? AnhBia { get; set; }
    }
}
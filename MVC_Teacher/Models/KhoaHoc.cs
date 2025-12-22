using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_ADMIN.Models
{
    public class KhoaHoc
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên khóa học không được vượt quá 200 ký tự")]
        [Display(Name = "Tên khóa học")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [Display(Name = "Danh mục")]
        public virtual Category Category { get; set; }

        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày bắt đầu")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }
    }
}
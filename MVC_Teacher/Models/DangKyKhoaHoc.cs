using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_ADMIN.Models
{
    public class DangKyKhoaHoc
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Khóa học là bắt buộc")]
        [Display(Name = "Khóa học")]
        public int KhoaHocId { get; set; }

        [ForeignKey("KhoaHocId")]
        [Display(Name = "Khóa học")]
        public virtual KhoaHoc KhoaHoc { get; set; }

        [Display(Name = "Người dùng")]
        public int? UserId { get; set; }

        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        [Display(Name = "Ngày đăng ký")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày duyệt")]
        public DateTime? ApprovalDate { get; set; }

        [Display(Name = "Ghi chú")]
        public string Notes { get; set; }

        [Display(Name = "Đã thanh toán")]
        public bool IsPaid { get; set; } = false;
    }
}
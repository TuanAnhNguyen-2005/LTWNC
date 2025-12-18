using System;
using System.Collections.Generic;
using System.Linq;
// Models/ProfileEditViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace MVC_ADMIN.Models
{
    public class ProfileEditViewModel
    {
        // Không bao giờ show password ra view !
        public string Password { get; set; }
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Ngày sinh")]
        // Chuỗi vì input type="date" sẽ gửi dưới dạng "yyyy-MM-dd"
        public string DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Web.Mvc;
using MVC_ADMIN.Services;

namespace MVC_ADMIN.Controllers
{
    /// <summary>
    /// Controller xử lý đăng ký
    /// </summary>
    public class SignUpController : BaseController
    {
        private readonly UserDataService _userDataService;

        public SignUpController()
        {
            _userDataService = new UserDataService();
        }

        public ActionResult Index()
        {
            // Nếu đã đăng nhập thì redirect về trang chủ
            if (IsAuthenticated())
                return RedirectToHomeByRole();

            return View(new SignUpViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(SignUpViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                // Kiểm tra mật khẩu xác nhận
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp!");
                    return View(model);
                }

                // Lưu vào database
                bool success = _userDataService.RegisterUser(
                    model.FullName,
                    model.Email,
                    model.Password,
                    model.Role,
                    model.PhoneNumber,
                    model.Gender,
                    model.Address,
                    model.DateOfBirth
                );

                if (success)
                {
                    SetSuccessMessage("Đăng ký thành công! Vui lòng đăng nhập.");
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    ViewBag.Error = "Đăng ký thất bại! Email có thể đã được sử dụng.";
                    return View(model);
                }
            }
            catch (SqlException sqlEx)
            {
                // Lỗi SQL cụ thể
                string errorMsg = "Đã xảy ra lỗi khi đăng ký: ";
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Duplicate key
                {
                    errorMsg = "Email này đã được sử dụng. Vui lòng chọn email khác.";
                }
                else
                {
                    errorMsg += sqlEx.Message;
                }
                ViewBag.Error = errorMsg;
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau.");
                ViewBag.Error = "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau.";
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                return View(model);
            }
        }
    }

    /// <summary>
    /// ViewModel cho đăng ký
    /// </summary>
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Student";
    }
}
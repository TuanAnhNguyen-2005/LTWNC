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
    [AllowAnonymous]
    public class SignUpController : BaseController
    {
        private readonly UserDataService _userDataService;

        public SignUpController()
        {
            _userDataService = new UserDataService();
        }

        [AllowAnonymous]
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
                // show the SQL error for debugging (temporary)
                ViewBag.Error = "Đã xảy ra lỗi khi đăng ký: " + sqlEx.Message;
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Message}");
                return View(model);
            }
            catch (InvalidOperationException invEx)
            {
                // Missing connection string or configuration error — give actionable message
                ViewBag.Error = invEx.Message;
                System.Diagnostics.Debug.WriteLine($"Config Error: {invEx.Message}");
                return View(model);
            }
            catch (Exception ex)
            {
                // TEMPORARY: include detailed error to help diagnose environment problems
                ViewBag.Error = "Đã xảy ra lỗi khi đăng ký. Details: " + ex.Message + (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "");
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                return View(model);
            }
        }

        // Temporary diagnostic endpoint — remove in production
        [AllowAnonymous]
        public ActionResult TestDb()
        {
            var svc = new UserDataService();
            if (svc.TestConnection(out var error))
                return Content("DB connection OK");
            return Content("DB connection FAILED: " + error);
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
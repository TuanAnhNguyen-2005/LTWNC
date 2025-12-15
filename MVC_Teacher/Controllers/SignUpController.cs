using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVC_Teacher.Services;

namespace MVC_Teacher.Controllers
{
    public class SignUpController : Controller
    {
        private readonly UserDataService _userDataService;

        public SignUpController()
        {
            _userDataService = new UserDataService();
        }

        // GET: SignUp
        public ActionResult Index()
        {
            // If already logged in, redirect to Home
            if (Session != null && Session["UserId"] != null)
                return RedirectToAction("Index", "Home");

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

                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp!");
                    return View(model);
                }

                bool success = _userDataService.RegisterUser(
                    model.FullName,
                    model.Email,
                    model.Password,
                    model.Role ?? "Teacher",
                    model.PhoneNumber,
                    model.Gender,
                    model.Address,
                    model.DateOfBirth
                );

                if (success)
                {
                    TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Index", "Login");
                }

                ModelState.AddModelError("", "Đăng ký thất bại. Email có thể đã tồn tại.");
                return View(model);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Signup error: {ex.Message}");
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại sau.");
                return View(model);
            }
        }

        public class SignUpViewModel
        {
            [Required]
            public string FullName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [MinLength(6)]
            public string Password { get; set; }

            [Required]
            [MinLength(6)]
            public string ConfirmPassword { get; set; }

            public string Role { get; set; }
            public string PhoneNumber { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }
    }
}
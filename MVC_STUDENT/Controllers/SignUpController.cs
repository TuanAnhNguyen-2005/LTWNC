using System;
using System.Web.Mvc;
using MVC_STUDENT.Services; // Namespace mới

namespace MVC_STUDENT.Controllers
{
    public class SignUpController : Controller
    {
        private readonly UserDataService _userService = new UserDataService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string fullName, string email, string password, string confirmPassword, string phone, string role)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin bắt buộc.";
                return RedirectToAction("Index");
            }

            if (password != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp!";
                return RedirectToAction("Index");
            }

            if (password.Length < 6)
            {
                TempData["Error"] = "Mật khẩu phải ít nhất 6 ký tự.";
                return RedirectToAction("Index");
            }

            try
            {
                bool success = _userService.RegisterUser(fullName, email, password, role ?? "Student", phone);

                if (success)
                {
                    TempData["Success"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    TempData["Error"] = "Email đã được sử dụng hoặc lỗi hệ thống.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối database: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
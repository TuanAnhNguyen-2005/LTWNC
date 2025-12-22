using System;
using System.Web.Mvc;
using MVC_STUDENT.Services;
using MVC_STUDENT.Models; // ← THÊM DÒNG NÀY CHO LoginResult

namespace MVC_STUDENT.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserDataService _userService;

        public LoginController()
        {
            _userService = new UserDataService();
        }

        // Helper method: Kiểm tra user đã đăng nhập và là Student chưa
        private bool IsStudentLoggedIn()
        {
            return Session["UserId"] != null &&
                   Session["Role"] != null &&
                   Session["Role"].ToString() == "Student";
        }

        // GET: /Login
        public ActionResult Index()
        {
            if (IsStudentLoggedIn())
            {
                return Redirect("/"); // Về trang chủ chính[](https://localhost:44326/)
            }

            return View();
        }

        // POST: /Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string password, bool? rememberMe)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return View();
            }

            try
            {
                var result = _userService.LoginUser(email.Trim(), password);

                if (result.Success)
                {
                    // === RÀNG BUỘC: CHỈ SINH VIÊN (MaVaiTro = 3, Role = "Student") ===
                    if (result.Role == "Student")
                    {
                        // Lưu Session
                        Session["UserId"] = result.UserId;
                        Session["FullName"] = result.FullName;
                        Session["Email"] = result.Email;
                        Session["Role"] = "Student";

                        // Hỗ trợ "Ghi nhớ đăng nhập" bằng Cookie (tùy chọn)
                        if (rememberMe == true)
                        {
                            Response.Cookies["RememberMe"].Value = result.UserId.ToString();
                            Response.Cookies["RememberMe"].Expires = DateTime.Now.AddDays(30);
                        }

                        TempData["Success"] = $"Chào mừng {result.FullName}!";
                        return Redirect("/"); // Về thẳng trang chủ chính
                    }
                    else
                    {
                        TempData["Error"] = "❌ Trang này chỉ dành cho Sinh viên. " +
                            "Tài khoản của bạn là " + result.Role +
                            ". Vui lòng dùng <a href='/'>trang chính</a>.";
                        return View();
                    }
                }
                else
                {
                    TempData["Error"] = "Email hoặc mật khẩu không chính xác.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống. Vui lòng thử lại.";
                System.Diagnostics.Debug.WriteLine("Login error: " + ex.Message);
                return View();
            }
        }

        // GET: /Login/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Response.Cookies["RememberMe"].Expires = DateTime.Now.AddDays(-1);

            TempData["Success"] = "Đăng xuất thành công!";
            return RedirectToAction("Index");
        }
    }
}
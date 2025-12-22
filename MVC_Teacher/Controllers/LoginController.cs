using System;
using System.Web.Mvc;
using MVC_Teacher.Services;

namespace MVC_Teacher.Controllers
{
    /// <summary>
    /// Controller xử lý đăng nhập/đăng xuất cho MVC_Teacher
    /// </summary>
    public class LoginController : Controller
    {
        private readonly UserDataService _userDataService;

        public LoginController()
        {
            _userDataService = new UserDataService();
        }

        // GET: /Login
        [HttpGet]
        public ActionResult Index()
        {
            // Nếu đã đăng nhập thì redirect về trang chủ
            if (Session != null && Session["UserId"] != null)
                return RedirectToAction("Index", "Home");

            // Xử lý message từ logout
            if (TempData["SuccessMessage"] != null)
                ViewBag.Success = TempData["SuccessMessage"];

            if (TempData["ErrorMessage"] != null)
                ViewBag.Error = TempData["ErrorMessage"];

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string password)
        {
            try
            {
                // Debug: Kiểm tra token
                DebugTokenInfo();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu!";
                    return View();
                }

                // Gọi service đăng nhập
                var result = _userDataService.LoginUser(email, password);

                if (result == null)
                {
                    ViewBag.Error = "Lỗi kết nối dịch vụ. Vui lòng thử lại sau!";
                    return View();
                }

                // Kiểm tra kết quả đăng nhập
                // (Sử dụng reflection nếu không biết cấu trúc cụ thể của result)
                bool loginSuccess = false;

                if (result.GetType().GetProperty("Success") != null)
                {
                    var successProp = result.GetType().GetProperty("Success");
                    if (successProp != null)
                    {
                        var successValue = successProp.GetValue(result);
                        if (successValue is bool)
                            loginSuccess = (bool)successValue;
                        else if (successValue is string)
                            loginSuccess = ((string)successValue).ToLower() == "true";
                    }
                }

                if (!loginSuccess)
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                    return View();
                }

                // Lấy thông tin user
                int? userId = null;
                string fullName = null;
                string userEmail = null;
                string role = null;

                // Sử dụng reflection để lấy các properties
                var userIdProp = result.GetType().GetProperty("UserId");
                if (userIdProp != null && userIdProp.GetValue(result) != null)
                    userId = Convert.ToInt32(userIdProp.GetValue(result));

                var fullNameProp = result.GetType().GetProperty("FullName");
                if (fullNameProp != null && fullNameProp.GetValue(result) != null)
                    fullName = fullNameProp.GetValue(result) as string;

                var emailProp = result.GetType().GetProperty("Email");
                if (emailProp != null && emailProp.GetValue(result) != null)
                    userEmail = emailProp.GetValue(result) as string;

                var roleProp = result.GetType().GetProperty("Role");
                if (roleProp != null && roleProp.GetValue(result) != null)
                    role = roleProp.GetValue(result) as string;

                // Kiểm tra quyền Teacher/Giảng viên
                bool isTeacher = false;

                if (!string.IsNullOrEmpty(role) &&
                   (role.Equals("Teacher", StringComparison.OrdinalIgnoreCase) ||
                    role.Equals("Giảng viên", StringComparison.OrdinalIgnoreCase) ||
                    role.Equals("GiangVien", StringComparison.OrdinalIgnoreCase)))
                {
                    isTeacher = true;
                }

                if (!isTeacher)
                {
                    ViewBag.Error = "Bạn không có quyền truy cập! Chỉ tài khoản Teacher/Giảng viên mới được phép đăng nhập.";
                    return View();
                }

                // Lưu thông tin vào Session
                if (userId.HasValue)
                    Session["UserId"] = userId.Value;

                Session["FullName"] = fullName ?? "Teacher";
                Session["Email"] = userEmail ?? email;
                Session["Role"] = role ?? "Teacher";
                Session["IsAuthenticated"] = true;
                Session["LoginTime"] = DateTime.Now;

                // Redirect đến trang chủ
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                ViewBag.Error = "Đã xảy ra lỗi hệ thống. Vui lòng liên hệ quản trị viên.";
                return View();
            }
        }

        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                string userName = Session["FullName"] as string ?? "Người dùng";

                // Clear session
                Session.Clear();
                Session.Abandon();

                // Xóa session cookie
                Response.Cookies.Add(new System.Web.HttpCookie("ASP.NET_SessionId", "")
                {
                    Expires = DateTime.Now.AddDays(-1)
                });

                TempData["SuccessMessage"] = $"Đăng xuất thành công. Tạm biệt {userName}!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logout error: {ex.Message}");
                TempData["ErrorMessage"] = "Lỗi khi đăng xuất";
                return RedirectToAction("Index");
            }
        }

        // Helper method để debug token
        private void DebugTokenInfo()
        {
            try
            {
                var cookieToken = Request.Cookies["__RequestVerificationToken"]?.Value;
                var formToken = Request.Form["__RequestVerificationToken"];

                System.Diagnostics.Debug.WriteLine("=== ANTI-FORGERY TOKEN DEBUG ===");
                System.Diagnostics.Debug.WriteLine($"Cookie Token Exists: {cookieToken != null}");
                System.Diagnostics.Debug.WriteLine($"Form Token Exists: {formToken != null}");
                System.Diagnostics.Debug.WriteLine($"Cookie Count: {Request.Cookies.Count}");

                // Log tất cả cookies để debug
                foreach (string key in Request.Cookies.AllKeys)
                {
                    System.Diagnostics.Debug.WriteLine($"Cookie: {key} = {Request.Cookies[key]?.Value}");
                }
            }
            catch
            {
                // Ignore debug errors
            }
        }
    }
}
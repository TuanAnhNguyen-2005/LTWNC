using System;
using System.Web.Mvc;
using MVC_ADMIN.Services;

namespace MVC_ADMIN.Controllers
{
    /// <summary>
    /// Controller xử lý đăng nhập/đăng xuất
    /// Kế thừa BaseController để sử dụng các method chung
    /// </summary>
    public class LoginController : BaseController
    {
        private readonly UserDataService _userDataService;

        public LoginController()
        {
            _userDataService = new UserDataService();
        }

        // GET: /Login
        public ActionResult Index()
        {
            // Nếu đã đăng nhập thì redirect về trang chủ
            if (IsAuthenticated())
                return RedirectToHomeByRole();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string password)
        {
            try
            {
                // Debug thông tin đăng nhập
                System.Diagnostics.Debug.WriteLine($"=== ĐĂNG NHẬP ===");
                System.Diagnostics.Debug.WriteLine($"Email: {email}");
                System.Diagnostics.Debug.WriteLine($"Password: {password}");

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu!";
                    return View();
                }

                var result = _userDataService.LoginUser(email, password);

                // Debug kết quả đăng nhập
                System.Diagnostics.Debug.WriteLine($"Kết quả đăng nhập: {result.Success}");
                System.Diagnostics.Debug.WriteLine($"User ID: {result.UserId}");
                System.Diagnostics.Debug.WriteLine($"Full Name: {result.FullName}");
                System.Diagnostics.Debug.WriteLine($"Email: {result.Email}");
                System.Diagnostics.Debug.WriteLine($"Role: {result.Role}");

                if (!result.Success)
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng!";

                    // Test với tài khoản mẫu để debug
                    System.Diagnostics.Debug.WriteLine($"=== DEBUG: Kiểm tra thông tin ===");
                    System.Diagnostics.Debug.WriteLine($"Tài khoản mẫu: thanhtu98912@gmail.com / 12345");

                    return View();
                }

                // ✅ TẠM THỜI BỎ KIỂM TRA ADMIN - CHO PHÉP MỌI ROLE ĐĂNG NHẬP
                // bool isAdmin = result.RoleId == 1; // Sẽ thêm sau
                // if (!isAdmin)
                // {
                //     ViewBag.Error = "Bạn không đủ quyền truy cập! Chỉ có admin mới vào được trang web này !";
                //     return View();
                // }

                // ✅ Lưu session cho mọi role
                Session["UserId"] = result.UserId;
                Session["FullName"] = result.FullName;
                Session["Email"] = result.Email;
                Session["Role"] = result.Role;

                // Lưu thêm thông tin cho debug
                Session["LoginTime"] = DateTime.Now;
                Session["UserAgent"] = Request.UserAgent;

                // Debug thông tin session
                System.Diagnostics.Debug.WriteLine($"=== THÔNG TIN SESSION ĐÃ LƯU ===");
                System.Diagnostics.Debug.WriteLine($"Session ID: {Session.SessionID}");
                System.Diagnostics.Debug.WriteLine($"UserId: {Session["UserId"]}");
                System.Diagnostics.Debug.WriteLine($"Role: {Session["Role"]}");
                System.Diagnostics.Debug.WriteLine($"Email: {Session["Email"]}");

                // Kiểm tra redirect URL
                var redirectUrl = RedirectToHomeByRole();
                System.Diagnostics.Debug.WriteLine($"Redirect to: {redirectUrl}");

                return redirectUrl;
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi
                System.Diagnostics.Debug.WriteLine($"=== LỖI ĐĂNG NHẬP ===");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                HandleException(ex);
                ViewBag.Error = $"Đã xảy ra lỗi khi đăng nhập: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Đăng xuất
        /// </summary>
        public ActionResult Logout()
        {
            try
            {
                // Debug trước khi xóa session
                System.Diagnostics.Debug.WriteLine($"=== ĐĂNG XUẤT ===");
                System.Diagnostics.Debug.WriteLine($"User ID: {Session["UserId"]}");
                System.Diagnostics.Debug.WriteLine($"Email: {Session["Email"]}");

                // Lưu thông tin trước khi xóa (cho mục đích debug)
                var userId = Session["UserId"];
                var email = Session["Email"];

                // Xóa tất cả session
                Session.Clear();
                Session.Abandon();

                // Xóa cookie session
                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                }

                SetSuccessMessage("Đăng xuất thành công!");

                System.Diagnostics.Debug.WriteLine($"Đã đăng xuất user: {email} (ID: {userId})");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi đăng xuất: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Kiểm tra xem session có hợp lệ không (cho API/testing)
        /// </summary>
        [HttpGet]
        public ActionResult CheckSession()
        {
            var sessionInfo = new
            {
                IsAuthenticated = IsAuthenticated(),
                UserId = Session["UserId"],
                Email = Session["Email"],
                FullName = Session["FullName"],
                Role = Session["Role"],
                SessionId = Session.SessionID,
                LoginTime = Session["LoginTime"],
                SessionCount = Session.Count
            };

            return Json(sessionInfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Trang test đơn giản để kiểm tra đăng nhập
        /// </summary>
        public ActionResult Test()
        {
            return Content(@"
                <html>
                <head><title>Test Login</title></head>
                <body>
                    <h2>Test Login Form</h2>
                    <form action='/Login/Index' method='post'>
                        <input type='hidden' name='__RequestVerificationToken' value='" + GetAntiForgeryToken() + @"' />
                        <div>
                            <label>Email:</label><br/>
                            <input type='email' name='email' value='thanhtu98912@gmail.com' />
                        </div>
                        <div>
                            <label>Password:</label><br/>
                            <input type='password' name='password' value='12345' />
                        </div>
                        <div>
                            <button type='submit'>Đăng nhập</button>
                        </div>
                    </form>
                </body>
                </html>
            ");
        }

        /// <summary>
        /// Lấy AntiForgeryToken cho test
        /// </summary>
        private string GetAntiForgeryToken()
        {
            string cookieToken, formToken;
            System.Web.Helpers.AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return formToken;
        }
    }
}
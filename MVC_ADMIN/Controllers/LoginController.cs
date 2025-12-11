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
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu!";
                    return View();
                }

                // Đăng nhập từ database
                var result = _userDataService.LoginUser(email, password);

                if (result.Success)
                {
                    // Lưu thông tin user vào Session
                    Session["UserId"] = result.UserId;
                    Session["FullName"] = result.FullName;
                    Session["Email"] = result.Email;
                    Session["Role"] = result.Role; // Admin / Teacher / Student

                    // Chuyển hướng theo role
                    return RedirectToHomeByRole();
                }
                else
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                ViewBag.Error = "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            SetSuccessMessage("Đăng xuất thành công!");
            return RedirectToAction("Index");
        }
    }
}
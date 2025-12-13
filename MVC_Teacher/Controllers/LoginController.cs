using System;
using System.Web.Mvc;
using System.Web.Security;
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
        public ActionResult Index()
        {
            if (Session != null && Session["UserId"] != null)
                return RedirectToAction("Index", "Home");

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

                var result = _userDataService.LoginUser(email, password);

                if (result.Success)
                {
                    // Forms auth optional
                    FormsAuthentication.SetAuthCookie(result.Email, false);

                    Session["UserId"] = result.UserId;
                    Session["FullName"] = result.FullName;
                    Session["Email"] = result.Email;
                    Session["Role"] = result.Role;

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                ViewBag.Error = "Đã xảy ra lỗi khi đăng nhập. Vui lòng thử lại sau.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            TempData["Success"] = "Đăng xuất thành công";
            return RedirectToAction("Index");
        }
    }
}
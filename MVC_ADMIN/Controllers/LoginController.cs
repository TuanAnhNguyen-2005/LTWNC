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

                var result = _userDataService.LoginUser(email, password);

                if (!result.Success)
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                    return View();
                }

                // ✅ CHẶN QUYỀN: chỉ Admin (MaVaiTro = 1) mới được đăng nhập
                // (Ưu tiên kiểm tra RoleId nếu service có trả, còn không thì fallback qua Role string)
                bool isAdmin =
                    (result.Role != null && result.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                    || (result.GetType().GetProperty("RoleId") != null
                        && (int)result.GetType().GetProperty("RoleId").GetValue(result) == 1);

                if (!isAdmin)
                {
                    ViewBag.Error = "Bạn không đủ quyền truy cập! Chỉ có admin mới vào được trang web này !";
                    return View();
                }

                // ✅ Admin mới được lưu session
                Session["UserId"] = result.UserId;
                Session["FullName"] = result.FullName;
                Session["Email"] = result.Email;
                Session["Role"] = result.Role;

                return RedirectToHomeByRole();
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
using System;
using System.Web.Mvc;

namespace MVC_Teacher.Controllers
{
    /// <summary>
    /// Controller cho trang chủ của Teacher
    /// </summary>
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            try
            {
                // Kiểm tra đăng nhập đơn giản (dùng Session như các project khác)
                if (Session == null || Session["UserId"] == null)
                    return RedirectToAction("Index", "Login");

                ViewBag.Title = "Trang chủ";
                ViewBag.UserName = Session["FullName"]?.ToString() ?? string.Empty;
                ViewBag.UserRole = Session["Role"]?.ToString() ?? string.Empty;

                return View();
            }
            catch (Exception ex)
            {
                // Ghi log để debug, hiển thị view với thông báo lỗi nhẹ
                System.Diagnostics.Debug.WriteLine($"HomeController.Index error: {ex.Message}");
                TempData["Error"] = "Đã xảy ra lỗi. Vui lòng thử lại sau.";
                return View();
            }
        }

        // GET: Home/About
        public ActionResult About()
        {
            ViewBag.Title = "Giới thiệu";
            ViewBag.Message = "Mô tả ứng dụng.";
            return View();
        }

        // GET: Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Title = "Liên hệ";
            ViewBag.Message = "Thông tin liên hệ.";
            return View();
        }
    }
}
using System;
using System.Web.Mvc;

public class HomeController : Controller
{
    public ActionResult Index()
    {
        try
        {
            if (Session == null || Session["UserId"] == null)
                return RedirectToAction("Index", "Login");

            ViewBag.Title = "Trang chủ";
            ViewBag.UserName = Session["FullName"]?.ToString() ?? string.Empty;
            ViewBag.UserRole = Session["Role"]?.ToString() ?? string.Empty;

            // Thêm các ViewBag cho Dashboard stats
            ViewBag.TotalClasses = 12;
            ViewBag.TotalStudents = 154;
            ViewBag.CompletionRate = 87;
            ViewBag.AverageRating = 4.8;

            return View();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"HomeController.Index error: {ex.Message}");
            TempData["Error"] = "Đã xảy ra lỗi. Vui lòng thử lại sau.";
            return View();
        }
    }
}
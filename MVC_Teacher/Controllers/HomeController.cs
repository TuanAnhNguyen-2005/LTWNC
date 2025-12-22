using System;
using System.Web.Mvc;
using MVC_Teacher.Models;

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

                // Ưu tiên lấy từ Session TeacherData
                Teacher teacher = null;

                if (Session["TeacherData"] != null)
                {
                    teacher = Session["TeacherData"] as Teacher;
                }
                else if (TempData["UpdatedTeacher"] != null)
                {
                    // Nếu vừa update từ Profile
                    teacher = TempData["UpdatedTeacher"] as Teacher;
                    Session["TeacherData"] = teacher;
                }

                if (teacher != null)
                {
                    // Cập nhật Session riêng lẻ từ Teacher object
                    Session["FullName"] = teacher.FullName;
                    Session["Email"] = teacher.Email;
                    Session["Subject"] = teacher.Subject;
                    Session["Role"] = teacher.Role;

                    ViewBag.UserName = teacher.FullName;
                    ViewBag.UserRole = teacher.Role;
                    ViewBag.UserSubject = teacher.Subject;
                    ViewBag.UserEmail = teacher.Email;
                }
                else
                {
                    // Fallback: lấy từ Session riêng lẻ
                    ViewBag.UserName = Session["FullName"]?.ToString() ?? "Thầy/Cô";
                    ViewBag.UserRole = Session["Role"]?.ToString() ?? "Giáo viên";
                    ViewBag.UserSubject = Session["Subject"]?.ToString() ?? "Môn học";
                    ViewBag.UserEmail = Session["Email"]?.ToString() ?? "email@example.com";
                }

                // Thêm các ViewBag cho Dashboard stats
                ViewBag.TotalClasses = 12;
                ViewBag.TotalStudents = 154;
                ViewBag.CompletionRate = 87;
                ViewBag.AverageRating = 4.8;

                // Kiểm tra nếu vừa update từ Profile
                if (TempData["ProfileUpdated"] != null)
                {
                    ViewBag.ProfileUpdated = true;
                }

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

        // API để lấy thông tin Teacher hiện tại (cho JavaScript)
        [HttpGet]
        public JsonResult GetTeacherInfo()
        {
            var teacher = Session["TeacherData"] as Teacher;
            if (teacher == null)
            {
                teacher = new Teacher
                {
                    FullName = Session["FullName"] as string ?? "Thầy/Cô",
                    Subject = Session["Subject"] as string ?? "Môn học",
                    Email = Session["Email"] as string ?? "email@example.com"
                };
            }

            return Json(new
            {
                fullName = teacher.FullName,
                subject = teacher.Subject,
                email = teacher.Email,
                role = teacher.Role
            }, JsonRequestBehavior.AllowGet);
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
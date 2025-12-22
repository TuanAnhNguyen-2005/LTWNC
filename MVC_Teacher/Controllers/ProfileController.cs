using System;
using System.Web.Mvc;
using MVC_Teacher.Models;

namespace MVC_Teacher.Controllers
{
    // TẠM THỜI bỏ [Authorize] để test
    // [Authorize]
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            try
            {
                // Tạo dữ liệu test để View không bị lỗi null
                var teacher = new Teacher
                {
                    TeacherId = 1,
                    FullName = Session["FullName"]?.ToString() ?? "Nguyễn Văn A (Test)",
                    Email = Session["Email"]?.ToString() ?? "teacher@example.com",
                    PhoneNumber = "0912345678",
                    Subject = Session["Subject"]?.ToString() ?? "Toán",
                    Role = "Giáo viên",
                    IsActive = true,
                    IsVerified = true,
                    CreatedDate = DateTime.Now.AddMonths(-3),
                    LastLoginDate = DateTime.Now.AddHours(-2)
                };

                ViewBag.LoginCount = 15;
                return View(teacher);
            }
            catch (Exception ex)
            {
                // Trả về view lỗi nếu có exception
                ViewBag.Error = ex.Message;
                return View(new Teacher());
            }
        }

        // POST: Profile/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Teacher model)
        {
            if (ModelState.IsValid)
            {
                // Lưu vào Session tạm thời
                Session["FullName"] = model.FullName;
                Session["Subject"] = model.Subject;

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }

            return View("Index", model);
        }
    }
}
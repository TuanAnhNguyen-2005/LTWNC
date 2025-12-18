// Controllers/ProfileController.cs
using System;
using System.Linq;
using System.Web.Mvc;
using MVC_Teacher.Models;
using MVC_Teacher.Services;
using MVC_Teacher.Helpers;

namespace MVC_Teacher.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserDataService _userService = new UserDataService();

        // GET: Profile
        public ActionResult Index()
        {
            var email = User.Identity.Name;
            var teacher = _userService.GetTeacherByEmail(email);

            if (teacher == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.LoginCount = _userService.GetLoginCountForTeacher(teacher.TeacherId);

            return View(teacher);
        }

        // POST: Profile/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Teacher model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                var email = User.Identity.Name;
                var teacher = _userService.GetTeacherByEmail(email);

                if (teacher == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin giáo viên!";
                    return RedirectToAction("Index");
                }

                teacher.FullName = model.FullName;
                teacher.PhoneNumber = model.PhoneNumber;
                teacher.Subject = model.Subject;

                _userService.UpdateTeacher(teacher);

                // Update session
                Session["FullName"] = teacher.FullName;
                Session["Subject"] = teacher.Subject;

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update profile error: {ex.Message}");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật thông tin!";
                return RedirectToAction("Index");
            }
        }

        // POST: Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPassword, string newPassword)
        {
            try
            {
                var email = User.Identity.Name;
                var teacher = _userService.GetTeacherByEmail(email);

                if (teacher == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy tài khoản!" });
                }

                // Verify current password
                if (!PasswordHasher.VerifyPassword(currentPassword, teacher.PasswordHash))
                {
                    return Json(new { success = false, message = "Mật khẩu hiện tại không đúng!" });
                }

                // Update password
                teacher.PasswordHash = PasswordHasher.HashPassword(newPassword);
                _userService.UpdateTeacher(teacher);

                return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Change password error: {ex.Message}");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi đổi mật khẩu!" });
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_ADMIN.Filters;

namespace MVC_ADMIN.Controllers
{
    [AuthorizeRole("Admin", "Teacher", "Student")]
    public class ProfileController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "Thông tin cá nhân";
                
                var userId = GetUserId();
                if (!userId.HasValue)
                    return RedirectToLogin();

                // Gọi API lấy thông tin profile
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"users/{userId}/profile");
                
                if (response.Success)
                {
                    ViewBag.Profile = response.Data;
                }
                else
                {
                    // Nếu API lỗi, vẫn hiển thị thông tin từ Session
                    ViewBag.FullName = GetUserFullName();
                    ViewBag.Email = GetUserEmail();
                    ViewBag.Role = GetUserRole();
                }
                
                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                // Fallback: hiển thị thông tin từ Session
                ViewBag.FullName = GetUserFullName();
                ViewBag.Email = GetUserEmail();
                ViewBag.Role = GetUserRole();
                return View();
            }
        }

        public async Task<ActionResult> Edit()
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa thông tin";
                
                var userId = GetUserId();
                if (!userId.HasValue)
                    return RedirectToLogin();

                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"users/{userId}/profile");
                
                if (response.Success)
                {
                    ViewBag.Profile = response.Data;
                }
                
                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(dynamic profile)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(profile);

                var userId = GetUserId();
                if (!userId.HasValue)
                    return RedirectToLogin();

                var response = await ApiService.PutAsync<dynamic, dynamic>($"users/{userId}/profile", profile);
                
                if (response != null)
                {
                    // Cập nhật Session nếu có thay đổi
                    if (profile.fullName != null)
                        Session["FullName"] = profile.fullName;
                    if (profile.email != null)
                        Session["Email"] = profile.email;

                    SetSuccessMessage("Cập nhật thông tin thành công!");
                    return RedirectToAction("Index");
                }
                
                SetErrorMessage("Cập nhật thông tin thất bại!");
                return View(profile);
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi cập nhật thông tin");
                return View(profile);
            }
        }
    }
}
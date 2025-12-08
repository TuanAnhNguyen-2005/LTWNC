using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_ADMIN.Filters;

namespace MVC_ADMIN.Controllers
{
    [AuthorizeRole("Admin")]
    public class PermissionController : BaseController
    {
        // Temporary debug endpoint — remove after debugging
        [AllowAnonymous]
        public ActionResult DebugSession()
        {
            var userId = Session["UserId"] ?? "null";
            var email = Session["Email"] ?? "null";
            var role = Session["Role"] ?? "null";
            return Content($"UserId={userId}, Email={email}, Role={role}");
        }

        // Allow anonymous for Index while debugging routing/auth — remove AllowAnonymous when done
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "Quản lý quyền";

                // Gọi API lấy danh sách quyền
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>("permissions");

                if (response.Success)
                {
                    ViewBag.Permissions = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Error ?? "Không thể tải danh sách quyền");
                }

                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải danh sách quyền");
                return View();
            }
        }

        // Make GET create accessible for debugging like Category — POST remains protected by AuthorizeRole on controller
        [AllowAnonymous]
        public ActionResult Create()
        {
            ViewBag.Title = "Thêm quyền mới";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            try
            {
                var permissionData = new
                {
                    permissionName = form["permissionName"],
                    displayName = form["displayName"],
                    description = form["description"],
                    module = form["module"],
                    isActive = form["isActive"] == "on" || form["isActive"] == "true"
                };

                var response = await ApiService.PostAsync<dynamic, dynamic>("permissions", permissionData);

                if (response != null)
                {
                    SetSuccessMessage("Thêm quyền thành công!");
                    return RedirectToAction("Index");
                }

                SetErrorMessage("Thêm quyền thất bại!");
                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi thêm quyền");
                return View();
            }
        }

        // Allow anonymous GET details for debugging; POST/modify actions remain protected
        [AllowAnonymous]
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"permissions/{id}");

                if (response.Success)
                {
                    ViewBag.Permission = response.Data;
                    return View();
                }

                SetErrorMessage(response.Error ?? "Không tìm thấy quyền");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải thông tin quyền");
                return RedirectToAction("Index");
            }
        }

        // Allow anonymous GET edit page for debugging; POST update stays protected
        [AllowAnonymous]
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa quyền";

                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"permissions/{id}");

                if (response.Success)
                {
                    ViewBag.Permission = response.Data;
                    return View();
                }

                SetErrorMessage(response.Error ?? "Không tìm thấy quyền");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải thông tin quyền");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FormCollection form)
        {
            try
            {
                int permissionId = int.Parse(form["permissionId"]);

                var permissionData = new
                {
                    permissionName = form["permissionName"],
                    displayName = form["displayName"],
                    description = form["description"],
                    module = form["module"],
                    isActive = form["isActive"] == "on" || form["isActive"] == "true"
                };

                var response = await ApiService.PutAsync<dynamic, dynamic>($"permissions/{permissionId}", permissionData);

                if (response != null)
                {
                    SetSuccessMessage("Cập nhật quyền thành công!");
                    return RedirectToAction("Index");
                }

                SetErrorMessage("Cập nhật quyền thất bại!");
                return RedirectToAction("Edit", new { id = permissionId });
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi cập nhật quyền");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var success = await ApiService.DeleteAsync($"permissions/{id}");

                if (success)
                {
                    SetSuccessMessage("Xóa quyền thành công!");
                }
                else
                {
                    SetErrorMessage("Xóa quyền thất bại!");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi xóa quyền");
                return RedirectToAction("Index");
            }
        }
    }
}
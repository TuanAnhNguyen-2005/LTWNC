using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_ADMIN.Filters;

namespace MVC_ADMIN.Controllers
{
    [AuthorizeRole("Admin")]
    public class UserController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "Quản lý người dùng";
                
                // Gọi API lấy danh sách user
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>("users");
                
                if (response.Success)
                {
                    ViewBag.Users = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Error ?? "Không thể tải danh sách người dùng");
                }
                
                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải danh sách người dùng");
                return View();
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"users/{id}");
                
                if (response.Success)
                {
                    ViewBag.User = response.Data;
                    return View();
                }
                
                SetErrorMessage(response.Error ?? "Không tìm thấy người dùng");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải thông tin người dùng");
                return RedirectToAction("Index");
            }
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Thêm người dùng mới";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            try
            {
                var userData = new
                {
                    fullName = form["fullName"],
                    email = form["email"],
                    password = form["password"],
                    role = form["role"],
                    phone = form["phone"],
                    address = form["address"]
                };

                var response = await ApiService.PostAsync<dynamic, dynamic>("users", userData);
                
                if (response != null)
                {
                    SetSuccessMessage("Thêm người dùng thành công!");
                    return RedirectToAction("Index");
                }
                
                SetErrorMessage("Thêm người dùng thất bại!");
                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi thêm người dùng");
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa người dùng";
                
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"users/{id}");
                
                if (response.Success)
                {
                    ViewBag.User = response.Data;
                    return View();
                }
                
                SetErrorMessage(response.Error ?? "Không tìm thấy người dùng");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải thông tin người dùng");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FormCollection form)
        {
            try
            {
                int userId = int.Parse(form["userId"]);
                
                var userData = new
                {
                    fullName = form["fullName"],
                    email = form["email"],
                    password = !string.IsNullOrEmpty(form["password"]) ? form["password"] : null,
                    role = form["role"],
                    phone = form["phone"],
                    address = form["address"],
                    isActive = form["isActive"] == "on" || form["isActive"] == "true"
                };

                var response = await ApiService.PutAsync<dynamic, dynamic>($"users/{userId}", userData);
                
                if (response != null)
                {
                    SetSuccessMessage("Cập nhật người dùng thành công!");
                    return RedirectToAction("Index");
                }
                
                SetErrorMessage("Cập nhật người dùng thất bại!");
                return RedirectToAction("Edit", new { id = userId });
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi cập nhật người dùng");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var success = await ApiService.DeleteAsync($"users/{id}");
                
                if (success)
                {
                    SetSuccessMessage("Xóa người dùng thành công!");
                }
                else
                {
                    SetErrorMessage("Xóa người dùng thất bại!");
                }
                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi xóa người dùng");
                return RedirectToAction("Index");
            }
        }
    }
}
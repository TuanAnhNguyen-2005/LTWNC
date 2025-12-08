using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_ADMIN.Filters;

namespace MVC_ADMIN.Controllers
{
    [AuthorizeRole("Admin")]
    public class CategoryController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "Quản lý danh mục";
                
                // Gọi API lấy danh sách danh mục
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>("categories");
                
                if (response.Success)
                {
                    ViewBag.Categories = response.Data;
                }
                else
                {
                    SetErrorMessage(response.Error ?? "Không thể tải danh sách danh mục");
                }
                
                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải danh sách danh mục");
                return View();
            }
        }

        public ActionResult Create()
        {
            ViewBag.Title = "Thêm danh mục mới";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            try
            {
                var categoryData = new
                {
                    categoryName = form["categoryName"],
                    slug = form["slug"],
                    description = form["description"],
                    parentCategoryId = !string.IsNullOrEmpty(form["parentCategoryId"]) ? int.Parse(form["parentCategoryId"]) : (int?)null,
                    displayOrder = !string.IsNullOrEmpty(form["displayOrder"]) ? int.Parse(form["displayOrder"]) : 0,
                    isActive = form["isActive"] == "on" || form["isActive"] == "true"
                };

                var response = await ApiService.PostAsync<dynamic, dynamic>("categories", categoryData);
                
                if (response != null)
                {
                    SetSuccessMessage("Thêm danh mục thành công!");
                    return RedirectToAction("Index");
                }
                
                SetErrorMessage("Thêm danh mục thất bại!");
                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi thêm danh mục");
                return View();
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"categories/{id}");
                
                if (response.Success)
                {
                    ViewBag.Category = response.Data;
                    return View();
                }
                
                SetErrorMessage(response.Error ?? "Không tìm thấy danh mục");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải thông tin danh mục");
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa danh mục";
                
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>($"categories/{id}");
                
                if (response.Success)
                {
                    ViewBag.Category = response.Data;
                    return View();
                }
                
                SetErrorMessage(response.Error ?? "Không tìm thấy danh mục");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi tải thông tin danh mục");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FormCollection form)
        {
            try
            {
                int categoryId = int.Parse(form["categoryId"]);
                
                var categoryData = new
                {
                    categoryName = form["categoryName"],
                    slug = form["slug"],
                    description = form["description"],
                    parentCategoryId = !string.IsNullOrEmpty(form["parentCategoryId"]) ? int.Parse(form["parentCategoryId"]) : (int?)null,
                    displayOrder = !string.IsNullOrEmpty(form["displayOrder"]) ? int.Parse(form["displayOrder"]) : 0,
                    isActive = form["isActive"] == "on" || form["isActive"] == "true"
                };

                var response = await ApiService.PutAsync<dynamic, dynamic>($"categories/{categoryId}", categoryData);
                
                if (response != null)
                {
                    SetSuccessMessage("Cập nhật danh mục thành công!");
                    return RedirectToAction("Index");
                }
                
                SetErrorMessage("Cập nhật danh mục thất bại!");
                return RedirectToAction("Edit", new { id = categoryId });
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi cập nhật danh mục");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var success = await ApiService.DeleteAsync($"categories/{id}");
                
                if (success)
                {
                    SetSuccessMessage("Xóa danh mục thành công!");
                }
                else
                {
                    SetErrorMessage("Xóa danh mục thất bại!");
                }
                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Đã xảy ra lỗi khi xóa danh mục");
                return RedirectToAction("Index");
            }
        }
    }
}
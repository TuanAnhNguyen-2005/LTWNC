using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_ADMIN.Filters;
using MVC_ADMIN.Services;
using Newtonsoft.Json;

namespace MVC_ADMIN.Controllers
{
    [AuthorizeRole("Admin")]
    public class CategoryController : BaseController
    {
        private readonly ApiService _api = ApiServiceHelper.Instance;

        // GET: /Category
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "Quản lý danh mục";

                var res = await _api.GetWithErrorHandlingAsync<dynamic>("categories");

                if (res.Success)
                    ViewBag.Categories = res.Data;
                else
                    SetErrorMessage(res.Error);

                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex, "Không tải được danh mục");
                return View();
            }
        }

        // GET: /Category/Create
        public ActionResult Create()
        {
            ViewBag.Title = "Thêm danh mục";
            return View();
        }

        // POST: /Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FormCollection form)
        {
            try
            {
                var data = new
                {
                    categoryName = form["categoryName"],
                    slug = form["slug"],
                    description = form["description"],
                    isActive = form["isActive"] == "on"
                };

                await _api.PostAsync<dynamic, dynamic>("categories", data);
                SetSuccessMessage("Thêm danh mục thành công!");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Thêm danh mục thất bại");
                return View();
            }
        }

        // GET: /Category/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var res = await _api.GetWithErrorHandlingAsync<dynamic>($"categories/{id}");
                if (!res.Success) return RedirectToAction("Index");

                ViewBag.Category = res.Data;
                ViewBag.Title = "Chỉnh sửa danh mục";
                return View();
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // POST: /Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FormCollection form)
        {
            try
            {
                int id = int.Parse(form["categoryId"]);

                var data = new
                {
                    categoryName = form["categoryName"],
                    slug = form["slug"],
                    description = form["description"],
                    isActive = form["isActive"] == "on"
                };

                await _api.PutAsync<dynamic, dynamic>($"categories/{id}", data);
                SetSuccessMessage("Cập nhật danh mục thành công!");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Cập nhật thất bại");
                return RedirectToAction("Index");
            }
        }

        // POST: /Category/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _api.DeleteAsync($"categories/{id}");
                SetSuccessMessage("Xóa danh mục thành công!");
            }
            catch (Exception ex)
            {
                HandleException(ex, "Xóa danh mục thất bại");
            }

            return RedirectToAction("Index");
        }
    }
}

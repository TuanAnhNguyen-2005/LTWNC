using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_ADMIN.Filters;

namespace MVC_ADMIN.Controllers
{
    [AuthorizeRole("Admin")]
    public class StatisticalController : BaseController
    {
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "Dashboard Admin";

                // Gọi API lấy thống kê
                var response = await ApiService.GetWithErrorHandlingAsync<dynamic>("statistics/dashboard");

                if (response.Success)
                {
                    ViewBag.Statistics = response.Data;
                }
                else
                {
                    // Nếu API lỗi, vẫn hiển thị trang với dữ liệu mặc định
                    ViewBag.Statistics = new
                    {
                        totalUsers = 0,
                        totalQuizzes = 0,
                        totalMaterials = 0,
                        totalCategories = 0
                    };
                }

                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                // Vẫn hiển thị trang với dữ liệu mặc định
                ViewBag.Statistics = new
                {
                    totalUsers = 0,
                    totalQuizzes = 0,
                    totalMaterials = 0,
                    totalCategories = 0
                };
                return View();
            }
        }
    }
}
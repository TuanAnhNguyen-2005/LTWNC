// Controllers/QuizController.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using MVC_Teacher.Models; // Đảm bảo có namespace này
namespace MVC_Teacher.Controllers
{
    public class QuizController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7068/api/Quiz"; // Thay bằng cổng API của bạn

        public QuizController()
        {
            _httpClient = new HttpClient();
        }

        // GET: /Quiz/Create
        public async Task<ActionResult> Create(int? maKhoaHoc)
        {
            var model = new QuizCreateViewModel
            {
                MaGiaoVien = GetCurrentTeacherId(), // Hàm bạn tự viết dựa trên Session/User
                MaKhoaHoc = maKhoaHoc,
                ThoiGianLamBai = 30,
                CauHois = new List<CauHoiViewModel>
                {
                    new CauHoiViewModel // Thêm 1 câu hỏi mặc định
                    {
                        LoaiCauHoi = "MultipleChoice",
                        Diem = 1,
                        LuaChons = new List<LuaChonViewModel>
                        {
                            new LuaChonViewModel { NoiDung = "", LaDapAnDung = true },
                            new LuaChonViewModel { NoiDung = "", LaDapAnDung = false }
                        }
                    }
                }
            };

            // Load danh sách khóa học của giáo viên (tùy chọn)
            ViewBag.KhoaHocList = await GetKhoaHocCuaGiaoVien(model.MaGiaoVien);

            return View(model);
        }

        // POST: /Quiz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(QuizCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.KhoaHocList = await GetKhoaHocCuaGiaoVien(model.MaGiaoVien);
                return View(model);
            }

            model.MaGiaoVien = GetCurrentTeacherId();

            var json = JsonConvert.SerializeObject(model.ToDto());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo quiz thành công! Bạn có thể tiếp tục tạo quiz mới hoặc xem trong danh sách khóa học.";
                return RedirectToAction("Create"); // QUAN TRỌNG: phải là "Create"
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Lỗi khi tạo quiz: " + error;
            }

            ViewBag.KhoaHocList = await GetKhoaHocCuaGiaoVien(model.MaGiaoVien);
            return View(model);
        }

        // Hàm giả lập lấy ID giáo viên hiện tại (bạn thay bằng logic thực tế)
        private int GetCurrentTeacherId()
        {
            if (Session["MaNguoiDung"] != null && int.TryParse(Session["MaNguoiDung"].ToString(), out int id))
            {
                return id;
            }
            return 2; // Giá trị mặc định để test
        }

        // Hàm lấy danh sách khóa học của giáo viên (gọi API khác nếu cần)
        private async Task<List<SelectListItem>> GetKhoaHocCuaGiaoVien(int maGiaoVien)
        {
            // Gọi API lấy khóa học: GET /api/KhoaHoc/teacher/{id}
            // Tạm thời trả về mẫu
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Lập trình C#" },
                new SelectListItem { Value = "2", Text = "ASP.NET Core" }
            };
        }
    }
}
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

        // GET: /Quiz/Create?maKhoaHoc=5
        public async Task<ActionResult> Create(int? maKhoaHoc)
        {
            var model = new QuizCreateViewModel
            {
                MaGiaoVien = GetCurrentTeacherId(),
                MaKhoaHoc = maKhoaHoc,
                ThoiGianLamBai = 30,
                CauHois = new List<CauHoiViewModel>
                {
                    new CauHoiViewModel
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

            // Nếu có maKhoaHoc → lấy tên khóa học để hiển thị
            if (maKhoaHoc.HasValue)
            {
                var tenKhoaHoc = await GetTenKhoaHoc(maKhoaHoc.Value);
                ViewBag.TenKhoaHocHienTai = tenKhoaHoc ?? "Khóa học #" + maKhoaHoc;
                ViewBag.CoKhoaHoc = true;
            }
            else
            {
                // Nếu không có → cho phép chọn từ dropdown
                ViewBag.KhoaHocList = await GetKhoaHocCuaGiaoVien(model.MaGiaoVien);
                ViewBag.CoKhoaHoc = false;
            }

            return View(model);
        }

        // POST: /Quiz/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(QuizCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Load lại dữ liệu khi validation lỗi
                if (model.MaKhoaHoc.HasValue)
                {
                    ViewBag.CoKhoaHoc = true;
                    ViewBag.TenKhoaHocHienTai = await GetTenKhoaHoc(model.MaKhoaHoc.Value) ?? "Khóa học đã chọn";
                }
                else
                {
                    ViewBag.KhoaHocList = await GetKhoaHocCuaGiaoVien(model.MaGiaoVien);
                    ViewBag.CoKhoaHoc = false;
                }

                return View(model);
            }

            model.MaGiaoVien = GetCurrentTeacherId();

            var json = JsonConvert.SerializeObject(model.ToDto());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiBaseUrl, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo quiz thành công! Bạn có thể tiếp tục tạo quiz mới hoặc xem trong danh sách khóa học.";
                return RedirectToAction("Create"); // Quay lại form tạo mới
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Lỗi khi tạo quiz: " + error;
            }

            // Nếu có lỗi từ API → load lại giao diện
            if (model.MaKhoaHoc.HasValue)
            {
                ViewBag.CoKhoaHoc = true;
                ViewBag.TenKhoaHocHienTai = await GetTenKhoaHoc(model.MaKhoaHoc.Value);
            }
            else
            {
                ViewBag.KhoaHocList = await GetKhoaHocCuaGiaoVien(model.MaGiaoVien);
                ViewBag.CoKhoaHoc = false;
            }

            return View(model);
        }

        // Lấy tên khóa học từ API (để hiển thị khi đã chọn sẵn)
        private async Task<string> GetTenKhoaHoc(int maKhoaHoc)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7068/api/Courses/{maKhoaHoc}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var khoahoc = JsonConvert.DeserializeObject<dynamic>(json);
                    return khoahoc.tenKhoaHoc ?? "Khóa học không tên";
                }
            }
            catch (Exception ex)
            {
                // Có thể log lỗi nếu cần
                System.Diagnostics.Debug.WriteLine("Lỗi lấy tên khóa học: " + ex.Message);
            }
            return null;
        }

        // Hàm lấy danh sách khóa học của giáo viên (dùng cho dropdown khi không truyền maKhoaHoc)
        private async Task<List<SelectListItem>> GetKhoaHocCuaGiaoVien(int maGiaoVien)
        {
            var list = new List<SelectListItem>();

            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7068/api/Courses/teacher/{maGiaoVien}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var courses = JsonConvert.DeserializeObject<List<dynamic>>(json);

                    foreach (var c in courses)
                    {
                        list.Add(new SelectListItem
                        {
                            Value = c.maKhoaHoc.ToString(),
                            Text = c.tenKhoaHoc.ToString()
                        });
                    }

                    return list;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi lấy danh sách khóa học giáo viên: " + ex.Message);
            }

            // Dữ liệu mẫu nếu gọi API lỗi (để không crash khi dev)
            list.Add(new SelectListItem { Value = "1", Text = "Lập trình C# (mẫu)" });
            list.Add(new SelectListItem { Value = "2", Text = "ASP.NET Core (mẫu)" });

            return list;
        }

        // Hàm giả lập lấy ID giáo viên hiện tại
        private int GetCurrentTeacherId()
        {
            if (Session["MaNguoiDung"] != null && int.TryParse(Session["MaNguoiDung"].ToString(), out int id))
            {
                return id;
            }
            return 2; // Giá trị test
        }
    }
}
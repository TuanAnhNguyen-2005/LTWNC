// Controllers/QuizController.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Serialization;
using MVC_Teacher.Models; // Đảm bảo có namespace này
using MVC_Teacher.Services; // Thêm namespace cho service

namespace MVC_Teacher.Controllers
{
    public class QuizController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly APIClientService _apiService = new APIClientService();
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
        // GET: /Quiz/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy quiz.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var quizData = JsonConvert.DeserializeObject<QuizDto>(json);

                var model = new QuizDetailViewModel
                {
                    MaQuiz = quizData.MaQuiz,
                    TenQuiz = quizData.TenQuiz,
                    MoTa = quizData.MoTa,
                    TrangThai = quizData.TrangThai,
                    ThoiGianLamBai = quizData.ThoiGianLamBai,
                    NgayTao = quizData.NgayTao,
                    SoCauHoi = quizData.CauHois?.Count ?? 0,
                    TongDiem = quizData.CauHois?.Sum(ch => ch.Diem) ?? 0,
                    SoHocSinhLamBai = 0, // TODO: Lấy từ API KetQuaQuiz
                    DiemTrungBinh = 7.5,
                    PhanTramHoanThanh = 85,
                    PhanTramDat = 60,
                    CauHois = quizData.CauHois?.Select(ch => new CauHoiDetailDto
                    {
                        MaCauHoi = ch.MaCauHoi,
                        NoiDung = ch.NoiDung,
                        LoaiCauHoi = ch.LoaiCauHoi,
                        Diem = ch.Diem,
                        LuaChons = ch.LuaChons?.Select(lc => new LuaChonDetailDto
                        {
                            MaLuaChon = lc.MaLuaChon,
                            NoiDung = lc.NoiDung,
                            LaDapAnDung = lc.LaDapAnDung
                        }).ToList() ?? new List<LuaChonDetailDto>()
                    }).ToList() ?? new List<CauHoiDetailDto>()
                };

                if (quizData.MaKhoaHoc != null)
                {
                    var tenKhoaHoc = await GetTenKhoaHoc(quizData.MaKhoaHoc.Value);
                    ViewBag.TenKhoaHoc = tenKhoaHoc ?? "Khóa học #" + quizData.MaKhoaHoc;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi tải chi tiết: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: /Quiz/Delete/5 - Xác nhận xóa
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không tìm thấy quiz.";
                    return RedirectToAction("Index");
                }

                var json = await response.Content.ReadAsStringAsync();
                var quizData = JsonConvert.DeserializeObject<dynamic>(json);

                var model = new QuizDeleteViewModel
                {
                    MaQuiz = id,
                    TenQuiz = quizData.tenQuiz,
                    TenKhoaHoc = "Khóa học mẫu", // Lấy từ API Courses
                    SoCauHoi = quizData.cauHois?.Count ?? 0,
                    SoHocSinhLamBai = 3 // Lấy từ thống kê KetQuaQuiz
                };

                return View(model);
            }
            catch
            {
                TempData["Error"] = "Lỗi tải thông tin quiz.";
                return RedirectToAction("Index");
            }
        }

        // POST: /Quiz/Delete/5 - Xóa thật
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đã xóa quiz vĩnh viễn thành công!";
                    // Chuyển về trang danh sách khóa học
                    return RedirectToAction("MyCourses", "KhoaHoc");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Lỗi xóa quiz: " + error;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối API: " + ex.Message;
            }

            // Nếu lỗi → quay lại trang chi tiết
            return RedirectToAction("Details", new { id });
        }
        // GET: /Quiz/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không tìm thấy quiz.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var quizData = JsonConvert.DeserializeObject<QuizDto>(json);

            var model = new QuizCreateViewModel
            {
                MaQuiz = quizData.MaQuiz,
                TenQuiz = quizData.TenQuiz,
                MoTa = quizData.MoTa,
                MaKhoaHoc = quizData.MaKhoaHoc,
                ThoiGianLamBai = quizData.ThoiGianLamBai,
                // Load câu hỏi nếu cần chỉnh sửa
                CauHois = quizData.CauHois?.Select(ch => new CauHoiViewModel
                {
                    NoiDung = ch.NoiDung,
                    LoaiCauHoi = ch.LoaiCauHoi,
                    Diem = (float)ch.Diem,
                    LuaChons = ch.LuaChons?.Select(lc => new LuaChonViewModel
                    {
                        NoiDung = lc.NoiDung,
                        LaDapAnDung = lc.LaDapAnDung
                    }).ToList() ?? new List<LuaChonViewModel>()
                }).ToList() ?? new List<CauHoiViewModel>()
            };

            return View(model);
        }

        // POST: /Quiz/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, QuizCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.MaQuiz = id; // Đảm bảo id đúng

            var json = JsonConvert.SerializeObject(model.ToDto());
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{_apiBaseUrl}/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật quiz thành công!";
                return RedirectToAction("Details", new { id });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Lỗi cập nhật: " + error;
                return View(model);
            }
        }


        // GET: /Quiz/QuizzesByCourse?maKhoaHoc=3
        public async Task<ActionResult> QuizzesByCourse(int maKhoaHoc)
        {
            try
            {
                ViewBag.DebugInfo = new List<string>();
                ViewBag.DebugInfo.Add($"[DEBUG] maKhoaHoc = {maKhoaHoc}");

                // 1. Lấy khóa học qua service
                var courseData = await _apiService.GetCourseByIdAsync(maKhoaHoc);
                ViewBag.DebugInfo.Add($"[DEBUG] Khóa học: {courseData?.TenKhoaHoc}");

                if (courseData == null)
                {
                    TempData["Error"] = "Không tìm thấy khóa học.";
                    return RedirectToAction("MyCourses", "KhoaHoc");
                }

                // 2. Lấy quiz của giáo viên
                var teacherId = GetCurrentTeacherId();
                var allQuizzes = await _apiService.GetQuizzesByTeacherAsync(teacherId);

                var quizzes = allQuizzes
                    .Where(q => q.MaKhoaHoc == maKhoaHoc)
                    .Select(q => new QuizListItemVm
                    {
                        MaQuiz = q.MaQuiz,
                        TenQuiz = q.TenQuiz,
                        TrangThai = q.TrangThai,
                        ThoiGianLamBai = q.ThoiGianLamBai
                    })
                    .ToList();

                var model = new QuizListByCourseViewModel
                {
                    MaKhoaHoc = maKhoaHoc,
                    TenKhoaHoc = courseData.TenKhoaHoc ?? "Khóa học không tên",
                    Quizzes = quizzes
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.DebugInfo = new List<string> { $"[EXCEPTION] {ex.Message}" };
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("MyCourses", "KhoaHoc");
            }
        }
        // GET: /Quiz/ (danh sách tất cả quiz của giáo viên)
        public async Task<ActionResult> Index()
        {
            try
            {
                var teacherId = GetCurrentTeacherId();
                var allQuizzes = await _apiService.GetQuizzesByTeacherAsync(teacherId);

                var model = allQuizzes.Select(q => new QuizListItemVm
                {
                    MaQuiz = q.MaQuiz,
                    TenQuiz = q.TenQuiz,
                    TrangThai = q.TrangThai,
                    ThoiGianLamBai = q.ThoiGianLamBai,
                    SoCauHoi = q.CauHois?.Count ?? 0
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi tải danh sách quiz: " + ex.Message;
                return View(new List<QuizListItemVm>());
            }
        }
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
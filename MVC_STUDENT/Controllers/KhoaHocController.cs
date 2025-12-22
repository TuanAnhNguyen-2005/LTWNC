// MVC_Student/Controllers/KhoaHocController.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MVC_Student.Controllers
{
    public class KhoaHocController : Controller
    {
        // Khai báo HttpClient duy nhất cho toàn controller
        private readonly HttpClient _httpClient;
        private readonly string _apiCoursesUrl = "https://localhost:7068/api/Courses";
        private readonly string _apiQuizUrl = "https://localhost:7068/api/Quiz";

        // Constructor để khởi tạo HttpClient
        public KhoaHocController()
        {
            _httpClient = new HttpClient();
        }

        // GET: /KhoaHoc - Danh sách khóa học công khai cho học sinh
        public async Task<ActionResult> Index()
        {
            List<KhoaHocDto> khoaHocs = new List<KhoaHocDto>();

            try
            {
                var response = await _httpClient.GetAsync($"{_apiCoursesUrl}/published");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    khoaHocs = JsonConvert.DeserializeObject<List<KhoaHocDto>>(json) ?? new List<KhoaHocDto>();
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần (hoặc để trống)
                TempData["Error"] = "Không thể tải danh sách khóa học. Vui lòng thử lại sau.";
            }

            return View(khoaHocs);
        }

        // GET: /KhoaHoc/Detail/5 - Chi tiết khóa học + danh sách Quiz
        // GET: /KhoaHoc/Detail/5
        public async Task<ActionResult> Detail(int id)
        {
            KhoaHocDto khoaHoc = null;
            List<QuizDto> quizzes = new List<QuizDto>();
            string debugInfo = ""; // Để hiển thị lỗi chi tiết trên view (tạm thời)

            try
            {
                debugInfo += $"Đang gọi API khóa học: {_apiCoursesUrl}/{id}<br/>";

                var courseResponse = await _httpClient.GetAsync($"{_apiCoursesUrl}/{id}");
                debugInfo += $"Status Code khóa học: {courseResponse.StatusCode}<br/>";

                if (!courseResponse.IsSuccessStatusCode)
                {
                    var errorContent = await courseResponse.Content.ReadAsStringAsync();
                    debugInfo += $"Lỗi API khóa học: {errorContent}<br/>";
                    TempData["Error"] = "Không tìm thấy hoặc không thể tải khóa học (API lỗi).";
                    ViewBag.DebugInfo = debugInfo; // Hiển thị tạm trên view để debug
                    return RedirectToAction("Index");
                }

                var json = await courseResponse.Content.ReadAsStringAsync();
                debugInfo += $"JSON nhận được: {json}<br/>";
                khoaHoc = JsonConvert.DeserializeObject<KhoaHocDto>(json);

                // Lấy quiz
                debugInfo += $"Đang gọi API quiz: {_apiQuizUrl}/khoahoc/{id}/published<br/>";
                var quizResponse = await _httpClient.GetAsync($"{_apiQuizUrl}/khoahoc/{id}/published");
                debugInfo += $"Status Code quiz: {quizResponse.StatusCode}<br/>";

                if (quizResponse.IsSuccessStatusCode)
                {
                    var quizJson = await quizResponse.Content.ReadAsStringAsync();
                    quizzes = JsonConvert.DeserializeObject<List<QuizDto>>(quizJson) ?? new List<QuizDto>();
                }
                else
                {
                    var quizError = await quizResponse.Content.ReadAsStringAsync();
                    debugInfo += $"Lỗi API quiz: {quizError}<br/>";
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"<strong style='color:red'>Exception: {ex.Message}</strong><br/>";
                if (ex.InnerException != null)
                    debugInfo += $"Inner: {ex.InnerException.Message}<br/>";

                TempData["Error"] = "Lỗi kết nối đến server. Vui lòng kiểm tra API đang chạy.";
            }

            if (khoaHoc == null)
            {
                TempData["Error"] = "Không tải được thông tin khóa học.";
                ViewBag.DebugInfo = debugInfo;
                return RedirectToAction("Index");
            }

            ViewBag.Quizzes = quizzes;
            ViewBag.DebugInfo = debugInfo; // Tạm thời để xem lỗi thật sự là gì

            return View(khoaHoc);
        }
    }

        // DTOs - Đặt trong cùng file cho tiện (hoặc chuyển ra Models nếu muốn)
        public class KhoaHocDto
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string MoTa { get; set; }
        public string AnhBia { get; set; }
        public string TrangThaiDuyet { get; set; }
    }

    public class QuizDto
    {
        public int MaQuiz { get; set; }
        public string TenQuiz { get; set; }
        public string MoTa { get; set; }
        public int? ThoiGianLamBai { get; set; }
    }
}
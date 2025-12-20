// MVC_Student/Controllers/QuizController.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using MVC_STUDENT.Models; // Namespace của project Student

namespace MVC_Student.Controllers
{
    public class QuizController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7068/api/Quiz"; // Cổng API của bạn

        public QuizController()
        {
            _httpClient = new HttpClient();
        }

        // GET: /Quiz - Danh sách quiz học sinh có thể làm
        // GET: /Quiz - Danh sách quiz cho Student
        public async Task<ActionResult> Index()
        {
            List<QuizDto> quizzes = new List<QuizDto>();
            string debugInfo = "";

            try
            {
                debugInfo += "Bắt đầu gọi API: " + _apiUrl + "/published<br/>";

                var response = await _httpClient.GetAsync(_apiUrl + "/published");

                debugInfo += "Status Code: " + response.StatusCode + "<br/>";

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    debugInfo += "JSON nhận được: " + json + "<br/>";

                    if (string.IsNullOrWhiteSpace(json) || json == "[]")
                    {
                        debugInfo += "Danh sách rỗng (chưa có quiz Published)<br/>";
                    }
                    else
                    {
                        quizzes = JsonConvert.DeserializeObject<List<QuizDto>>(json);
                        debugInfo += "Parse thành công: " + quizzes.Count + " quiz<br/>";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    debugInfo += "LỖI API: " + errorContent + "<br/>";
                }
            }
            catch (Exception ex)
            {
                debugInfo += "<strong style='color:red'>EXCEPTION: " + ex.Message + "</strong><br/>";
                if (ex.InnerException != null)
                    debugInfo += "Inner: " + ex.InnerException.Message + "<br/>";
            }

            // Gửi debug info ra View để hiển thị (tạm thời)
            ViewBag.DebugInfo = debugInfo;

            return View(quizzes);
        }

        // GET: /Quiz/Take/5 - Làm bài quiz
        public async Task<ActionResult> Take(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không tìm thấy quiz hoặc quiz chưa được phát hành.";
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var quiz = JsonConvert.DeserializeObject<QuizTakeDto>(json);

            if (quiz.TrangThai != "Published")
            {
                TempData["Error"] = "Quiz này chưa mở hoặc đã đóng.";
                return RedirectToAction("Index");
            }

            // Gắn thời gian bắt đầu
            ViewBag.StartTime = DateTime.Now;
            ViewBag.TimeLimit = quiz.ThoiGianLamBai ?? 30; // phút

            return View(quiz);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(QuizSubmitDto submit)
        {
            try
            {
                submit.MaHocSinh = GetCurrentStudentId();
                submit.ThoiGianKetThuc = DateTime.Now;

                var json = JsonConvert.SerializeObject(submit);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiUrl + "/submit", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<QuizResultDto>(resultJson);

                    TempData["Success"] = "Nộp bài thành công!";
                    return View("Result", result); // Dùng dữ liệu thật
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = "Lỗi khi chấm bài: " + error;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối: " + ex.Message;
            }

            // Nếu lỗi, quay lại trang làm bài
            var quizResponse = await _httpClient.GetAsync($"{_apiUrl}/{submit.MaQuiz}");
            if (quizResponse.IsSuccessStatusCode)
            {
                var quizJson = await quizResponse.Content.ReadAsStringAsync();
                var quiz = JsonConvert.DeserializeObject<QuizTakeDto>(quizJson);
                ViewBag.StartTime = submit.ThoiGianBatDau;
                ViewBag.TimeLimit = quiz?.ThoiGianLamBai ?? 30;
                return View("Take", quiz);
            }

            return RedirectToAction("Index");
        }

        private int GetCurrentStudentId()
        {
            // Thay bằng logic thực tế của bạn
            return Session["MaNguoiDung"] != null ? (int)Session["MaNguoiDung"] : 3; // Test với HS ID = 3
        }
    }
}
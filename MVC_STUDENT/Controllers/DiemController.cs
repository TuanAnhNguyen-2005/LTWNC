// MVC_STUDENT/Controllers/DiemController.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using MVC_STUDENT.Models; // Namespace của project Student

namespace MVC_STUDENT.Controllers
{
    public class DiemController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7068/api/Quiz";

        public DiemController()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        private bool IsStudentLoggedIn()
        {
            return Session["UserId"] != null && Session["Role"]?.ToString() == "Student";
        }

        private int GetMaHocSinh()
        {
            if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int id))
                return id;
            return 0;
        }

        // GET: /Diem - Danh sách điểm
        public async Task<ActionResult> Index()
        {
            string debugInfo = "<strong>🔍 DEBUG ĐIỂM THI (XÓA SAU)</strong><br/>";

            if (!IsStudentLoggedIn())
            {
                debugInfo += "❌ Chưa đăng nhập Student<br/>";
                ViewBag.DebugInfo = debugInfo;
                TempData["Error"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Index", "Login");
            }

            int maHocSinh = GetMaHocSinh();
            debugInfo += $"<strong>MaHocSinh:</strong> {maHocSinh}<br/>";
            debugInfo += $"<strong>Session UserId:</strong> {Session["UserId"]}<br/>";

            if (maHocSinh <= 0)
            {
                debugInfo += "<span style='color:red'>❌ LỖI: MaHocSinh = 0 → Kiểm tra LoginController Session!</span><br/>";
                ViewBag.DebugInfo = debugInfo;
                return View(new List<QuizDiemDto>());
            }

            List<QuizDiemDto> diemList = new List<QuizDiemDto>();
            try
            {
                string url = $"{_apiBaseUrl}/diem/{maHocSinh}";
                debugInfo += $"<strong>API URL:</strong> {url}<br/>";

                var response = await _httpClient.GetAsync(url);
                debugInfo += $"<strong>Status:</strong> {response.StatusCode}<br/>";

                string jsonResponse = await response.Content.ReadAsStringAsync();
                debugInfo += $"<strong>JSON:</strong> {jsonResponse?.Substring(0, Math.Min(200, jsonResponse?.Length ?? 0))}...<br/>";

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        diemList = JsonConvert.DeserializeObject<List<QuizDiemDto>>(jsonResponse) ?? new List<QuizDiemDto>();
                        debugInfo += $"<span style='color:green'>✅ Parse OK: {diemList.Count} kết quả</span><br/>";
                    }
                    else
                    {
                        debugInfo += "<span style='color:orange'>⚠️ JSON rỗng → Chưa có điểm nào</span><br/>";
                    }
                }
                else
                {
                    debugInfo += $"<span style='color:red'>❌ LỖI API {response.StatusCode}: {jsonResponse}</span><br/>";
                }
            }
            catch (Exception ex)
            {
                debugInfo += $"<span style='color:red'>💥 EXCEPTION: {ex.Message}</span><br/>";
                debugInfo += $"Stack: {ex.StackTrace?.Substring(0, 100)}...<br/>";
            }

            ViewBag.DebugInfo = debugInfo;
            return View(diemList);
        }

        // GET: /Diem/ChiTiet/5
        // GET: /Diem/ChiTiet?maKetQua=5
        public async Task<ActionResult> ChiTiet(string maKetQua)
        {
            if (!IsStudentLoggedIn()) return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(maKetQua) || !int.TryParse(maKetQua.Trim(), out int mkq) || mkq <= 0)
            {
                TempData["Error"] = "Tham số maKetQua không hợp lệ.";
                return RedirectToAction("Index");
            }

            string url = $"{_apiBaseUrl}/diem/chitiet/{mkq}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var chiTiet = JsonConvert.DeserializeObject<QuizDiemChiTietDto>(json);
                    return View(chiTiet);
                }

                TempData["Error"] = $"Lỗi API: {response.StatusCode}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }

}
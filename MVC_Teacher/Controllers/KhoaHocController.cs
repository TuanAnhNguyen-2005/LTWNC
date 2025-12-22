using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using MVC_Teacher.Models;
using System.Net.Sockets;

namespace MVC_TEACHER.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly string _apiBase =
            System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];

        // GET: /KhoaHoc/Create
        public ActionResult Create()
        {
            // Nếu API không cấu hình, vẫn cho phép mở form tạo
            if (string.IsNullOrEmpty(_apiBase))
            {
                ViewBag.Warning = "Cảnh báo: Chưa cấu hình ApiBaseUrl trong Web.config. Tạo khóa học sẽ không lưu được.";
            }

            return View();
        }

        // POST: /KhoaHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateKhoaHocVm model, HttpPostedFileBase anhBiaFile)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(_apiBase))
            {
                ViewBag.Error = "Không thể tạo khóa học: ApiBaseUrl chưa được cấu hình trong Web.config.";
                return View(model);
            }

            model.MaGiaoVien = GetTeacherId();

            var url = _apiBase.TrimEnd('/') + "/Courses";

            try
            {
                using (var http = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(model.TenKhoaHoc ?? ""), "TenKhoaHoc");
                        content.Add(new StringContent(model.Slug ?? ""), "Slug");
                        content.Add(new StringContent(model.MoTa ?? ""), "MoTa");
                        content.Add(new StringContent(model.MaGiaoVien.ToString()), "MaGiaoVien");

                        if (anhBiaFile != null && anhBiaFile.ContentLength > 0)
                        {
                            var fileContent = new StreamContent(anhBiaFile.InputStream);
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(anhBiaFile.ContentType);
                            content.Add(fileContent, "anhBiaFile", anhBiaFile.FileName);
                        }

                        var resp = await http.PostAsync(url, content);
                        var apiText = await resp.Content.ReadAsStringAsync();

                        if (!resp.IsSuccessStatusCode)
                        {
                            ViewBag.Error = $"Lỗi từ API ({(int)resp.StatusCode}): {apiText}";
                            return View(model);
                        }
                    }
                }

                TempData["msg"] = "✅ Đã tạo khóa học (Draft) thành công!";
                return RedirectToAction("MyCourses");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = "Không thể kết nối đến server API. Vui lòng kiểm tra backend đang chạy chưa (port 5015).<br/>Chi tiết: " + ex.Message;
                return View(model);
            }
            catch (SocketException ex)
            {
                ViewBag.Error = "Không thể kết nối đến API (target machine actively refused).<br/>Hãy chạy project API backend trước.<br/>Chi tiết: " + ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi không xác định khi gọi API: " + ex.Message;
                return View(model);
            }
        }

        // GET: /KhoaHoc/MyCourses
        public async Task<ActionResult> MyCourses()
        {
            if (string.IsNullOrEmpty(_apiBase))
            {
                ViewBag.Warning = "ApiBaseUrl chưa cấu hình. Danh sách khóa học không thể tải.";
                return View(new KhoaHocListVm[0]);
            }

            int teacherId = GetTeacherId();
            var url = _apiBase.TrimEnd('/') + $"/Courses/teacher/{teacherId}";

            try
            {
                using (var http = new HttpClient())
                {
                    var json = await http.GetStringAsync(url);
                    var data = JsonConvert.DeserializeObject<KhoaHocListVm[]>(json);
                    return View(data);
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Không thể tải danh sách khóa học (API không phản hồi).";
                return View(new KhoaHocListVm[0]);
            }
        }

        // GET: /KhoaHoc (danh sách của giáo viên hiện tại)
        public async Task<ActionResult> Index()
        {
            return await MyCourses();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteMultiple(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                TempData["error"] = "Không có khóa học nào được chọn";
                return RedirectToAction("MyCourses");
            }

            if (string.IsNullOrEmpty(_apiBase))
            {
                TempData["error"] = "Không thể xóa: API chưa được cấu hình";
                return RedirectToAction("MyCourses");
            }

            int teacherId = GetTeacherId();
            var url = _apiBase.TrimEnd('/') + $"/Courses/batch?teacherId={teacherId}";

            try
            {
                using (var http = new HttpClient())
                {
                    var jsonContent = JsonConvert.SerializeObject(new { Ids = ids });
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage(HttpMethod.Delete, url)
                    {
                        Content = content
                    };

                    var response = await http.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["msg"] = $"🗑️ Đã xóa thành công {ids.Count} khóa học!";
                    }
                    else
                    {
                        TempData["error"] = "⚠️ Không thể xóa một số khóa học (có thể đã được duyệt)";
                    }
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Không thể kết nối đến API để xóa khóa học.";
            }

            return RedirectToAction("MyCourses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(int id)
        {
            if (string.IsNullOrEmpty(_apiBase))
            {
                TempData["error"] = "Không thể gửi duyệt: API chưa được cấu hình";
                return RedirectToAction("MyCourses");
            }

            int teacherId = GetTeacherId();
            var url = _apiBase.TrimEnd('/') + $"/Courses/{id}/submit?teacherId={teacherId}";

            try
            {
                using (var http = new HttpClient())
                {
                    await http.PutAsync(url, new StringContent(""));
                }

                TempData["msg"] = "📨 Đã gửi chờ Admin duyệt";
            }
            catch (Exception)
            {
                TempData["error"] = "Không thể gửi yêu cầu duyệt (API không phản hồi)";
            }

            return RedirectToAction("MyCourses");
        }

        // Demo – sau này lấy từ Session hoặc Identity
        private int GetTeacherId()
        {
            return 1; // Giả lập giáo viên ID = 1
        }
    }

    // =======================
    // VIEW MODEL
    // =======================

    public class CreateKhoaHocVm
    {
        public string TenKhoaHoc { get; set; }
        public string Slug { get; set; }
        public string MoTa { get; set; }
        public int MaGiaoVien { get; set; }
    }

    public class KhoaHocListVm
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string TrangThaiDuyet { get; set; }
        public string AnhBia { get; set; }
        public DateTime NgayTao { get; set; }
        public List<QuizListItemVm> Quizzes { get; set; } = new List<QuizListItemVm>();
    }
}
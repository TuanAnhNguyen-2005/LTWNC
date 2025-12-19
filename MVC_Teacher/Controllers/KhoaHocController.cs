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

namespace MVC_TEACHER.Controllers
{
    public class KhoaHocController : Controller
    {
        private readonly string _apiBase =
            System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];

        // GET: /KhoaHoc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /KhoaHoc/Create
        [HttpPost]
        public async Task<ActionResult> Create(CreateKhoaHocVm model, HttpPostedFileBase anhBiaFile)  // 🔴 Dùng HttpPostedFileBase
        {
            if (model == null)
                throw new Exception("Model is null");

            if (string.IsNullOrEmpty(_apiBase))
                throw new Exception("ApiBaseUrl chưa được cấu hình trong Web.config");

            model.MaGiaoVien = GetTeacherId();

            var url = _apiBase.TrimEnd('/') + "/Courses";

            using (var http = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    // Thêm các field text
                    content.Add(new StringContent(model.TenKhoaHoc ?? ""), "TenKhoaHoc");
                    content.Add(new StringContent(model.Slug ?? ""), "Slug");
                    content.Add(new StringContent(model.MoTa ?? ""), "MoTa");
                    content.Add(new StringContent(model.MaGiaoVien.ToString()), "MaGiaoVien");

                    // 🔴 XỬ LÝ FILE ẢNH (nếu có)
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
                        ViewBag.Error =
                            $"API lỗi {(int)resp.StatusCode}\n" +
                            $"URL: {url}\n" +
                            $"Chi tiết: {apiText}";

                        return View(model);
                    }
                }
            }

            TempData["msg"] = "✅ Đã tạo khóa học (Draft)";
            return RedirectToAction("MyCourses");
        }
        // GET: /KhoaHoc/MyCourses
        public async Task<ActionResult> MyCourses()
        {
            int teacherId = GetTeacherId();
            var url = _apiBase.TrimEnd('/') + $"/Courses/teacher/{teacherId}";

            using (var http = new HttpClient())
            {
                var json = await http.GetStringAsync(url);
                var data = JsonConvert.DeserializeObject<KhoaHocListVm[]>(json);
                return View(data);
            }
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

            int teacherId = GetTeacherId();
            var url = _apiBase.TrimEnd('/') + $"/Courses/batch?teacherId={teacherId}";

            using (var http = new HttpClient())
            {
                // Tạo JSON body
                var jsonContent = JsonConvert.SerializeObject(new { Ids = ids });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Tạo request thủ công với method DELETE và body
                var request = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = content
                };

                try
                {
                    var response = await http.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        // Có thể parse để lấy số lượng xóa nếu muốn
                        TempData["msg"] = $"🗑️ Đã xóa thành công {ids.Count} khóa học!";
                    }
                    else
                    {
                        var errorText = await response.Content.ReadAsStringAsync();
                        TempData["error"] = "⚠️ Không thể xóa một số khóa học (có thể đã được duyệt hoặc đang chờ duyệt)";
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = "Lỗi kết nối đến server: " + ex.Message;
                }
            }

            return RedirectToAction("MyCourses");
        }

        // POST: /KhoaHoc/Submit
        [HttpPost]
        public async Task<ActionResult> Submit(int id)
        {
            int teacherId = GetTeacherId();
            var url = _apiBase.TrimEnd('/') + $"/Courses/{id}/submit?teacherId={teacherId}";

            using (var http = new HttpClient())
            {
                await http.PutAsync(url, new StringContent(""));
            }

            TempData["msg"] = "📨 Đã gửi chờ Admin duyệt";
            return RedirectToAction("MyCourses");
        }

        // Demo – sau này bạn lấy từ đăng nhập
        private int GetTeacherId()
        {
            return 1;
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
    }
}

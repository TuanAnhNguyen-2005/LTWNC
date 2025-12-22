using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_Teacher.Models;
using Newtonsoft.Json;

namespace MVC_Teacher.Controllers
{
    public class UploadFileController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "https://localhost:7068/api/"; // Thay port nếu khác

        public UploadFileController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(ApiBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: UploadFile/Upload?maKhoaHoc=5
        public async Task<ActionResult> Upload(int? maKhoaHoc)
        {
            if (!maKhoaHoc.HasValue)
            {
                TempData["error"] = "Không xác định được khóa học để tải tài liệu.";
                return RedirectToAction("MyCourses", "KhoaHoc");
            }

            int teacherId = GetCurrentTeacherId();

            try
            {
                var courseResponse = await _httpClient.GetAsync($"Courses/{maKhoaHoc.Value}");
                if (!courseResponse.IsSuccessStatusCode)
                {
                    TempData["error"] = $"Không tìm thấy khóa học (API lỗi {courseResponse.StatusCode}).";
                    return RedirectToAction("MyCourses", "KhoaHoc");
                }

                var courseJson = await courseResponse.Content.ReadAsStringAsync();

                // DEBUG: Hiển thị JSON trả về để biết chính xác API trả gì
                ViewBag.DebugApiResponse = courseJson;

                var courseObj = JsonConvert.DeserializeObject<dynamic>(courseJson);

                // Log chi tiết để debug
                int? apiMaGiaoVien = courseObj?.MaGiaoVien;
                string apiTrangThai = courseObj?.TrangThaiDuyet?.ToString();
                string apiTenKhoaHoc = courseObj?.tenKhoaHoc?.ToString();

                ViewBag.DebugInfo = $"teacherId hiện tại: {teacherId}<br>" +
                                    $"API MaGiaoVien: {apiMaGiaoVien}<br>" +
                                    $"API TrangThaiDuyet: {apiTrangThai}<br>" +
                                    $"API TenKhoaHoc: {apiTenKhoaHoc}";

                //if (apiMaGiaoVien == null || apiTrangThai != "DaDuyet" || apiMaGiaoVien != teacherId)
                //{
                //    TempData["error"] = "Bạn không có quyền tải tài liệu lên khóa học này hoặc khóa học chưa được duyệt.";
                //    return RedirectToAction("MyCourses", "KhoaHoc");
                //}

                ViewBag.TenKhoaHoc = apiTenKhoaHoc ?? "Khóa học không tên";
                ViewBag.MaKhoaHoc = maKhoaHoc.Value;

                // Lấy danh sách tài liệu (giữ nguyên)
                var hocLieuResponse = await _httpClient.GetAsync($"HocLieu/course/{maKhoaHoc.Value}");
                List<HocLieuViewModel> danhSach = new List<HocLieuViewModel>();
                string hocLieuJson = "";
                if (hocLieuResponse.IsSuccessStatusCode)
                {
                    hocLieuJson = await hocLieuResponse.Content.ReadAsStringAsync();
                    danhSach = JsonConvert.DeserializeObject<List<HocLieuViewModel>>(hocLieuJson) ?? new List<HocLieuViewModel>();
                }
                else
                {
                    hocLieuJson = $"Error: {hocLieuResponse.StatusCode} - {await hocLieuResponse.Content.ReadAsStringAsync()}";
                }
                ViewBag.DebugHocLieuJson = hocLieuJson;
                ViewBag.DanhSachTaiLieu = danhSach;

                return View(new UploadHocLieuDto { MaKhoaHoc = maKhoaHoc.Value });
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi kết nối API: " + ex.Message + "<br>Stack Trace: " + ex.StackTrace;
                ViewBag.DebugInfo = ex.ToString();
                return RedirectToAction("MyCourses", "KhoaHoc");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(UploadHocLieuDto model)
        {
            // BƯỚC 1: Kiểm tra ModelState từ DataAnnotations ([Required])
            if (!ModelState.IsValid)
            {
                // Nếu có lỗi validation (TieuDe rỗng hoặc File null), load lại view với thông báo lỗi
                await LoadCourseAndMaterials(model.MaKhoaHoc);
                return View(model);
            }

            // BƯỚC 2: Kiểm tra file có thực sự được chọn (phòng trường hợp binding lỗi)
            if (model.FileHocLieu == null || model.FileHocLieu.ContentLength == 0)
            {
                ModelState.AddModelError("FileHocLieu", "Vui lòng chọn file tài liệu.");
                await LoadCourseAndMaterials(model.MaKhoaHoc);
                return View(model);
            }

            // BƯỚC 3: Kiểm tra định dạng và kích thước file
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt", ".zip", ".rar" };
            var extension = Path.GetExtension(model.FileHocLieu.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("FileHocLieu", "Chỉ chấp nhận định dạng: PDF, Word, Excel, PPT, TXT, ZIP, RAR.");
                await LoadCourseAndMaterials(model.MaKhoaHoc);
                return View(model);
            }

            if (model.FileHocLieu.ContentLength > 50 * 1024 * 1024)
            {
                ModelState.AddModelError("FileHocLieu", "File không được lớn hơn 50MB.");
                await LoadCourseAndMaterials(model.MaKhoaHoc);
                return View(model);
            }

            // BƯỚC 4: Gửi lên API
            try
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StringContent(model.TieuDe.Trim()), "TieuDe");

                    if (!string.IsNullOrWhiteSpace(model.MoTa))
                        content.Add(new StringContent(model.MoTa.Trim()), "MoTa");

                    content.Add(new StringContent(model.MaKhoaHoc.ToString()), "MaKhoaHoc");
                    content.Add(new StringContent(GetCurrentTeacherId().ToString()), "MaNguoiDung");

                    var fileContent = new StreamContent(model.FileHocLieu.InputStream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.FileHocLieu.ContentType);
                    content.Add(fileContent, "File", model.FileHocLieu.FileName);

                    var response = await _httpClient.PostAsync("HocLieu/upload", content);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    TempData["debug_response"] = $"Status: {(int)response.StatusCode} | Body: {responseBody}";

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["msg"] = $"Đã upload tài liệu \"{model.TieuDe}\" thành công!";
                        return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
                        // Hoặc: return RedirectToAction("Success", new { maKhoaHoc = model.MaKhoaHoc });
                    }
                    else
                    {
                        TempData["error"] = "Upload thất bại: " + responseBody;
                        return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi hệ thống: " + ex.Message;
                return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
            }
        }

        // Hàm phụ để load tên khóa học + danh sách tài liệu khi có lỗi
        private async Task LoadCourseAndMaterials(int maKhoaHoc)
        {
            try
            {
                var courseResp = await _httpClient.GetAsync($"Courses/{maKhoaHoc}");
                if (courseResp.IsSuccessStatusCode)
                {
                    var json = await courseResp.Content.ReadAsStringAsync();
                    dynamic obj = JsonConvert.DeserializeObject(json);
                    ViewBag.TenKhoaHoc = obj?.tenKhoaHoc?.ToString() ?? $"Khóa học #{maKhoaHoc}";
                }
                else
                {
                    ViewBag.TenKhoaHoc = $"Khóa học #{maKhoaHoc}";
                }
            }
            catch
            {
                ViewBag.TenKhoaHoc = $"Khóa học #{maKhoaHoc}";
            }

            ViewBag.MaKhoaHoc = maKhoaHoc;

            // Load danh sách tài liệu
            var hlResp = await _httpClient.GetAsync($"HocLieu/course/{maKhoaHoc}");
            List<HocLieuViewModel> ds = new List<HocLieuViewModel>();
            if (hlResp.IsSuccessStatusCode)
            {
                var json = await hlResp.Content.ReadAsStringAsync();
                ds = JsonConvert.DeserializeObject<List<HocLieuViewModel>>(json) ?? new List<HocLieuViewModel>();
            }
            ViewBag.DanhSachTaiLieu = ds;
        }

        // Hàm phụ để tránh lặp code khi load view Upload
        private async Task<ActionResult> PrepareUploadView(int maKhoaHoc)
        {
            // Load tên khóa học đúng cách
            try
            {
                var resp = await _httpClient.GetAsync($"Courses/{maKhoaHoc}");
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    dynamic obj = JsonConvert.DeserializeObject(json);
                    ViewBag.TenKhoaHoc = obj?.tenKhoaHoc?.ToString() ?? $"Khóa học #{maKhoaHoc}";
                }
                else
                {
                    ViewBag.TenKhoaHoc = $"Khóa học #{maKhoaHoc}";
                }
            }
            catch
            {
                ViewBag.TenKhoaHoc = $"Khóa học #{maKhoaHoc}";
            }

            ViewBag.MaKhoaHoc = maKhoaHoc;

            // Load danh sách tài liệu
            var hocLieuResp = await _httpClient.GetAsync($"HocLieu/course/{maKhoaHoc}");
            List<HocLieuViewModel> danhSach = new List<HocLieuViewModel>();
            if (hocLieuResp.IsSuccessStatusCode)
            {
                var json = await hocLieuResp.Content.ReadAsStringAsync();
                danhSach = JsonConvert.DeserializeObject<List<HocLieuViewModel>>(json) ?? new List<HocLieuViewModel>();
            }
            ViewBag.DanhSachTaiLieu = danhSach;

            return View("Upload", new UploadHocLieuDto { MaKhoaHoc = maKhoaHoc });
        }

        // Hàm phụ để load tên khóa học khi có lỗi validation (tránh hiển thị "Khóa học #5")
        private async Task LoadCourseInfo(int maKhoaHoc)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Courses/{maKhoaHoc}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic obj = JsonConvert.DeserializeObject(json);
                    ViewBag.TenKhoaHoc = obj?.tenKhoaHoc?.ToString() ?? $"Khóa học #{maKhoaHoc}";
                }
            }
            catch { ViewBag.TenKhoaHoc = $"Khóa học #{maKhoaHoc}"; }
            ViewBag.MaKhoaHoc = maKhoaHoc;
        }

        public async Task<ActionResult> Success(int maKhoaHoc)
        {
            int teacherId = GetCurrentTeacherId();

            try
            {
                var courseResponse = await _httpClient.GetAsync($"Courses/{maKhoaHoc}");
                if (!courseResponse.IsSuccessStatusCode)
                {
                    TempData["error"] = $"Không tìm thấy khóa học (API lỗi {courseResponse.StatusCode}).";
                    return RedirectToAction("MyCourses", "KhoaHoc");
                }

                var courseJson = await courseResponse.Content.ReadAsStringAsync();
                var courseObj = JsonConvert.DeserializeObject<dynamic>(courseJson);
                string apiTenKhoaHoc = courseObj?.tenKhoaHoc?.ToString();

                ViewBag.TenKhoaHoc = apiTenKhoaHoc ?? "Khóa học không tên";

                ViewBag.TenKhoaHoc = apiTenKhoaHoc ?? "Khóa học không tên";
                ViewBag.MaKhoaHoc = maKhoaHoc;
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

                var hocLieuResponse = await _httpClient.GetAsync($"HocLieu/course/{maKhoaHoc}");
                List<HocLieuViewModel> danhSach = new List<HocLieuViewModel>();
                if (hocLieuResponse.IsSuccessStatusCode)
                {
                    var json = await hocLieuResponse.Content.ReadAsStringAsync();
                    danhSach = JsonConvert.DeserializeObject<List<HocLieuViewModel>>(json) ?? new List<HocLieuViewModel>();
                }
                ViewBag.DanhSachTaiLieu = danhSach;

                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi kết nối API: " + ex.Message;
                return RedirectToAction("MyCourses", "KhoaHoc");
            }
        }

        private int GetCurrentTeacherId()
        {
            if (Session["MaNguoiDung"] != null && int.TryParse(Session["MaNguoiDung"].ToString(), out int id))
                return id;
            return 2; // test
        }
    }
}
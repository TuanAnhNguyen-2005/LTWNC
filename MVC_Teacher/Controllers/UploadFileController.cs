using MVC_Teacher.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC_Teacher.Controllers
{
    public class UploadFileController : Controller
    {
        // ✅ API Base (đúng theo bạn đang chạy)
        private readonly string _apiBase = "https://localhost:7068/api/";


        [HttpGet]
        public ActionResult Upload(int maKhoaHoc)
        {
            var keys = string.Join(", ", Session.Keys.Cast<string>());
            ViewBag.SessionKeys = keys;

            ViewBag.MaNguoiDung = Session["MaNguoiDung"];
            ViewBag.UserId = Session["UserId"];
            ViewBag.TeacherId = Session["TeacherId"];
            ViewBag.MaGiaoVien = Session["MaGiaoVien"];

            var model = new UploadHocLieuDto { MaKhoaHoc = maKhoaHoc };
            return View(model);
        }


        // POST: /UploadFile/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(UploadHocLieuDto model)
        {
            // 1) Validate input
            if (model == null)
            {
                TempData["error"] = "Dữ liệu không hợp lệ";
                return RedirectToAction("Upload", new { maKhoaHoc = 0 });
            }

            if (string.IsNullOrWhiteSpace(model.TieuDe))
            {
                TempData["error"] = "Vui lòng nhập tiêu đề tài liệu";
                return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
            }

            if (model.FileHocLieu == null || model.FileHocLieu.ContentLength <= 0)
            {
                TempData["error"] = "Vui lòng chọn file tài liệu";
                return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
            }

            // 2) Validate file extension + size (match API rule)
            var allowedExtensions = new[]
            {
                ".pdf", ".doc", ".docx", ".ppt", ".pptx",
                ".xls", ".xlsx", ".txt", ".zip", ".rar"
            };

            var ext = Path.GetExtension(model.FileHocLieu.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext))
            {
                TempData["error"] = "Định dạng file không được hỗ trợ";
                return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
            }

            long maxBytes = 50L * 1024 * 1024; // 50MB
            if (model.FileHocLieu.ContentLength > maxBytes)
            {
                TempData["error"] = "File không được lớn hơn 50MB";
                return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
            }

            // 3) Lấy MaGiaoVien từ session/login
            int teacherId = GetCurrentTeacherId();
            if (teacherId <= 0)
            {
                TempData["error"] = "Bạn chưa đăng nhập hoặc không xác định được mã giáo viên.";
                return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
            }

            // 4) Call API upload
            try
            {
                using (var http = new HttpClient())
                {
                    http.BaseAddress = new Uri(_apiBase);
                    http.DefaultRequestHeaders.Accept.Clear();
                    http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                    using (var content = new MultipartFormDataContent())
                    {
                        // ✅ tên field PHẢI khớp DTO API
                        content.Add(new StringContent(model.TieuDe.Trim()), "TieuDe");
                        content.Add(new StringContent(model.MoTa ?? ""), "MoTa");
                        content.Add(new StringContent(model.MaKhoaHoc.ToString()), "MaKhoaHoc");

                        // ✅ API mới dùng MaGiaoVien (KHÔNG phải MaNguoiDung)
                        content.Add(new StringContent(teacherId.ToString()), "MaGiaoVien");

                        // ✅ API mới nhận file key là "File"
                        var fileContent = new StreamContent(model.FileHocLieu.InputStream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.FileHocLieu.ContentType);
                        content.Add(fileContent, "File", Path.GetFileName(model.FileHocLieu.FileName));

                        var response = await http.PostAsync("HocLieu/upload", content);
                        var responseBody = await response.Content.ReadAsStringAsync();

                        TempData["debug_response"] = $"Status: {(int)response.StatusCode}\n{responseBody}";

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["msg"] = "Upload tài liệu thành công!";
                        }
                        else
                        {
                            TempData["error"] = "Upload thất bại: " + responseBody;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("Upload", new { maKhoaHoc = model.MaKhoaHoc });
        }

        // ✅ Lấy ID giáo viên đang đăng nhập
        // Bạn chỉnh đúng theo session key dự án bạn đang dùng
        private int GetCurrentTeacherId()
        {
            if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int id))
                return id;

            return 0;
        }

    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace MVC_ADMIN.Controllers
{
    [RouteArea("Admin")]
    [RoutePrefix("DuyetKhoaHoc")]
    public class DuyetKhoaHocController : Controller
    {
        private readonly string _apiBase =
            System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];

        // GET: /DuyetKhoaHoc
        [Route("")]
        [Route("Index")]
        public async Task<ActionResult> Index()
        {
            var url = _apiBase.TrimEnd('/') + "/Courses/pending";

            using (var http = new HttpClient())
            {
                var resp = await http.GetAsync(url);
                var json = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    ViewBag.ApiError = $"API lỗi {(int)resp.StatusCode}\n{url}";
                    return View(new List<KhoaHocPendingVm>());
                }

                var data = JsonConvert.DeserializeObject<List<KhoaHocPendingVm>>(json)
                           ?? new List<KhoaHocPendingVm>();

                return View(data);
            }
        }
        // Action Approve - SỬA THÀNH GIỐNG INDEX
        [HttpPost]
        public async Task<ActionResult> Approve(int id)
        {
            int nguoiDuyetId = GetNguoiDuyetId();

            // ✅ Giữ nguyên cách xây URL giống Index (đang hoạt động)
            var url = _apiBase.TrimEnd('/') + $"/Courses/{id}/approve?nguoiDuyetId={nguoiDuyetId}";

            using (var http = new HttpClient())
            {
                var res = await http.PutAsync(url, new StringContent("", Encoding.UTF8, "application/json"));

                if (!res.IsSuccessStatusCode)
                {
                    var error = await res.Content.ReadAsStringAsync();
                    TempData["msg"] = $"❌ Duyệt thất bại: {(int)res.StatusCode} - {error}";
                }
                else
                {
                    TempData["msg"] = "✅ Đã duyệt thành công";
                }
            }

            return RedirectToAction("Index");
        }

        // Action Reject - SỬA TƯƠNG TỰ
        [HttpPost]
        public async Task<ActionResult> Reject(int id, string lyDoTuChoi)
        {
            int nguoiDuyetId = GetNguoiDuyetId();

            // ✅ Giống hệt cách của Index
            var url = _apiBase.TrimEnd('/') + $"/Courses/{id}/reject?nguoiDuyetId={nguoiDuyetId}";

            var bodyObj = new { lyDoTuChoi = lyDoTuChoi ?? "" };
            var bodyJson = JsonConvert.SerializeObject(bodyObj);

            using (var http = new HttpClient())
            {
                var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
                var res = await http.PutAsync(url, content);

                if (!res.IsSuccessStatusCode)
                {
                    var error = await res.Content.ReadAsStringAsync();
                    TempData["msg"] = $"❌ Từ chối thất bại: {(int)res.StatusCode} - {error}";
                }
                else
                {
                    TempData["msg"] = "✅ Đã từ chối khóa học";
                }
            }

            return RedirectToAction("Index");
        }

        // Tạm thời hard-code để chạy demo.
        // Khi bạn có login admin rồi thì lấy từ Session/UserClaims.
        private int GetNguoiDuyetId()
        {
            return 1;
        }
    }

    // ViewModel nhận từ API /pending
    public class KhoaHocPendingVm
    {
        public int MaKhoaHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string Slug { get; set; }
        public string MoTa { get; set; }
        public int MaGiaoVien { get; set; }
        public string TrangThaiDuyet { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayGuiDuyet { get; set; }
    }
}

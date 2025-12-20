using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_STUDENT.Models;

namespace MVC_STUDENT.Controllers
{
    public class SignUpController : Controller
    {
        private readonly string _apiBase =
            System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        // POST /SignUp  (KHỚP VIEW CỦA BẠN)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(
            string fullName,
            string email,
            string password,
            string confirmPassword,
            string role)
        {
            // Validate cơ bản
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ họ tên, email và mật khẩu.";
                return RedirectToAction("Index");
            }

            if (password != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("Index");
            }

            // Map role -> MaVaiTro (SỬA ĐÚNG THEO DB)
            int maVaiTro = role == "Teacher" ? 2 : 3;

            var payload = new CreateNguoiDungRequest
            {
                HoTen = fullName.Trim(),
                Email = email.Trim(),
                MatKhau = password,
                MaVaiTro = maVaiTro,
                IsLocked = false
            };

            var url = _apiBase.TrimEnd('/') + "/NguoiDung";

            using (var http = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage res;
                try
                {
                    res = await http.PostAsync(url, content);
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Không gọi được API: " + ex.Message;
                    return RedirectToAction("Index");
                }

                if (!res.IsSuccessStatusCode)
                {
                    var body = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = "Đăng ký thất bại: " + body;
                    return RedirectToAction("Index");
                }
            }

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Index", "Login");
        }
    }
}

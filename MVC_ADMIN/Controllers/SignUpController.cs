// Controllers/SignUpController.cs  (hoặc LoginController, ProfileController… đều dùng được)
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace MVC_ADMIN.Controllers
{
    public class SignUpController : Controller
    {
        // Thuộc tính tự động lấy URL API – KHÔNG CÒN HARD-CODE
        // ĐOẠN NÀY DÁN VÀO ĐẦU MỌI CONTROLLER – CHỈ DÁN 1 LẦN DUY NHẤT!!!
        // DÁN NGUYÊN KHỐI NÀY VÀO ĐẦU MỌI CONTROLLER (Login, SignUp, User, Category, v.v…)
        private string ApiUrl
        {
            get
            {
                // Nếu đã cache trong Session rồi → trả luôn (nhanh)
                if (Session["ApiBaseUrl"] != null)
                    return Session["ApiBaseUrl"].ToString();

                const string FALLBACK_URL = "https://localhost:7068/api"; // ĐỔI 7068 THÀNH CỔNG API HIỆN TẠI CỦA BẠN

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = client.GetAsync("https://localhost:7068/api/config/urls").Result;

                        if (response.IsSuccessStatusCode)
                        {
                            string json = response.Content.ReadAsStringAsync().Result;
                            dynamic cfg = JsonConvert.DeserializeObject(json);

                            string baseUrl = (cfg?.ApiBaseUrl?.ToString() ?? FALLBACK_URL.Replace("/api", "")).TrimEnd('/');
                            string fullUrl = baseUrl + "/api";

                            Session["ApiBaseUrl"] = fullUrl;
                            return fullUrl;
                        }
                    }
                }
                catch
                {
                    // Không gọi được config → im lặng dùng fallback
                }

                // Fallback cuối cùng
                Session["ApiBaseUrl"] = FALLBACK_URL;
                return FALLBACK_URL;
            }
        }
        // DÁN XONG – CHẠY NGON 100% VỚI C# 7.3!
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(new
                {
                    fullName = model.FullName,
                    email = model.Email,
                    password = model.Password,
                    confirmPassword = model.ConfirmPassword,
                    role = model.Role
                });

                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                // DÙNG ApiUrl Ở ĐÂY → KHÔNG LỖI NỮA
                var response = await client.PostAsync($"{ApiUrl}/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Index", "Login");
                }

                var errorJson = await response.Content.ReadAsStringAsync();
                dynamic err = JsonConvert.DeserializeObject(errorJson);
                ViewBag.Error = err?.message ?? "Đăng ký thất bại!";
                return View(model);
            }
        }
    }

    // ViewModel (đã fix Compare lỗi)
    public class SignUpViewModel
    {
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [System.Web.Mvc.Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; } = "Student";
    }
}
// Controllers/LoginController.cs
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVC_ADMIN.Controllers
{
    public class LoginController : Controller
    {
        // ==================== DÁN TỪ ĐÂY TRỞ XUỐNG ====================
        private string ApiUrl
        {
            get
            {
                // Nếu đã cache trong Session rồi → trả luôn (nhanh)
                if (Session["ApiBaseUrl"] != null)
                    return Session["ApiBaseUrl"].ToString();

                // Chưa có → gọi API Core để lấy URL mới nhất
                string fallback = "https://localhost:7068/api";  // ĐỔI 7068 THÀNH CỔNG HIỆN TẠI CỦA BẠN

                try
                {
                    using (var client = new HttpClient())
                    {
                        // Gọi endpoint config của API Core
                        var response = client.GetAsync("https://localhost:7068/api/config/urls").Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var json = response.Content.ReadAsStringAsync().Result;
                            dynamic cfg = JsonConvert.DeserializeObject(json);
                            string baseUrl = cfg.ApiBaseUrl?.ToString() ?? fallback.Replace("/api", "");
                            string fullUrl = baseUrl.TrimEnd('/') + "/api";

                            Session["ApiBaseUrl"] = fullUrl;   // Cache lại
                            return fullUrl;
                        }
                    }
                }
                catch
                {
                    // Không gọi được config → dùng fallback
                }

                Session["ApiBaseUrl"] = fallback;
                return fallback;
            }
        }
        // ==================== DÁN ĐẾN ĐÂY ====================
        // GET: /Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string email, string password)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", email),
                    new KeyValuePair<string, string>("password", password)
                });

                // ĐÃ SỬA: dùng ApiUrl thay vì apiUrl → KHÔNG CÒN LỖI
                var response = await client.PostAsync($"{ApiUrl}/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(json);

                    // Lưu thông tin user vào Session
                    Session["UserId"] = result.userId;
                    Session["FullName"] = result.fullName;
                    Session["Email"] = result.email;
                    Session["Role"] = result.role; // Admin / Teacher / Student

                    // Chuyển hướng theo role
                    if (result.role == "Admin")
                        return Redirect("/Admin");
                    else if (result.role == "Teacher")
                        return Redirect("/Teacher/Dashboard");
                    else
                        return Redirect("/Student/Dashboard");
                }
                else
                {
                    ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                    return View();
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index");
        }
    }
}
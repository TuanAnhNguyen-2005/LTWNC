using System;
using System.Web;
using System.Web.Mvc;
using MVC_ADMIN.Helpers;
using MVC_ADMIN.Services;

namespace MVC_ADMIN.Controllers
{
    /// <summary>
    /// Base Controller với các method chung cho tất cả controllers
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected ApiService ApiService => ApiServiceHelper.Instance;

        /// <summary>
        /// Lấy API Base URL từ config hoặc cache
        /// </summary>
        protected string GetApiBaseUrl()
        {
            // Kiểm tra cache trong Session (chỉ khi Session đã sẵn sàng)
            if (Session != null && Session["ApiBaseUrl"] != null)
                return Session["ApiBaseUrl"].ToString();

            // Lấy từ Web.config
            string fallbackUrl = ConfigurationHelper.GetAppSetting("ApiBaseUrl", "https://localhost:7264/api");

            try
            {
                // Thử lấy từ API config endpoint
                using (var client = new System.Net.Http.HttpClient())
                {
                    var configUrl = ConfigurationHelper.GetAppSetting("ApiConfigUrl", "https://localhost:7264/api/config/urls");
                    var response = client.GetAsync(configUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        dynamic cfg = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        string baseUrl = cfg?.ApiBaseUrl?.ToString() ?? fallbackUrl.Replace("/api", "");
                        string fullUrl = baseUrl.TrimEnd('/') + "/api";
                        
                        // Cache vào Session nếu có
                        if (Session != null)
                            Session["ApiBaseUrl"] = fullUrl;
                            
                        return fullUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                System.Diagnostics.Debug.WriteLine($"Error getting API URL: {ex.Message}");
            }

            // Fallback - cache vào Session nếu có
            if (Session != null)
                Session["ApiBaseUrl"] = fallbackUrl;
                
            return fallbackUrl;
        }

        /// <summary>
        /// Kiểm tra user đã đăng nhập chưa
        /// </summary>
        protected bool IsAuthenticated()
        {
            return Session["UserId"] != null && Session["Email"] != null;
        }

        /// <summary>
        /// Kiểm tra user có role cụ thể không
        /// </summary>
        protected bool HasRole(string role)
        {
            return Session["Role"]?.ToString() == role;
        }

        /// <summary>
        /// Lấy UserId từ Session
        /// </summary>
        protected int? GetUserId()
        {
            if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int userId))
                return userId;
            return null;
        }

        /// <summary>
        /// Lấy Email từ Session
        /// </summary>
        protected string GetUserEmail()
        {
            return Session["Email"]?.ToString();
        }

        /// <summary>
        /// Lấy FullName từ Session
        /// </summary>
        protected string GetUserFullName()
        {
            return Session["FullName"]?.ToString();
        }

        /// <summary>
        /// Lấy Role từ Session
        /// </summary>
        protected string GetUserRole()
        {
            return Session["Role"]?.ToString();
        }

        /// <summary>
        /// Redirect về login nếu chưa đăng nhập
        /// </summary>
        protected ActionResult RedirectToLogin()
        {
            return RedirectToAction("Index", "Login");
        }

        /// <summary>
        /// Redirect về trang chủ theo role
        /// </summary>
        protected ActionResult RedirectToHomeByRole()
        {
            string role = GetUserRole();
            if (role == "Admin")
            {
                // Admin redirect đến Statistical/Index
                return RedirectToAction("Index", "Statistical");
            }
            else
            {
                // Teacher và Student redirect đến Home/Index
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Xử lý exception và trả về error message
        /// </summary>
        protected void HandleException(Exception ex, string customMessage = null)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            if (ex.InnerException != null)
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");

            TempData["Error"] = customMessage ?? "Đã xảy ra lỗi. Vui lòng thử lại sau.";
        }

        /// <summary>
        /// Set success message
        /// </summary>
        protected void SetSuccessMessage(string message)
        {
            TempData["Success"] = message;
        }

        /// <summary>
        /// Set error message
        /// </summary>
        protected void SetErrorMessage(string message)
        {
            TempData["Error"] = message;
        }
    }
}


using System;
using MVC_ADMIN.Helpers;

namespace MVC_ADMIN.Services
{
    public static class ApiServiceHelper
    {
        private static readonly Lazy<ApiService> _instance = new Lazy<ApiService>(() =>
        {
            // Lấy URL từ Web.config, fallback về URL mặc định
            string apiUrl = ConfigurationHelper.GetAppSetting("ApiBaseUrl", "https://localhost:7068/api");
            System.Diagnostics.Debug.WriteLine($"ApiServiceHelper: Using API URL: {apiUrl}");
            // Đảm bảo có /api ở cuối nếu chưa có
            if (!apiUrl.EndsWith("/api"))
            {
                apiUrl = apiUrl.TrimEnd('/') + "/api";
            }
            return new ApiService(apiUrl);
        });

        public static ApiService Instance => _instance.Value;
    }
}
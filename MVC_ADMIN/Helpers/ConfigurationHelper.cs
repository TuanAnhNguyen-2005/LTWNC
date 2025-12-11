using System;
using System.Configuration;

namespace MVC_ADMIN.Helpers
{
    /// <summary>
    /// Helper class để đọc configuration từ Web.config
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Lấy giá trị từ appSettings trong Web.config
        /// </summary>
        /// <param name="key">Key cần lấy</param>
        /// <param name="defaultValue">Giá trị mặc định nếu không tìm thấy</param>
        /// <returns>Giá trị từ config hoặc defaultValue</returns>
        public static string GetAppSetting(string key, string defaultValue = null)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Lấy connection string từ Web.config
        /// </summary>
        /// <param name="name">Tên connection string</param>
        /// <returns>Connection string hoặc null</returns>
        public static string GetConnectionString(string name = "DefaultConnection")
        {
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings[name];
                return connectionString?.ConnectionString;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Kiểm tra xem có đang ở chế độ debug không
        /// </summary>
        public static bool IsDebugMode()
        {
            try
            {
                var compilation = ConfigurationManager.GetSection("system.web/compilation") as System.Web.Configuration.CompilationSection;
                return compilation?.Debug ?? false;
            }
            catch
            {
                return false;
            }
        }
    }
}


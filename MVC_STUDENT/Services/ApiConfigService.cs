using System;
using System.IO;
using Newtonsoft.Json;

namespace MVC_STUDENT.Services
{
    public class ApiConfigService
    {
        public string ApiBaseUrl { get; }

        public ApiConfigService()
        {
            try
            {
                // Giống y hệt cách bạn đang đọc connection string
                string filePath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..",
                    "appsettings.json"
                );

                filePath = Path.GetFullPath(filePath);

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Không tìm thấy appsettings.json tại {filePath}");

                var json = File.ReadAllText(filePath);
                dynamic config = JsonConvert.DeserializeObject(json);

                ApiBaseUrl = config?.ApiSettings?.BaseUrl;


                if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                    throw new Exception("Không tìm thấy ApiSettings:ApiBaseUrl trong appsettings.json");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Lỗi đọc ApiBaseUrl: " + ex.Message, ex);
            }
        }
    }
}

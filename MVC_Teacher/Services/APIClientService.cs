using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using MVC_Teacher.Models;
using System.Collections.Generic;

namespace MVC_Teacher.Services
{
    public class APIClientService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase;

        public APIClientService()
        {
            _httpClient = new HttpClient();
            _apiBase = ConfigurationManager.AppSettings["ApiBaseUrl"];

            if (string.IsNullOrEmpty(_apiBase))
                throw new InvalidOperationException("ApiBaseUrl chưa được cấu hình trong Web.config");
        }

        // Gọi GET API chung
        public async Task<T> GetAsync<T>(string endpoint)
        {
            var url = _apiBase.TrimEnd('/') + "/" + endpoint.TrimStart('/');
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API lỗi {response.StatusCode}: {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        // Gọi GET chi tiết khóa học
        public async Task<CourseDetailDto> GetCourseByIdAsync(int maKhoaHoc)
        {
            return await GetAsync<CourseDetailDto>($"Courses/{maKhoaHoc}");
        }

        // Gọi GET quiz của giáo viên
        public async Task<List<QuizDto>> GetQuizzesByTeacherAsync(int maGiaoVien)
        {
            return await GetAsync<List<QuizDto>>($"Quiz/teacher/{maGiaoVien}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MVC_ADMIN.Services
{
    /// <summary>
    /// Service để quản lý các HTTP calls đến API
    /// Tái sử dụng HttpClient và xử lý lỗi tập trung
    /// </summary>
    public class ApiService : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private bool _disposed = false;

        public ApiService(string baseUrl)
        {
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
            
            // Xử lý SSL certificate cho development (bỏ qua lỗi certificate)
            var handler = new System.Net.Http.HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            
            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30) // Timeout 30 giây
            };
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        /// <summary>
        /// GET request
        /// </summary>
        public async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint.TrimStart('/')}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// GET request trả về string
        /// </summary>
        public async Task<string> GetStringAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint.TrimStart('/')}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// POST request với object
        /// </summary>
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint.TrimStart('/')}", content);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// POST request với FormUrlEncodedContent
        /// </summary>
        public async Task<TResponse> PostFormAsync<TResponse>(string endpoint, IEnumerable<KeyValuePair<string, string>> formData)
        {
            try
            {
                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint.TrimStart('/')}", content);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// POST request trả về string
        /// </summary>
        public async Task<string> PostStringAsync(string endpoint, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint.TrimStart('/')}", content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// PUT request
        /// </summary>
        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint.TrimStart('/')}", content);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(responseJson);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// DELETE request
        /// </summary>
        public async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint.TrimStart('/')}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// GET request với error handling tốt hơn
        /// </summary>
        public async Task<ApiResponse<T>> GetWithErrorHandlingAsync<T>(string endpoint)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
                System.Diagnostics.Debug.WriteLine($"API Request: {url}");
                
                var response = await _httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                
                System.Diagnostics.Debug.WriteLine($"API Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"API Response Content: {(string.IsNullOrEmpty(json) ? "(empty)" : json.Substring(0, Math.Min(500, json.Length)))}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var data = JsonConvert.DeserializeObject<T>(json);
                        return new ApiResponse<T> { Success = true, Data = data };
                    }
                    catch (JsonException jsonEx)
                    {
                        return new ApiResponse<T> 
                        { 
                            Success = false, 
                            Error = $"Lỗi parse JSON: {jsonEx.Message}. Response: {(string.IsNullOrEmpty(json) ? "(empty)" : json.Substring(0, Math.Min(200, json.Length)))}" 
                        };
                    }
                }
                else
                {
                    string errorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                    try
                    {
                        var error = JsonConvert.DeserializeObject<ApiError>(json);
                        if (error != null && !string.IsNullOrEmpty(error.Message))
                            errorMessage = error.Message;
                    }
                    catch { }
                    
                    return new ApiResponse<T> { Success = false, Error = errorMessage };
                }
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                string errorMsg = $"Không thể kết nối đến API: {httpEx.Message}";
                if (httpEx.InnerException != null)
                    errorMsg += $" ({httpEx.InnerException.Message})";
                return new ApiResponse<T> { Success = false, Error = errorMsg };
            }
            catch (TaskCanceledException)
            {
                return new ApiResponse<T> { Success = false, Error = "Request timeout - API không phản hồi" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<T> { Success = false, Error = $"Lỗi: {ex.Message}" };
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Response wrapper với error handling
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
    }

    /// <summary>
    /// Error model từ API
    /// </summary>
    public class ApiError
    {
        public string Message { get; set; }
        public string ErrorCode { get; set; }
    }
}


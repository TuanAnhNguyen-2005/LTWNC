using System;
using MVC_ADMIN.Helpers;

namespace MVC_ADMIN.Services
{
    public static class ApiServiceHelper
    {
        private static readonly Lazy<ApiService> _instance = new Lazy<ApiService>(() =>
            new ApiService("https://localhost:7068"));  // ← ĐÚNG URL BACKEND

        public static ApiService Instance => _instance.Value;
    }
}
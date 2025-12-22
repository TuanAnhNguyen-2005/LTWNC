using MVC_STUDENT.Models;
using MVC_STUDENT.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC_STUDENT.Controllers
{
    public class HocLieuController : Controller
    {
        private readonly string _apiBaseUrl;

        public HocLieuController()
        {
            var apiConfig = new ApiConfigService();
            _apiBaseUrl = apiConfig.ApiBaseUrl;
        }

        // GET: /HocLieu/Course?maKhoaHoc=33
        public async Task<ActionResult> Course(int maKhoaHoc)
        {
            var baseApi = _apiBaseUrl.TrimEnd('/');
            if (!baseApi.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
                baseApi += "/api";

            var url = $"{baseApi}/HocLieu/course/{maKhoaHoc}";
            ViewBag.DebugUrl = url;

            using (var http = new HttpClient())
            {
                var resp = await http.GetAsync(url);

                if (!resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    ViewBag.Error = $"HTTP {(int)resp.StatusCode} - {resp.ReasonPhrase}\n{body}";
                    return View(new List<HocLieuVm>());
                }

                var json = await resp.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<List<HocLieuVm>>(json)
                           ?? new List<HocLieuVm>();

                ViewBag.FileBaseUrl = baseApi.Replace("/api", "");
                return View(data);
            }
        }

    }
}

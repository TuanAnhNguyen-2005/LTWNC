using System;
using System.Web.Mvc;

namespace MVC_STUDENT.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search?keyword=...
        public ActionResult Index(string keyword)
        {
            ViewBag.Title = "Tìm kiếm";
            ViewBag.Keyword = keyword ?? "";
            return View();
        }
    }
}


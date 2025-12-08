using System;
using System.Web.Mvc;

namespace MVC_STUDENT.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            ViewBag.Title = "Danh mục";
            return View();
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Chi tiết danh mục";
            ViewBag.CategoryId = id;
            return View();
        }
    }
}


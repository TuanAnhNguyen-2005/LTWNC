using System.Web.Mvc;

namespace MVC_ADMIN.Controllers
{
    public class CategoryController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Quản lý danh mục";
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}
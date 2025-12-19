using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVC_STUDENT.Controllers
{
    public class LearningMaterialController : Controller
    {
        // GET: LearningMaterial
        public ActionResult Index()
        {
            ViewBag.Title = "Học liệu";
            return View();
        }

        // GET: LearningMaterial/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Title = "Chi tiết học liệu";
            ViewBag.MaterialId = id;
            return View();
        }

        // GET: LearningMaterial/Category/5
        public ActionResult Category(int id)
        {
            ViewBag.Title = "Học liệu theo danh mục";
            ViewBag.CategoryId = id;
            return View();
        }
    }
}





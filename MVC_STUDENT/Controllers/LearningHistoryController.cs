using System;
using System.Web.Mvc;

namespace MVC_STUDENT.Controllers
{
    public class LearningHistoryController : Controller
    {
        // GET: LearningHistory
        public ActionResult Index()
        {
            ViewBag.Title = "Lịch sử học tập";
            return View();
        }
    }
}




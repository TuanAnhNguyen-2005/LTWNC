using System;
using System.Web.Mvc;

namespace MVC_STUDENT.Controllers
{
    public class FavoritesController : Controller
    {
        // GET: Favorites
        public ActionResult Index()
        {
            ViewBag.Title = "Yêu thích";
            return View();
        }
    }
}





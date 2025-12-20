using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_STUDENT.Controllers
{
    public class TrangChuController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("=== TRANG CHỦ INDEX ĐƯỢC GỌI ===");
            System.Diagnostics.Debug.WriteLine($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");

            ViewBag.Message = "Chào mừng đến Trang Chủ!";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
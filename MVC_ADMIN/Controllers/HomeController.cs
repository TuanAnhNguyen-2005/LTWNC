using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_ADMIN.Filters;

namespace MVC_ADMIN.Controllers
{
    /// <summary>
    /// Controller cho trang chủ (dành cho Teacher và Student)
    /// </summary>
    [AuthorizeRole("Teacher", "Student")]
    public class HomeController : BaseController
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Title = "Trang chủ";
            ViewBag.UserName = GetUserFullName();
            ViewBag.UserRole = GetUserRole();
            return View();
        }
    }
}
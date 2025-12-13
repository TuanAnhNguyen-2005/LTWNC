// Controllers/BaseController.cs
using System;
using System.Web.Mvc;

namespace MVC_Teacher.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        protected ActionResult RedirectToHomeByRole()
        {
            var role = Session["Role"] as string;

            switch (role?.ToLower())
            {
                case "admin":
                    return RedirectToAction("Index", "Admin");
                case "teacher":
                    return RedirectToAction("Index", "Dashboard");
                case "student":
                    return RedirectToAction("Index", "Student");
                default:
                    return RedirectToAction("Index", "Login");
            }
        }

        protected void HandleException(Exception ex)
        {
            // Log exception
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }

        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }
    }
}
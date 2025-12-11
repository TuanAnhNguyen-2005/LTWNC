using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using MVC_Teacher.Models;

namespace MVC_Teacher.Controllers
{
    public class AccountController : Controller
    {
        private readonly NenTangHocLieuContext db = new NenTangHocLieuContext();

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = db.UserAccounts
                         .FirstOrDefault(u => u.UserName == model.UserNameOrEmail || u.Email == model.UserNameOrEmail);
            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                return View(model);
            }

            if (user.PasswordHash != ComputeSha256Hash(model.Password))
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                return View(model);
            }

            // sign in
            FormsAuthentication.SetAuthCookie(user.UserName, false);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        public ActionResult Register()
        {
            return View("Login"); // single view handles tabs
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View("Login", model);

            if (db.UserAccounts.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại.");
                return View("Login", model);
            }

            if (db.UserAccounts.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View("Login", model);
            }

            var user = new UserAccount
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName,
                PasswordHash = ComputeSha256Hash(model.Password),
                CreatedAt = DateTime.UtcNow
            };

            db.UserAccounts.Add(user);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công. Bạn có thể đăng nhập bây giờ.";
            return RedirectToAction("Login");
        }

        // GET: /Account/ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View("Login");
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View("Login", model);

            var user = db.UserAccounts.FirstOrDefault(u => u.Email == model.Email);
            // For demo: do not leak whether email exists. In production, send reset email with token.
            TempData["Info"] = "Nếu email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private static string ComputeSha256Hash(string rawData)
        {
            if (rawData == null) return "";
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
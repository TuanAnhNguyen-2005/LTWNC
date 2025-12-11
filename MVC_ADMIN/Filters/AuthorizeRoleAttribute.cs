using System;
using System.Web;
using System.Web.Mvc;

namespace MVC_ADMIN.Filters
{
    /// <summary>
    /// Attribute để kiểm tra quyền truy cập theo role
    /// Sử dụng: [AuthorizeRole("Admin")] hoặc [AuthorizeRole("Admin", "Teacher")]
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedRoles;

        public AuthorizeRoleAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            // Kiểm tra đã đăng nhập chưa
            if (httpContext.Session["UserId"] == null || httpContext.Session["Email"] == null)
                return false;

            // Kiểm tra role
            string userRole = httpContext.Session["Role"]?.ToString();
            if (string.IsNullOrEmpty(userRole))
                return false;

            // Kiểm tra role có trong danh sách allowed roles không
            foreach (string role in _allowedRoles)
            {
                if (string.Equals(userRole, role, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserId"] == null)
            {
                // Chưa đăng nhập → redirect về login
                filterContext.Result = new RedirectResult("~/Login");
            }
            else
            {
                // Đã đăng nhập nhưng không có quyền → redirect về trang chủ
                string role = filterContext.HttpContext.Session["Role"]?.ToString();
                if (string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    // Trang dashboard admin
                    filterContext.Result = new RedirectResult("~/Admin/Statistical");
                }
                else
                {
                    // Teacher/Student trong site admin: đưa về trang Home chung (có layout gọn)
                    filterContext.Result = new RedirectResult("~/Home");
                }
            }
        }
    }
}


using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_ADMIN
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            // Chặn truy cập trực tiếp vào Views folder
            routes.IgnoreRoute("Views/{*pathInfo}");

            // Route cho Login và SignUp (public pages)
            routes.MapRoute(
                name: "Login",
                url: "Login/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "SignUp",
                url: "SignUp/{action}/{id}",
                defaults: new { controller = "SignUp", action = "Index", id = UrlParameter.Optional }
            );

            // 1. ROUTE ADMIN
            routes.MapRoute(
                name: "Admin",
                url: "Admin/{controller}/{action}/{id}",
                defaults: new { controller = "Statistical", action = "Index", id = UrlParameter.Optional }
            );

            // 2. ROUTE MẶC ĐỊNH – TRANG CHỦ CHUYỂN HƯỚNG LUÔN VÀO ADMIN
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Statistical", action = "Index", id = UrlParameter.Optional }
            );

            // 3. (TÙY CHỌN) Nếu vẫn muốn giữ trang Home/Index cũ thì để dòng này ở dưới cùng
            routes.MapRoute(
                name: "HomePage",
                url: "Home/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
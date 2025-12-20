using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MVC_Teacher.Services;

namespace MVC_Teacher
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                AreaRegistration.RegisterAllAreas();
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                // Create sample/default users on startup - tạm thời tắt
                // TODO: Uncomment sau khi đảm bảo database connection hoạt động
                /*
                try
                {
                    var userService = new UserDataService();
                    userService.CreateSampleUser();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error creating sample users on startup: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
                */
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không throw để có thể debug
                System.Diagnostics.Debug.WriteLine($"Error in Application_Start: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.InnerException.StackTrace}");
                }
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                // Không throw để tránh crash
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception lastError = Server.GetLastError();
            if (lastError != null)
            {
                System.Diagnostics.Debug.WriteLine($"Application Error: {lastError.Message}");
                if (lastError.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {lastError.InnerException.Message}");
                }
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {lastError.StackTrace}");
            }
        }
    }
}

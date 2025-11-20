using System.Web;
using System.Web.Mvc;

namespace RESTAPI_NenTangHocLieu
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

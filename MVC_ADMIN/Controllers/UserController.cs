using System.Web.Mvc;

public class UserController : Controller
{
    // Kiểm tra role Admin
    public ActionResult Index()
    {
        if (Session["Role"]?.ToString() != "Admin")
            return Redirect("/Login");

        // Gọi API lấy danh sách user
        // ...
        return View();
    }
}
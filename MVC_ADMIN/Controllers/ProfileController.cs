using System.Web.Mvc;

[Authorize] // tự viết filter hoặc dùng [Authorize] nếu có Forms Auth
public class ProfileController : Controller
{
    public ActionResult Index()
    {
        ViewBag.FullName = Session["FullName"];
        ViewBag.Email = Session["Email"];
        return View();
    }

    public ActionResult Edit()
    {
        return View();
    }
}
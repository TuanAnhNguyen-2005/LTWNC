using System.Web.Mvc;
using System.Linq;
using MVC_ADMIN.Models;           // ← SỬA THÀNH MVC_ADMIN
using System.Data.Entity;

namespace MVC_ADMIN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly NenTangHocLieuContext db = new NenTangHocLieuContext();

        public ActionResult Index()
        {
            return View(db.ChuDes.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChuDe chuDe)
        {
            if (ModelState.IsValid)
            {
                db.ChuDes.Add(chuDe);
                db.SaveChanges();
                TempData["Success"] = "Thêm chủ đề thành công!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var chuDe = db.ChuDes.Find(id);
            if (chuDe != null)
            {
                db.ChuDes.Remove(chuDe);
                db.SaveChanges();
                TempData["Success"] = "Xóa chủ đề thành công!";
            }
            return RedirectToAction("Index");
        }
    }
}
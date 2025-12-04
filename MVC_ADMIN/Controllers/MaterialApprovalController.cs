using System;
using System.Linq;
using System.Web.Mvc;
using MVC_ADMIN.Models;           // ← SỬA THÀNH MVC_ADMIN
using System.Data.Entity;

namespace MVC_ADMIN.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MaterialApprovalController : Controller
    {
        private readonly NenTangHocLieuContext db = new NenTangHocLieuContext();

        public ActionResult Pending(string search = null)
        {
            var query = db.HocLieus
                          .Include("NguoiDung")
                          .Include("MonHoc")
                          .Include("ChuDe")
                          .Where(h => h.TrangThaiDuyet == "Chờ duyệt");

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(h => h.TieuDe.Contains(search) ||
                                    (h.NguoiDung != null && h.NguoiDung.HoTen.Contains(search)));
            }

            ViewBag.Search = search;
            return View(query.OrderByDescending(h => h.NgayDang).ToList());
        }

        [HttpPost]
        public ActionResult Approve(int id)
        {
            var hl = db.HocLieus.Find(id);
            if (hl == null) return HttpNotFound();

            hl.DaDuyet = true;
            hl.TrangThaiDuyet = "Đã duyệt";
            int.TryParse(User.Identity.Name, out int userId);
            hl.NguoiDuyet = userId;
            hl.NgayDuyet = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = "Đã duyệt tài liệu thành công!";
            return RedirectToAction("Pending");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id, string lyDoTuChoi)
        {
            if (string.IsNullOrWhiteSpace(lyDoTuChoi))
            {
                TempData["Error"] = "Vui lòng nhập lý do từ chối!";
                return RedirectToAction("Pending");
            }

            var hl = db.HocLieus.Find(id);
            if (hl == null) return HttpNotFound();

            hl.DaDuyet = false;
            hl.TrangThaiDuyet = "Từ chối";
            hl.LyDoTuChoi = lyDoTuChoi;
            int.TryParse(User.Identity.Name, out int userId);
            hl.NguoiDuyet = userId;
            hl.NgayDuyet = DateTime.Now;
            db.SaveChanges();

            TempData["Success"] = "Đã từ chối tài liệu!";
            return RedirectToAction("Pending");
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Teacher.Models;          // ĐÚNG
using System.Data.Entity;

namespace MVC_Teacher.Controllers
{
    [Authorize(Roles = "Giảng viên,Sinh viên")]
    public class MaterialController : Controller
    {
        private readonly NenTangHocLieuContext db = new NenTangHocLieuContext();

        public ActionResult MyMaterials(string search = null, string doKho = null, int? monHoc = null, int? chuDe = null)
        {
            int userId = Convert.ToInt32(User.Identity.Name);

            var query = db.HocLieus.Where(h => h.MaNguoiDung == userId);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(h => h.TieuDe.Contains(search) || (h.MoTa ?? "").Contains(search));
            if (!string.IsNullOrEmpty(doKho))
                query = query.Where(h => h.DoKho == doKho);
            if (monHoc.HasValue)
                query = query.Where(h => h.MaMonHoc == monHoc);
            if (chuDe.HasValue)
                query = query.Where(h => h.MaChuDe == chuDe);

            ViewBag.MonHocs = db.MonHocs.ToList();
            ViewBag.ChuDes = db.ChuDes.ToList();
            ViewBag.LopHocs = db.LopHocs.ToList();
            ViewBag.Search = search;
            ViewBag.DoKho = doKho;
            ViewBag.SelectedMonHoc = monHoc;
            ViewBag.SelectedChuDe = chuDe;

            return View(query.OrderByDescending(h => h.NgayDang).ToList());
        }

        public ActionResult Create()
        {
            ViewBag.MonHocs = db.MonHocs.ToList();
            ViewBag.ChuDes = db.ChuDes.ToList();
            ViewBag.LopHocs = db.LopHocs.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HocLieu model, HttpPostedFileBase file)
        {
            int userId = Convert.ToInt32(User.Identity.Name);   // ← ĐÃ KHAI BÁO LẠI

            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/uploads/"), fileName);
                Directory.CreateDirectory(Server.MapPath("~/uploads/"));
                file.SaveAs(path);

                model.DuongDanTep = "/uploads/" + fileName;
                model.LoaiTep = file.ContentType;
                model.KichThuocTep = file.ContentLength;
                model.MaNguoiDung = userId;
                model.NgayDang = DateTime.Now;
                model.TrangThaiDuyet = "Chờ duyệt";
                model.DaDuyet = false;

                db.HocLieus.Add(model);
                db.SaveChanges();

                TempData["Success"] = "Tài liệu đã được gửi duyệt!";
                return RedirectToAction("MyMaterials");
            }

            ViewBag.MonHocs = db.MonHocs.ToList();
            ViewBag.ChuDes = db.ChuDes.ToList();
            ViewBag.LopHocs = db.LopHocs.ToList();
            return View(model);
        }
    }
}
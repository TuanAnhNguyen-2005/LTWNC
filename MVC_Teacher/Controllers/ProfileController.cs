using System;
using System.Web.Mvc;
using MVC_Teacher.Models;

namespace MVC_Teacher.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Index()
        {
            try
            {
                // Ưu tiên lấy từ Session
                Teacher teacher = null;

                // Kiểm tra Session có TeacherData không
                if (Session["TeacherData"] != null)
                {
                    teacher = Session["TeacherData"] as Teacher;
                }
                else if (Session["FullName"] != null)
                {
                    // Nếu có Session riêng lẻ, tạo Teacher từ đó
                    teacher = new Teacher
                    {
                        TeacherId = Session["UserId"] != null ? (int)Session["UserId"] : 1,
                        FullName = Session["FullName"]?.ToString() ?? "Nguyễn Văn A",
                        Email = Session["Email"]?.ToString() ?? "teacher@example.com",
                        PhoneNumber = "0912345678",
                        Subject = Session["Subject"]?.ToString() ?? "Toán",
                        Role = Session["Role"]?.ToString() ?? "Giáo viên",
                        IsActive = true,
                        IsVerified = true,
                        CreatedDate = DateTime.Now.AddMonths(-3),
                        LastLoginDate = DateTime.Now.AddHours(-2)
                    };

                    // Lưu vào Session TeacherData để lần sau dùng
                    Session["TeacherData"] = teacher;
                }
                else
                {
                    // Tạo dữ liệu test mới
                    teacher = new Teacher
                    {
                        TeacherId = 1,
                        FullName = "Nguyễn Văn A",
                        Email = "teacher@example.com",
                        PhoneNumber = "0912345678",
                        Subject = "Toán",
                        Role = "Giáo viên",
                        IsActive = true,
                        IsVerified = true,
                        CreatedDate = DateTime.Now.AddMonths(-3),
                        LastLoginDate = DateTime.Now.AddHours(-2)
                    };

                    // Lưu vào Session
                    Session["TeacherData"] = teacher;
                    Session["UserId"] = teacher.TeacherId;
                    Session["FullName"] = teacher.FullName;
                    Session["Email"] = teacher.Email;
                    Session["Subject"] = teacher.Subject;
                    Session["Role"] = teacher.Role;
                }

                ViewBag.LoginCount = 15;
                return View(teacher);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new Teacher());
            }
        }

        // POST: Profile/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(Teacher model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy Teacher hiện tại từ Session
                    Teacher currentTeacher = Session["TeacherData"] as Teacher ?? new Teacher();

                    // Cập nhật thông tin
                    currentTeacher.TeacherId = model.TeacherId;
                    currentTeacher.FullName = model.FullName;
                    currentTeacher.Email = model.Email;
                    currentTeacher.PhoneNumber = model.PhoneNumber;
                    currentTeacher.Subject = model.Subject;
                    currentTeacher.Role = model.Role;

                    // Lưu vào Session (QUAN TRỌNG: cả object và từng property)
                    Session["TeacherData"] = currentTeacher;
                    Session["FullName"] = currentTeacher.FullName;
                    Session["Email"] = currentTeacher.Email;
                    Session["Subject"] = currentTeacher.Subject;
                    Session["Role"] = currentTeacher.Role;

                    // Lưu vào TempData để Home có thể nhận
                    TempData["UpdatedTeacher"] = currentTeacher;
                    TempData["ProfileUpdated"] = true;

                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Lỗi: " + ex.Message;
                    return View("Index", model);
                }
            }

            return View("Index", model);
        }

        // API để Home gọi lấy thông tin mới
        [HttpGet]
        public JsonResult GetCurrentTeacher()
        {
            var teacher = Session["TeacherData"] as Teacher;
            if (teacher == null)
            {
                teacher = new Teacher
                {
                    FullName = Session["FullName"] as string ?? "Thầy/Cô",
                    Subject = Session["Subject"] as string ?? "Môn học",
                    Email = Session["Email"] as string ?? "email@example.com"
                };
            }

            return Json(new
            {
                fullName = teacher.FullName,
                subject = teacher.Subject,
                email = teacher.Email,
                role = teacher.Role
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MVC_ADMIN.Filters;
using MVC_ADMIN.Models;
using System.Globalization;
using MVC_ADMIN.Services;

namespace MVC_ADMIN.Controllers
{
    [AuthorizeRole("Admin", "Teacher", "Student")]
    public class ProfileController : BaseController
    {
        // Trang xem thông tin cá nhân
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Title = "Thông tin cá nhân";

                var userId = GetUserId();
                if (!userId.HasValue)
                    return RedirectToLogin();


                var api = ApiServiceHelper.Instance;
                var response = await api.GetWithErrorHandlingAsync<dynamic>($"api/NguoiDung/{userId}");

                if (response.Success && response.Data != null)
                {
                    var p = response.Data;
                    ViewBag.Profile = new {
                        fullName    = p.HoTen ?? p.hoTen ?? p.FullName ?? p.fullName ?? "",
                        email       = p.Email ?? p.email ?? "",
                        phone       = p.SoDienThoai ?? p.soDienThoai ?? "",
                        address     = p.DiaChi ?? p.diaChi ?? "",
                        dateOfBirth = p.NgaySinh != null ? (p.NgaySinh is string ? p.NgaySinh : (p.NgaySinh.ToString("yyyy-MM-dd"))) : (p.ngaySinh ?? ""),
                        gender      = p.GioiTinh ?? p.gioiTinh ?? "",
                        role        = (p.MaVaiTroNavigation != null ? (p.MaVaiTroNavigation.TenVaiTro ?? "") : (p.maVaiTroNavigation != null ? (p.maVaiTroNavigation.tenVaiTro ?? "") : (ViewBag.Role ?? ""))),
                        createdDate = p.NgayTao != null ? (p.NgayTao is string ? p.NgayTao : p.NgayTao.ToString("yyyy-MM-dd")) : (p.ngayTao ?? "")
                    };

                }
                else
                {
                    // Fallback từ Session nếu API lỗi
                    ViewBag.FullName = GetUserFullName();
                    ViewBag.Email = GetUserEmail();
                    ViewBag.Role = GetUserRole();
                }

                return View();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                ViewBag.FullName = GetUserFullName();
                ViewBag.Email = GetUserEmail();
                ViewBag.Role = GetUserRole();
                return View();
            }
        }

        // Trang chỉnh sửa (GET)
        public async Task<ActionResult> Edit()
        {
            try
            {
                ViewBag.Title = "Chỉnh sửa thông tin cá nhân";

                var userId = GetUserId();
                if (!userId.HasValue)
                    return RedirectToLogin();

                var api = ApiServiceHelper.Instance;
                var response = await api.GetWithErrorHandlingAsync<dynamic>($"api/NguoiDung/{userId}");

                var model = new ProfileEditViewModel();

                if (response.Success && response.Data != null)
                {
                    var profile = response.Data;

                    model.FullName = profile.HoTen ?? profile.hoTen ?? "";
                    model.Email = profile.Email ?? profile.email ?? "";
                    model.Phone = profile.SoDienThoai ?? profile.soDienThoai ?? "";
                    model.Address = profile.DiaChi ?? profile.diaChi ?? "";
                    model.Gender = profile.GioiTinh ?? profile.gioiTinh ?? "";

                    // Xử lý NgaySinh (DateOnly? → string yyyy-MM-dd)
                    model.DateOfBirth = profile.NgaySinh?.ToString() ?? profile.ngaySinh?.ToString() ?? "";
                }

                return View(model);
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return View(new ProfileEditViewModel());
            }
        }

        // Xử lý submit form (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return RedirectToLogin();

                // Lấy lại bản gốc từ API để đảm bảo không mất thông tin cũ
                var api = ApiServiceHelper.Instance;
                var oldRes = await api.GetWithErrorHandlingAsync<dynamic>($"api/NguoiDung/{userId}");
                var old = oldRes.Data;

                dynamic profileToUpdate = new ExpandoObject();
                var dict = (IDictionary<string, object>)profileToUpdate;

                dict["MaNguoiDung"] = userId.Value;
                dict["HoTen"] = model.FullName?.Trim() ?? (old.HoTen ?? "");
                dict["Email"] = model.Email?.Trim() ?? (old.Email ?? "");
                dict["SoDienThoai"] = model.Phone?.Trim() ?? (old.SoDienThoai ?? "");
                dict["DiaChi"] = model.Address?.Trim() ?? (old.DiaChi ?? "");
                dict["GioiTinh"] = model.Gender ?? (old.GioiTinh ?? "");
                // Bảo toàn mật khẩu: nếu DB đang null/rỗng, đặt mặc định "1" để tránh bị lưu chuỗi rỗng
                var oldPassword = (string)(old.MatKhau ?? old.matKhau ?? "");
                if (string.IsNullOrWhiteSpace(oldPassword))
                {
                    oldPassword = "1";
                }
                dict["MatKhau"] = oldPassword;
                dict["MaVaiTro"] = old.MaVaiTro;
                dict["AnhDaiDien"] = old.AnhDaiDien ?? null;
                dict["NgayTao"] = old.NgayTao ?? null;
                dict["LanDangNhapCuoi"] = old.LanDangNhapCuoi ?? null;
                dict["TrangThai"] = old.TrangThai ?? null;

                // Xử lý NgaySinh kiểu DateOnly?
                if (!string.IsNullOrEmpty(model.DateOfBirth) &&
                    DateTime.TryParseExact(model.DateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    dict["NgaySinh"] = model.DateOfBirth;
                }
                else
                {
                    dict["NgaySinh"] = old.NgaySinh ?? null;
                }

                // Gọi API PUT đúng endpoint
                var response = await api.PutAsync<dynamic, dynamic>($"api/NguoiDung/{userId}", profileToUpdate);

                // Backend trả NoContent() → ApiService thường trả response null hoặc không có Success khi thành công
                // Nếu không throw exception → coi như thành công
                Session["FullName"] = model.FullName;
                Session["Email"] = model.Email;

                SetSuccessMessage("Cập nhật thông tin thành công!");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                    errorMsg += " | Inner: " + ex.InnerException.Message;

                SetErrorMessage($"Đã xảy ra lỗi khi cập nhật thông tin: {errorMsg}");
                return View(model);
            }
        }

        // GET: Đổi mật khẩu
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
                return RedirectToLogin();

            ViewBag.Title = "Đổi mật khẩu";
            return View(new ChangePasswordViewModel());
        }

        // POST: Đổi mật khẩu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetUserId();
            if (!userId.HasValue)
                return RedirectToLogin();

            try
            {
                var api = ApiServiceHelper.Instance;
                // Lấy dữ liệu gốc để bảo toàn các trường khác
                var oldRes = await api.GetWithErrorHandlingAsync<dynamic>($"api/NguoiDung/{userId}");
                if (!oldRes.Success || oldRes.Data == null)
                {
                    SetErrorMessage("Không lấy được thông tin tài khoản hiện tại.");
                    return View(model);
                }

                var old = oldRes.Data;
                var currentPasswordDb = (string)(old.MatKhau ?? old.matKhau ?? "");
                if (string.IsNullOrWhiteSpace(currentPasswordDb))
                {
                    // Nếu DB trống, coi như không có mật khẩu cũ để so sánh
                    currentPasswordDb = "";
                }

                // So sánh mật khẩu cũ
                if (!string.Equals(model.CurrentPassword ?? "", currentPasswordDb ?? "", StringComparison.Ordinal))
                {
                    ModelState.AddModelError(nameof(model.CurrentPassword), "Mật khẩu hiện tại không đúng.");
                    return View(model);
                }

                // Chuẩn bị payload giữ nguyên các field khác
                dynamic profileToUpdate = new ExpandoObject();
                var dict = (IDictionary<string, object>)profileToUpdate;

                dict["MaNguoiDung"] = userId.Value;
                dict["HoTen"] = old.HoTen ?? old.hoTen ?? "";
                dict["Email"] = old.Email ?? old.email ?? "";
                dict["SoDienThoai"] = old.SoDienThoai ?? old.soDienThoai ?? "";
                dict["DiaChi"] = old.DiaChi ?? old.diaChi ?? "";
                dict["GioiTinh"] = old.GioiTinh ?? old.gioiTinh ?? "";

                // Ngày sinh: giữ định dạng chuỗi yyyy-MM-dd nếu có
                string dob = null;
                if (old.NgaySinh != null)
                {
                    dob = old.NgaySinh is string ? old.NgaySinh : old.NgaySinh.ToString("yyyy-MM-dd");
                }
                else if (old.ngaySinh != null)
                {
                    dob = old.ngaySinh.ToString();
                }
                dict["NgaySinh"] = string.IsNullOrWhiteSpace(dob) ? null : dob;

                // Mật khẩu mới
                var newPassword = model.NewPassword?.Trim();
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    // fallback không cho để trống
                    newPassword = currentPasswordDb;
                }
                dict["MatKhau"] = newPassword;

                dict["MaVaiTro"] = old.MaVaiTro ?? old.maVaiTro ?? null;
                dict["AnhDaiDien"] = old.AnhDaiDien ?? old.anhDaiDien ?? null;
                dict["NgayTao"] = old.NgayTao ?? old.ngayTao ?? null;
                dict["LanDangNhapCuoi"] = old.LanDangNhapCuoi ?? old.lanDangNhapCuoi ?? null;
                dict["TrangThai"] = old.TrangThai ?? old.trangThai ?? null;

                await api.PutAsync<dynamic, dynamic>($"api/NguoiDung/{userId}", profileToUpdate);

                SetSuccessMessage("Đổi mật khẩu thành công!");
                return RedirectToAction("ChangePassword");
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                    errorMsg += " | Inner: " + ex.InnerException.Message;

                SetErrorMessage($"Đã xảy ra lỗi khi đổi mật khẩu: {errorMsg}");
                return View(model);
            }
        }
    }
}
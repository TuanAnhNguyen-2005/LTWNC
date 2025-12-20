using MVC_STUDENT.Models; // sửa namespace nếu khác
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

public class LoginController : Controller
{
    private readonly string _apiBase =
        System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];

    [HttpGet]
    [AllowAnonymous]
    public ActionResult Index(string returnUrl)
    {
        if (User?.Identity?.IsAuthenticated == true && Session["UserId"] == null)
        {
            System.Web.Security.FormsAuthentication.SignOut();
        }

        return View();
    }

    // POST /Login  (KHỚP action="/Login")
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Index(string email, string password, bool rememberMe = false, string returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            TempData["Error"] = "Vui lòng nhập email và mật khẩu.";
            return RedirectToAction("Index");
        }

        var url = _apiBase.TrimEnd('/') + "/Auth/login"; // https://localhost:7068/api/Auth/login

        using (var http = new HttpClient())
        {
            // Nếu gặp lỗi SSL dev cert, bạn nói mình -> mình chỉ cách trust cert hoặc chuyển http
            var req = new AuthLoginRequest { UserName = email, Password = password };
            var json = JsonConvert.SerializeObject(req);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage res;
            try
            {
                res = await http.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không gọi được API: " + ex.Message;
                return RedirectToAction("Index");
            }

            if (!res.IsSuccessStatusCode)
            {
                TempData["Error"] = "Sai tài khoản hoặc mật khẩu.";
                return RedirectToAction("Index");
            }

            var body = await res.Content.ReadAsStringAsync();
            var auth = JsonConvert.DeserializeObject<AuthLoginResponse>(body);

            if (auth == null || auth.MaNguoiDung <= 0)
            {
                TempData["Error"] = "Đăng nhập thất bại (API trả dữ liệu không hợp lệ).";
                return RedirectToAction("Index");
            }

            // ✅ CHẶN ROLE học viên
            // Bạn đổi đúng mã VaiTro học viên trong DB của bạn
            const int STUDENT_ROLE = 3;
            if (auth.MaVaiTro != STUDENT_ROLE)
            {
                TempData["Error"] = "Bạn không đủ quyền truy cập (chỉ tài khoản học viên).";
                return RedirectToAction("Index");
            }

            // Lưu session
            Session["UserId"] = auth.MaNguoiDung;
            Session["Email"] = auth.Email;
            Session["FullName"] = auth.HoTen;
            Session["RoleId"] = auth.MaVaiTro;
            Session["AccessToken"] = auth.Token; // nếu có

            // Forms auth cookie cho [Authorize]
            FormsAuthentication.SetAuthCookie(auth.Email ?? email, rememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "TrangChu");
        }
    }

    [AllowAnonymous]
    public ActionResult Logout()
    {
        // Clear session
        Session.Clear();
        Session.Abandon();

        // Forms auth sign out
        System.Web.Security.FormsAuthentication.SignOut();

        // Xoá cookie auth
        var authCookie = new HttpCookie(
            System.Web.Security.FormsAuthentication.FormsCookieName, "")
        {
            Expires = DateTime.Now.AddDays(-1),
            Path = System.Web.Security.FormsAuthentication.FormsCookiePath
        };
        Response.Cookies.Add(authCookie);

        // Xoá session cookie
        var sessionCookie = new HttpCookie("ASP.NET_SessionId", "")
        {
            Expires = DateTime.Now.AddDays(-1),
            Path = "/"
        };
        Response.Cookies.Add(sessionCookie);

        return RedirectToAction("Index", "Login");
    }

}

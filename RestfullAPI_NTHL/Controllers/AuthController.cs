using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly NenTangDbContext _db;
        public AuthController(NenTangDbContext db) => _db = db;

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthLoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.UserName) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("Thiếu thông tin đăng nhập.");

            // Ví dụ: bạn lưu password plain (không khuyến khích) => so sánh trực tiếp
            // Nếu bạn có HashPassword thì thay bằng verify hash.
            var user = _db.NguoiDung
                .FirstOrDefault(x =>
                    (x.Email == req.UserName) &&
                    x.MatKhau == req.Password);

            if (user == null) return Unauthorized("Sai tài khoản hoặc mật khẩu.");

            // Nếu có cột trạng thái khóa tài khoản:
            // if (user.IsLocked) return Forbid("Tài khoản bị khóa.");

            // (Optional) tạo token JWT: ở đây mình để rỗng để bạn chạy trước đã
            var token = "";

            return Ok(new AuthLoginResponse
            {
                MaNguoiDung = user.MaNguoiDung,
                HoTen = user.HoTen,
                Email = user.Email,
                MaVaiTro = user.MaVaiTro ?? 0,
                Token = token
            });
        }
    }
}
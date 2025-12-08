using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestFullAPI.DTOs;
using RestFullAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace RestFullAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserDto
                {
                    UserId = u.MaNguoiDung,
                    FullName = u.HoTen,
                    Email = u.Email,
                    Phone = u.SoDienThoai,
                    Address = u.DiaChi,
                    DateOfBirth = u.NgaySinh,
                    Gender = u.GioiTinh,
                    Role = u.Role != null ? u.Role.TenVaiTro : null,
                    RoleId = u.MaVaiTro,
                    CreatedDate = u.NgayTao,
                    IsActive = u.TrangThai
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.MaNguoiDung == id);

            if (user == null)
            {
                return NotFound(new { error = "Không tìm thấy người dùng" });
            }

            var userDto = new UserDto
            {
                UserId = user.MaNguoiDung,
                FullName = user.HoTen,
                Email = user.Email,
                Phone = user.SoDienThoai,
                Address = user.DiaChi,
                DateOfBirth = user.NgaySinh,
                Gender = user.GioiTinh,
                Role = user.Role != null ? user.Role.TenVaiTro : null,
                RoleId = user.MaVaiTro,
                CreatedDate = user.NgayTao,
                IsActive = user.TrangThai
            };

            return Ok(userDto);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            // Kiểm tra email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email))
            {
                return BadRequest(new { error = "Email đã tồn tại" });
            }

            // Lấy RoleId từ tên role
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.TenVaiTro == createUserDto.Role);

            if (role == null)
            {
                return BadRequest(new { error = "Vai trò không hợp lệ" });
            }

            // Hash password (đơn giản, nên dùng BCrypt trong production)
            var hashedPassword = HashPassword(createUserDto.Password);

            var user = new User
            {
                HoTen = createUserDto.FullName,
                Email = createUserDto.Email,
                MatKhau = hashedPassword,
                SoDienThoai = createUserDto.Phone,
                DiaChi = createUserDto.Address,
                MaVaiTro = role.MaVaiTro,
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserId = user.MaNguoiDung,
                FullName = user.HoTen,
                Email = user.Email,
                Phone = user.SoDienThoai,
                Address = user.DiaChi,
                Role = role.TenVaiTro,
                RoleId = user.MaVaiTro,
                CreatedDate = user.NgayTao,
                IsActive = user.TrangThai
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.MaNguoiDung }, userDto);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "Không tìm thấy người dùng" });
            }

            // Kiểm tra email nếu có thay đổi
            if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email))
                {
                    return BadRequest(new { error = "Email đã tồn tại" });
                }
                user.Email = updateUserDto.Email;
            }

            // Cập nhật các trường
            if (!string.IsNullOrEmpty(updateUserDto.FullName))
                user.HoTen = updateUserDto.FullName;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
                user.MatKhau = HashPassword(updateUserDto.Password);

            if (updateUserDto.Phone != null)
                user.SoDienThoai = updateUserDto.Phone;

            if (updateUserDto.Address != null)
                user.DiaChi = updateUserDto.Address;

            if (updateUserDto.IsActive.HasValue)
                user.TrangThai = updateUserDto.IsActive.Value;

            if (!string.IsNullOrEmpty(updateUserDto.Role))
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.TenVaiTro == updateUserDto.Role);
                if (role != null)
                {
                    user.MaVaiTro = role.MaVaiTro;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { error = "Không tìm thấy người dùng" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.MaNguoiDung == id);
        }

        private string HashPassword(string password)
        {
            // Đơn giản: SHA256 hash (nên dùng BCrypt trong production)
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}


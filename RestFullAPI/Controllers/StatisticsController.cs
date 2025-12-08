using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestFullAPI.Models;

namespace RestFullAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/statistics/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardStatistics()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalCategories = await _context.Categories.CountAsync();
                var totalPermissions = await _context.Permissions.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.TrangThai);
                var activeCategories = await _context.Categories.CountAsync();

                // Thống kê theo role
                var usersByRole = await _context.Users
                    .Include(u => u.Role)
                    .GroupBy(u => u.Role != null ? u.Role.TenVaiTro : "Unknown")
                    .Select(g => new
                    {
                        Role = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Thống kê người dùng mới trong tháng
                var newUsersThisMonth = await _context.Users
                    .CountAsync(u => u.NgayTao.HasValue && 
                        u.NgayTao.Value.Year == DateTime.Now.Year && 
                        u.NgayTao.Value.Month == DateTime.Now.Month);

                var statistics = new
                {
                    totalUsers = totalUsers,
                    activeUsers = activeUsers,
                    inactiveUsers = totalUsers - activeUsers,
                    totalCategories = totalCategories,
                    activeCategories = activeCategories,
                    totalPermissions = totalPermissions,
                    newUsersThisMonth = newUsersThisMonth,
                    usersByRole = usersByRole,
                    lastUpdated = DateTime.Now
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi lấy thống kê", message = ex.Message });
            }
        }

        // GET: api/statistics/users
        [HttpGet("users")]
        public async Task<ActionResult<object>> GetUserStatistics()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var activeUsers = await _context.Users.CountAsync(u => u.TrangThai);
                var usersByRole = await _context.Users
                    .Include(u => u.Role)
                    .GroupBy(u => u.Role != null ? u.Role.TenVaiTro : "Unknown")
                    .Select(g => new
                    {
                        Role = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    totalUsers,
                    activeUsers,
                    inactiveUsers = totalUsers - activeUsers,
                    usersByRole
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi lấy thống kê người dùng", message = ex.Message });
            }
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestFullAPI.DTOs;
using RestFullAPI.Models;

namespace RestFullAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PermissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/permissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
        {
            var permissions = await _context.Permissions
                .Select(p => new PermissionDto
                {
                    PermissionId = p.PermissionId,
                    PermissionName = p.PermissionName,
                    DisplayName = p.DisplayName,
                    Description = p.Description,
                    Module = p.Module,
                    IsActive = p.IsActive,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate
                })
                .ToListAsync();

            return Ok(permissions);
        }

        // GET: api/permissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetPermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
            {
                return NotFound(new { error = "Không tìm thấy quyền" });
            }

            var permissionDto = new PermissionDto
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                DisplayName = permission.DisplayName,
                Description = permission.Description,
                Module = permission.Module,
                IsActive = permission.IsActive,
                CreatedDate = permission.CreatedDate,
                UpdatedDate = permission.UpdatedDate
            };

            return Ok(permissionDto);
        }

        // POST: api/permissions
        [HttpPost]
        public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionDto createPermissionDto)
        {
            // Kiểm tra permission name đã tồn tại chưa
            if (await _context.Permissions.AnyAsync(p => p.PermissionName == createPermissionDto.PermissionName))
            {
                return BadRequest(new { error = "Tên quyền đã tồn tại" });
            }

            var permission = new Permission
            {
                PermissionName = createPermissionDto.PermissionName,
                DisplayName = createPermissionDto.DisplayName,
                Description = createPermissionDto.Description,
                Module = createPermissionDto.Module,
                IsActive = createPermissionDto.IsActive,
                CreatedDate = DateTime.Now
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            var permissionDto = new PermissionDto
            {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                DisplayName = permission.DisplayName,
                Description = permission.Description,
                Module = permission.Module,
                IsActive = permission.IsActive,
                CreatedDate = permission.CreatedDate
            };

            return CreatedAtAction(nameof(GetPermission), new { id = permission.PermissionId }, permissionDto);
        }

        // PUT: api/permissions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, UpdatePermissionDto updatePermissionDto)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound(new { error = "Không tìm thấy quyền" });
            }

            // Kiểm tra permission name nếu có thay đổi
            if (!string.IsNullOrEmpty(updatePermissionDto.PermissionName) && 
                updatePermissionDto.PermissionName != permission.PermissionName)
            {
                if (await _context.Permissions.AnyAsync(p => p.PermissionName == updatePermissionDto.PermissionName))
                {
                    return BadRequest(new { error = "Tên quyền đã tồn tại" });
                }
                permission.PermissionName = updatePermissionDto.PermissionName;
            }

            if (!string.IsNullOrEmpty(updatePermissionDto.DisplayName))
                permission.DisplayName = updatePermissionDto.DisplayName;

            if (updatePermissionDto.Description != null)
                permission.Description = updatePermissionDto.Description;

            if (updatePermissionDto.Module != null)
                permission.Module = updatePermissionDto.Module;

            if (updatePermissionDto.IsActive.HasValue)
                permission.IsActive = updatePermissionDto.IsActive.Value;

            permission.UpdatedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PermissionExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/permissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound(new { error = "Không tìm thấy quyền" });
            }

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> PermissionExists(int id)
        {
            return await _context.Permissions.AnyAsync(e => e.PermissionId == id);
        }
    }
}



using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;
using System.Text.RegularExpressions;

namespace RestfullAPI_NTHL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly NenTangDbContext _db;
        public CoursesController(NenTangDbContext db) => _db = db;
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateCourseDto dto, IFormFile? anhBiaFile)
        {
            if (dto == null) return BadRequest("Body rỗng");
            if (string.IsNullOrWhiteSpace(dto.TenKhoaHoc)) return BadRequest("Thiếu TenKhoaHoc");
            if (dto.MaGiaoVien <= 0) return BadRequest("Thiếu/ sai MaGiaoVien");

            var teacherExists = await _db.NguoiDung.AnyAsync(u => u.MaNguoiDung == dto.MaGiaoVien);
            if (!teacherExists) return BadRequest("MaGiaoVien không tồn tại");

            string? anhBiaPath = null;

            // 🔴 XỬ LÝ UPLOAD ẢNH
            if (anhBiaFile != null && anhBiaFile.Length > 0)
            {
                // Kiểm tra định dạng ảnh
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(anhBiaFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Chỉ chấp nhận file ảnh: jpg, png, gif, webp");

                // Tạo tên file duy nhất
                var fileName = Guid.NewGuid().ToString("N") + extension;
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "courses");

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await anhBiaFile.CopyToAsync(stream);
                }

                anhBiaPath = "https://localhost:7068/images/courses/" + fileName;
            }

            var course = new KhoaHoc
            {
                TenKhoaHoc = dto.TenKhoaHoc.Trim(),
                Slug = string.IsNullOrWhiteSpace(dto.Slug)
                    ? GenerateSlug(dto.TenKhoaHoc)
                    : dto.Slug.Trim(),
                MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim(),
                MaGiaoVien = dto.MaGiaoVien,
                AnhBia = anhBiaPath,  // 🔴 Gán đường dẫn ảnh
                TrangThaiDuyet = "Draft",
                NgayTao = DateTime.Now,
                IsActive = true
            };

            _db.KhoaHoc.Add(course);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Đã tạo khóa học (Draft)",
                course.MaKhoaHoc,
                course.AnhBia
            });
        }

        private string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "khoa-hoc";
            var s = input.ToLowerInvariant();

            // bỏ ký tự đặc biệt
            s = Regex.Replace(s, @"[^a-z0-9\s-]", "");
            // khoảng trắng -> -
            s = Regex.Replace(s, @"\s+", "-").Trim('-');
            // gộp nhiều dấu -
            s = Regex.Replace(s, @"-+", "-");

            return string.IsNullOrWhiteSpace(s) ? "khoa-hoc" : s;
        }
        // GET: api/Courses/teacher/{teacherId}
        // Lấy danh sách khóa học của một giáo viên
        [HttpGet("teacher/{teacherId}")]
        public async Task<IActionResult> GetByTeacher(int teacherId)
        {
            if (teacherId <= 0)
                return BadRequest("teacherId không hợp lệ");

            var data = await _db.KhoaHoc
                .Where(kh => kh.IsActive == true && kh.MaGiaoVien == teacherId)
                .OrderByDescending(kh => kh.NgayTao)
                .Select(kh => new
                {
                    kh.MaKhoaHoc,
                    kh.TenKhoaHoc,
                    kh.AnhBia,                    // 🔴 BẮT BUỘC THÊM DÒNG NÀY
                    kh.TrangThaiDuyet,
                    kh.NgayTao
                })
                .ToListAsync();

            return Ok(data);
        }
        // GET: api/Courses/pending
        // Lấy danh sách khóa học đang "ChoDuyet"
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var data = await _db.KhoaHoc
                .Where(kh => kh.IsActive == true && kh.TrangThaiDuyet == "ChoDuyet")
                .OrderByDescending(kh => kh.NgayGuiDuyet ?? kh.NgayTao)
                .Select(kh => new
                {
                    kh.MaKhoaHoc,
                    kh.TenKhoaHoc,
                    kh.Slug,
                    kh.MoTa,
                    kh.MaGiaoVien,
                    kh.TrangThaiDuyet,
                    kh.NgayTao,
                    kh.NgayGuiDuyet
                })
                .ToListAsync();

            return Ok(data);
        }

        // PUT: api/Courses/{id}/approve?nguoiDuyetId=1
        // Duyệt khóa học: set TrangThaiDuyet = DaDuyet, set NgayDuyet, NguoiDuyetId
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id, [FromQuery] int nguoiDuyetId)
        {
            var course = await _db.KhoaHoc.FirstOrDefaultAsync(kh => kh.MaKhoaHoc == id && kh.IsActive == true);
            if (course == null) return NotFound("Không tìm thấy khóa học");

            // Chỉ cho duyệt khi đang chờ duyệt (tuỳ bạn muốn nới lỏng hay không)
            if (course.TrangThaiDuyet != "ChoDuyet")
                return BadRequest($"Không thể duyệt vì trạng thái hiện tại là '{course.TrangThaiDuyet}'");

            course.TrangThaiDuyet = "DaDuyet";
            course.NgayDuyet = DateTime.Now;
            course.NguoiDuyetId = nguoiDuyetId;
            course.LyDoTuChoi = null;

            await _db.SaveChangesAsync();
            return Ok(new { message = "Đã duyệt khóa học", id = course.MaKhoaHoc });
        }

        // PUT: api/Courses/{id}/reject?nguoiDuyetId=1
        // Body: { "lyDoTuChoi": "Thiếu nội dung ..." }
        public class RejectDto
        {
            public string? LyDoTuChoi { get; set; }
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id, [FromQuery] int nguoiDuyetId, [FromBody] RejectDto dto)
        {
            var course = await _db.KhoaHoc.FirstOrDefaultAsync(kh => kh.MaKhoaHoc == id && kh.IsActive == true);
            if (course == null) return NotFound("Không tìm thấy khóa học");

            if (course.TrangThaiDuyet != "ChoDuyet")
                return BadRequest($"Không thể từ chối vì trạng thái hiện tại là '{course.TrangThaiDuyet}'");

            course.TrangThaiDuyet = "TuChoi";
            course.NgayDuyet = DateTime.Now;
            course.NguoiDuyetId = nguoiDuyetId;
            course.LyDoTuChoi = string.IsNullOrWhiteSpace(dto?.LyDoTuChoi) ? null : dto!.LyDoTuChoi!.Trim();

            await _db.SaveChangesAsync();
            return Ok(new { message = "Đã từ chối khóa học", id = course.MaKhoaHoc });
        }

        // PUT: api/Courses/{id}/submit?teacherId=1
        [HttpPut("{id}/submit")]
        public async Task<IActionResult> SubmitForApproval(int id, [FromQuery] int teacherId)
        {
            var course = await _db.KhoaHoc
                .FirstOrDefaultAsync(kh => kh.MaKhoaHoc == id && kh.IsActive == true);

            if (course == null)
                return NotFound("Không tìm thấy khóa học");

            // Chưa cấu hình auth => không dùng Forbid()
            // Trả 403 thủ công để không crash
            if (course.MaGiaoVien != teacherId)
                return StatusCode(403, "Bạn không có quyền gửi duyệt khóa học này");

            // Cho phép gửi duyệt khi Draft hoặc TuChoi (gửi lại sau bị từ chối)
            if (course.TrangThaiDuyet != "Draft" && course.TrangThaiDuyet != "TuChoi")
                return BadRequest($"Không thể gửi duyệt vì trạng thái hiện tại là '{course.TrangThaiDuyet}'");

            course.TrangThaiDuyet = "ChoDuyet";
            course.NgayGuiDuyet = DateTime.Now;
            course.LyDoTuChoi = null;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Đã gửi duyệt khóa học",
                course.MaKhoaHoc,
                course.TrangThaiDuyet,
                course.NgayGuiDuyet
            });
        }
        public class DeleteBatchDto
        {
            public List<int> Ids { get; set; }
        }

        [HttpDelete("batch")]
        public async Task<IActionResult> DeleteBatch([FromQuery] int teacherId, [FromBody] DeleteBatchDto dto)
        {
            if (dto?.Ids == null || !dto.Ids.Any())
                return BadRequest("Danh sách ID rỗng");

            var courses = await _db.KhoaHoc
                .Where(kh => dto.Ids.Contains(kh.MaKhoaHoc)
                          && kh.IsActive == true
                          && kh.MaGiaoVien == teacherId)
                .ToListAsync();

            if (!courses.Any())
                return BadRequest("Không tìm thấy khóa học nào để xóa");

            foreach (var course in courses)
            {
                course.IsActive = false; // xóa mềm
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = $"Đã xóa thành công {courses.Count} khóa học",
                deletedCount = courses.Count,
                deletedIds = courses.Select(c => c.MaKhoaHoc).ToList()
            });
        }
        // GET: api/Courses/{id} - Lấy chi tiết 1 khóa học (dành cho học sinh xem)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _db.KhoaHoc
                .AsNoTracking()
                .Where(kh => kh.MaKhoaHoc == id && kh.IsActive == true && kh.TrangThaiDuyet == "DaDuyet")
                .Select(kh => new
                {
                    kh.MaKhoaHoc,
                    kh.TenKhoaHoc,
                    kh.MoTa,
                    kh.AnhBia,
                    kh.NgayTao,
                    kh.MaGiaoVien
                    // Thêm field nào cần thiết
                })
                .FirstOrDefaultAsync();

            if (course == null)
                return NotFound("Không tìm thấy khóa học hoặc chưa được duyệt.");

            return Ok(course);
        }

        // GET: api/Courses/published
        // Lấy danh sách khóa học đã được duyệt (công khai cho học sinh)
        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedCourses()
        {
            var data = await _db.KhoaHoc
                .Where(kh => kh.IsActive == true && kh.TrangThaiDuyet == "DaDuyet")
                .OrderByDescending(kh => kh.NgayTao)
                .Select(kh => new
                {
                    kh.MaKhoaHoc,
                    kh.TenKhoaHoc,
                    kh.MoTa,
                    kh.AnhBia,
                    kh.NgayTao
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}

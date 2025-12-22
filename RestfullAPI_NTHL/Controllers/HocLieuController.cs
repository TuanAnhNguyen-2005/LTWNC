using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;

namespace RestfullAPI_NTHL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HocLieuController : ControllerBase
{
    private readonly NenTangDbContext _db;

    public HocLieuController(NenTangDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HocLieu>>> GetAll(
        [FromQuery] int? maChuDe,
        [FromQuery] int? maMonHoc,
        [FromQuery] int? maLopHoc)
    {
        var query = _db.HocLieu.AsNoTracking().AsQueryable();

        if (maChuDe.HasValue) query = query.Where(x => x.MaChuDe == maChuDe);
        if (maMonHoc.HasValue) query = query.Where(x => x.MaMonHoc == maMonHoc);
        if (maLopHoc.HasValue) query = query.Where(x => x.MaLopHoc == maLopHoc);

        var data = await query.ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HocLieu>> GetById(int id)
    {
        var item = await _db.HocLieu.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<HocLieu>> Create(HocLieu model)
    {
        _db.HocLieu.Add(model);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = model.MaHocLieu }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, HocLieu model)
    {
        if (id != model.MaHocLieu) return BadRequest("Id không khớp");

        var exists = await _db.HocLieu.AnyAsync(x => x.MaHocLieu == id);
        if (!exists) return NotFound();

        _db.Entry(model).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.HocLieu.FindAsync(id);
        if (item == null) return NotFound();

        _db.HocLieu.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
    // GET: api/HocLieu/course/5 → Lấy danh sách tài liệu của một khóa học cụ thể
    [HttpGet("course/{maKhoaHoc}")]
    public async Task<IActionResult> GetByKhoaHoc(int maKhoaHoc)
    {
        var data = await _db.HocLieu
            .AsNoTracking()
            .Where(h => h.MaKhoaHoc == maKhoaHoc && h.DaDuyet == true)
            .OrderByDescending(h => h.NgayDang)
            .Select(h => new
            {
                h.MaHocLieu,
                h.TieuDe,
                h.MoTa,
                DuongDanTep = "/files/hoclieu/" + Path.GetFileName(h.DuongDanTep ?? ""), // chỉ trả về phần path public
                h.LoaiTep,
                h.KichThuocTep,
                h.NgayDang
            })
            .ToListAsync();

        return Ok(data);
    }
    // POST: api/HocLieu/upload → Upload file tài liệu cho khóa học
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] HocLieuUploadDto dto)
    {
        // 1) Validate cơ bản
        if (string.IsNullOrWhiteSpace(dto.TieuDe))
            return BadRequest("Thiếu tiêu đề tài liệu");

        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("Chưa chọn file");

        if (dto.File.Length > 50 * 1024 * 1024)
            return BadRequest("File không được lớn hơn 50MB");

        // 2) Kiểm tra giáo viên (MaVaiTro = 2)
        var gv = await _db.NguoiDung
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.MaNguoiDung == dto.MaGiaoVien);

        if (gv == null)
            return Unauthorized("Không tìm thấy giáo viên");

        if (gv.MaVaiTro != 2)
            return StatusCode(StatusCodes.Status403Forbidden,
                "Chỉ giáo viên (mã vai trò = 2) mới được upload tài liệu");

        // 3) Kiểm tra khóa học có tồn tại không
        var khoaHoc = await _db.KhoaHoc
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.MaKhoaHoc == dto.MaKhoaHoc);

        if (khoaHoc == null)
            return NotFound("Không tồn tại khóa học này");

        // 4) Kiểm tra khóa học có thuộc giáo viên này không
        if (khoaHoc.MaGiaoVien != dto.MaGiaoVien)
            return StatusCode(StatusCodes.Status403Forbidden,
                "Bạn không phải giáo viên của khóa học này");

        // 5) Kiểm tra trạng thái duyệt khóa học
        if (khoaHoc.TrangThaiDuyet != "DaDuyet")
            return StatusCode(StatusCodes.Status403Forbidden,
                "Khóa học chưa được duyệt nên không thể upload tài liệu");

        // 6) Kiểm tra định dạng
        var allowedExtensions = new[]
        {
        ".pdf", ".doc", ".docx", ".ppt", ".pptx",
        ".xls", ".xlsx", ".txt", ".zip", ".rar"
    };

        var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Định dạng file không được hỗ trợ");

        // 7) Lưu file
        var fileName = Guid.NewGuid().ToString("N") + extension;
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", "hoclieu");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var fullPath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        // 8) Lưu DB
        var hocLieu = new HocLieu
        {
            TieuDe = dto.TieuDe.Trim(),
            MoTa = string.IsNullOrWhiteSpace(dto.MoTa) ? null : dto.MoTa.Trim(),
            DuongDanTep = "/files/hoclieu/" + fileName,
            LoaiTep = extension.Substring(1).ToUpper(),
            KichThuocTep = dto.File.Length,
            NgayDang = DateTime.Now,

            MaKhoaHoc = dto.MaKhoaHoc,

            // lưu người upload (giáo viên)
            MaNguoiDung = dto.MaGiaoVien,

            DaDuyet = true,
            TrangThaiDuyet = "Đã duyệt"
        };

        _db.HocLieu.Add(hocLieu);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Upload tài liệu thành công",
            maHocLieu = hocLieu.MaHocLieu,
            duongDan = hocLieu.DuongDanTep
        });
    }

}



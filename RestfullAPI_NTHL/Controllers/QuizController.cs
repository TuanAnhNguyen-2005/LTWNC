// Controllers/QuizController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestfullAPI_NTHL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestfullAPI_NTHL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly NenTangDbContext _db;

        public QuizController(NenTangDbContext db)
        {
            _db = db;
        }

        // GET: api/Quiz/teacher/{maGiaoVien} - Lấy quiz của giáo viên
        [HttpGet("teacher/{maGiaoVien:int}")]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzesByTeacher(int maGiaoVien)
        {
            var quizzes = await _db.Quizzes
                .AsNoTracking()
                .Where(q => q.MaGiaoVien == maGiaoVien)
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .OrderByDescending(q => q.NgayTao)
                .ToListAsync();

            return Ok(quizzes);
        }

        // GET: api/Quiz/{id} - Chi tiết 1 quiz
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Quiz>> GetQuiz(int id)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .FirstOrDefaultAsync(q => q.MaQuiz == id);

            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        // GET: api/Quiz/published - Tất cả quiz đang Published (cho Student)
        [HttpGet("published")]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetPublishedQuizzes()
        {
            var quizzes = await _db.Quizzes
                .AsNoTracking()
                .Where(q => q.TrangThai == "Published")
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .OrderByDescending(q => q.NgayTao)
                .ToListAsync();

            return Ok(quizzes);
        }

        // POST: api/Quiz - Tạo quiz mới
        [HttpPost]
        public async Task<ActionResult<Quiz>> CreateQuiz([FromBody] QuizCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var quiz = new Quiz
            {
                TenQuiz = dto.TenQuiz,
                MoTa = dto.MoTa,
                MaKhoaHoc = dto.MaKhoaHoc,
                MaGiaoVien = dto.MaGiaoVien,
                ThoiGianLamBai = dto.ThoiGianLamBai,
                TrangThai = "Published",
                NgayTao = DateTime.Now
            };

            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();

            if (dto.CauHois != null && dto.CauHois.Any())
            {
                foreach (var chDto in dto.CauHois)
                {
                    var cauHoi = new CauHoi
                    {
                        MaQuiz = quiz.MaQuiz,
                        NoiDung = chDto.NoiDung,
                        LoaiCauHoi = chDto.LoaiCauHoi,
                        Diem = chDto.Diem
                    };

                    _db.CauHois.Add(cauHoi);
                    await _db.SaveChangesAsync();

                    if (chDto.LuaChons != null && chDto.LuaChons.Any())
                    {
                        var luaChons = chDto.LuaChons.Select(lc => new LuaChon
                        {
                            MaCauHoi = cauHoi.MaCauHoi,
                            NoiDung = lc.NoiDung,
                            LaDapAnDung = lc.LaDapAnDung
                        });

                        _db.LuaChons.AddRange(luaChons);
                    }
                }
                await _db.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.MaQuiz }, quiz);
        }

        // PUT: api/Quiz/{id} - Cập nhật quiz
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] QuizCreateDto dto)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .FirstOrDefaultAsync(q => q.MaQuiz == id);

            if (quiz == null) return NotFound();

            quiz.TenQuiz = dto.TenQuiz;
            quiz.MoTa = dto.MoTa;
            quiz.MaKhoaHoc = dto.MaKhoaHoc;
            quiz.ThoiGianLamBai = dto.ThoiGianLamBai;
            quiz.TrangThai = dto.TrangThai ?? quiz.TrangThai;

            // Xóa câu hỏi cũ
            _db.LuaChons.RemoveRange(quiz.CauHois.SelectMany(ch => ch.LuaChons));
            _db.CauHois.RemoveRange(quiz.CauHois);

            // Thêm câu hỏi mới
            if (dto.CauHois != null && dto.CauHois.Any())
            {
                foreach (var chDto in dto.CauHois)
                {
                    var cauHoi = new CauHoi
                    {
                        MaQuiz = quiz.MaQuiz,
                        NoiDung = chDto.NoiDung,
                        LoaiCauHoi = chDto.LoaiCauHoi,
                        Diem = chDto.Diem
                    };
                    _db.CauHois.Add(cauHoi);
                    await _db.SaveChangesAsync();

                    if (chDto.LuaChons != null && chDto.LuaChons.Any())
                    {
                        var luaChons = chDto.LuaChons.Select(lc => new LuaChon
                        {
                            MaCauHoi = cauHoi.MaCauHoi,
                            NoiDung = lc.NoiDung,
                            LaDapAnDung = lc.LaDapAnDung
                        });
                        _db.LuaChons.AddRange(luaChons);
                    }
                }
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Quiz/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            _db.Quizzes.Remove(quiz);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Quiz/{id}/publish - Phát hành quiz
        [HttpPatch("{id:int}/publish")]
        public async Task<IActionResult> PublishQuiz(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.TrangThai = "Published";
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Quiz/{id}/close - Đóng quiz
        [HttpPatch("{id:int}/close")]
        public async Task<IActionResult> CloseQuiz(int id)
        {
            var quiz = await _db.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();

            quiz.TrangThai = "Closed";
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Quiz/submit - Student nộp bài + chấm điểm tự động
        [HttpPost("submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz([FromBody] QuizSubmitDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var quiz = await _db.Quizzes
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .FirstOrDefaultAsync(q => q.MaQuiz == dto.MaQuiz);

            if (quiz == null) return NotFound("Quiz không tồn tại");
            if (quiz.TrangThai != "Published") return BadRequest("Quiz chưa được phát hành");

            double tongDiem = 0;
            double diemNhanDuoc = 0;
            int soCauDung = 0;

            foreach (var traLoi in dto.TraLois)
            {
                var cauHoi = quiz.CauHois.FirstOrDefault(ch => ch.MaCauHoi == traLoi.MaCauHoi);
                if (cauHoi == null) continue;

                tongDiem += cauHoi.Diem;

                bool dung = false;

                if (cauHoi.LoaiCauHoi == "MultipleChoice" || cauHoi.LoaiCauHoi == "TrueFalse")
                {
                    if (int.TryParse(traLoi.TraLoi, out int maLC))
                    {
                        dung = cauHoi.LuaChons.Any(lc => lc.MaLuaChon == maLC && lc.LaDapAnDung);
                    }
                }
                // ShortAnswer tạm cho 0 điểm (sẽ thêm chấm tự luận sau)

                if (dung)
                {
                    diemNhanDuoc += cauHoi.Diem;
                    soCauDung++;
                }
            }

            TimeSpan thoiGianLam = dto.ThoiGianKetThuc - dto.ThoiGianBatDau;

            var result = new QuizResultDto
            {
                TenQuiz = quiz.TenQuiz,
                Diem = diemNhanDuoc,
                TongDiem = tongDiem,
                SoCauDung = soCauDung,
                TongCau = quiz.CauHois.Count,
                ThoiGianLam = thoiGianLam,
                NgayNop = dto.ThoiGianKetThuc
            };

            return Ok(result);
        }
        // GET: api/Quiz/khoahoc/{maKhoaHoc}/published
        // Lấy tất cả quiz đang Published của một khóa học cụ thể (dành cho học sinh xem)
        [HttpGet("khoahoc/{maKhoaHoc:int}/published")]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzesByKhoaHocPublished(int maKhoaHoc)
        {
            var quizzes = await _db.Quizzes
                .AsNoTracking()
                .Where(q => q.MaKhoaHoc == maKhoaHoc && q.TrangThai == "Published")
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .OrderByDescending(q => q.NgayTao)
                .ToListAsync();

            return Ok(quizzes);
        }
    }

    // ==================== DTOs ====================
    public class QuizCreateDto
    {
        public string TenQuiz { get; set; } = null!;
        public string? MoTa { get; set; }
        public int? MaKhoaHoc { get; set; }
        public int MaGiaoVien { get; set; }
        public int? ThoiGianLamBai { get; set; }
        public string? TrangThai { get; set; }
        public List<CauHoiCreateDto>? CauHois { get; set; }
    }

    public class CauHoiCreateDto
    {
        public string NoiDung { get; set; } = null!;
        public string LoaiCauHoi { get; set; } = null!;
        public double Diem { get; set; } = 1; // dùng double để tránh lỗi cast từ DB
        public List<LuaChonCreateDto>? LuaChons { get; set; }
    }

    public class LuaChonCreateDto
    {
        public string NoiDung { get; set; } = null!;
        public bool LaDapAnDung { get; set; }
    }

    public class QuizSubmitDto
    {
        public int MaQuiz { get; set; }
        public int MaHocSinh { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public List<TraLoiDto> TraLois { get; set; } = new List<TraLoiDto>();
    }

    public class TraLoiDto
    {
        public int MaCauHoi { get; set; }
        public string TraLoi { get; set; } = string.Empty;
    }

    public class QuizResultDto
    {
        public string TenQuiz { get; set; } = string.Empty;
        public double Diem { get; set; }
        public double TongDiem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCau { get; set; }
        public TimeSpan ThoiGianLam { get; set; }
        public DateTime NgayNop { get; set; }
    }
}
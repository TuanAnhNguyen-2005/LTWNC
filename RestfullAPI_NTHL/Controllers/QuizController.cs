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

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .FirstOrDefaultAsync(q => q.MaQuiz == id);

            if (quiz == null)
            {
                return NotFound(new { message = "Không tìm thấy quiz." });
            }

            // Xóa kết quả làm bài (KetQuaQuiz) trước
            var ketQuaList = await _db.KetQuaQuizzes
                .Where(kq => kq.MaQuiz == id)
                .ToListAsync();

            if (ketQuaList.Any())
            {
                _db.KetQuaQuizzes.RemoveRange(ketQuaList);
                // TraLoi và TraLoiChiTiet sẽ tự xóa nhờ ON DELETE CASCADE
            }

            // Xóa quiz (cascade sẽ xóa CauHoi và LuaChon)
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

        // POST: api/Quiz/submit - Nộp bài, chấm điểm và lưu chi tiết trả lời từng câu
        [HttpPost("submit")]
        public async Task<ActionResult<QuizResultDto>> SubmitQuiz([FromBody] QuizSubmitDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lấy quiz kèm câu hỏi và lựa chọn
            var quiz = await _db.Quizzes
                .Include(q => q.CauHois)
                    .ThenInclude(ch => ch.LuaChons)
                .FirstOrDefaultAsync(q => q.MaQuiz == dto.MaQuiz);

            if (quiz == null)
                return NotFound("Quiz không tồn tại");

            if (quiz.TrangThai != "Published")
                return BadRequest("Quiz chưa được phát hành hoặc đã đóng");

            double tongDiem = 0;
            double diemNhanDuoc = 0;
            int soCauDung = 0;

            // Danh sách chi tiết trả lời để lưu vào bảng TraLoiChiTiet
            var chiTietTraLois = new List<TraLoiChiTiet>();

            foreach (var traLoiDto in dto.TraLois)
            {
                var cauHoi = quiz.CauHois.FirstOrDefault(ch => ch.MaCauHoi == traLoiDto.MaCauHoi);
                if (cauHoi == null) continue;

                tongDiem += cauHoi.Diem;

                bool dung = false;
                string noiDungTraLoi = traLoiDto.TraLoi; // Mặc định lưu nguyên string từ client

                // Xử lý trắc nghiệm (MultipleChoice hoặc TrueFalse)
                if (cauHoi.LoaiCauHoi == "MultipleChoice" || cauHoi.LoaiCauHoi == "TrueFalse")
                {
                    if (int.TryParse(traLoiDto.TraLoi, out int maLuaChon))
                    {
                        var luaChon = cauHoi.LuaChons.FirstOrDefault(lc => lc.MaLuaChon == maLuaChon);
                        if (luaChon != null)
                        {
                            dung = luaChon.LaDapAnDung;
                            noiDungTraLoi = luaChon.NoiDung; // Lưu nội dung lựa chọn thay vì ID
                        }
                    }
                }
                // Có thể mở rộng cho ShortAnswer, Essay... ở đây (tạm để dung = false)

                if (dung)
                {
                    diemNhanDuoc += cauHoi.Diem;
                    soCauDung++;
                }

                // Thêm chi tiết trả lời
                chiTietTraLois.Add(new TraLoiChiTiet
                {
                    MaCauHoi = traLoiDto.MaCauHoi,
                    TraLoi = noiDungTraLoi,
                    DungSai = dung
                });
            }

            TimeSpan thoiGianLam = dto.ThoiGianKetThuc - dto.ThoiGianBatDau;

            // Lưu kết quả tổng vào KetQuaQuiz
            var ketQua = new KetQuaQuiz
            {
                MaQuiz = dto.MaQuiz,
                MaHocSinh = dto.MaHocSinh,
                Diem = diemNhanDuoc,
                TongDiem = tongDiem,
                SoCauDung = soCauDung,
                TongCau = quiz.CauHois.Count,
                ThoiGianBatDau = dto.ThoiGianBatDau,
                ThoiGianKetThuc = dto.ThoiGianKetThuc,
                NgayNop = dto.ThoiGianKetThuc
            };

            _db.KetQuaQuizzes.Add(ketQua);
            await _db.SaveChangesAsync(); // Lưu để có MaKetQua

            // Gán MaKetQua cho các chi tiết trả lời và lưu
            foreach (var ct in chiTietTraLois)
            {
                ct.MaKetQua = ketQua.MaKetQua;
            }
            _db.TraLoiChiTiets.AddRange(chiTietTraLois);
            await _db.SaveChangesAsync();

            // Trả về kết quả cho client (trang nộp bài)
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

        // GET: api/Quiz/diem/{maHocSinh} - Lấy danh sách điểm của học sinh
        [HttpGet("diem/{maHocSinh:int}")]
        public async Task<ActionResult<IEnumerable<QuizDiemDto>>> GetDiemByHocSinh(int maHocSinh)
        {
            var results = await _db.KetQuaQuizzes
                .Where(kq => kq.MaHocSinh == maHocSinh)
                .Include(kq => kq.Quiz)
                .OrderByDescending(kq => kq.NgayNop)
                .Select(kq => new QuizDiemDto
                {
                    MaQuiz = kq.MaQuiz,
                    TenQuiz = kq.Quiz.TenQuiz,
                    Diem = kq.Diem ?? 0,
                    TongDiem = kq.TongDiem ?? 0,
                    NgayNop = kq.NgayNop ?? DateTime.Now,
                    ThoiGianLam = kq.ThoiGianKetThuc != null && kq.ThoiGianBatDau != null
                        ? (kq.ThoiGianKetThuc.Value - kq.ThoiGianBatDau.Value)
                        : TimeSpan.Zero
                })
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Quiz/diem/{maHocSinh}/{maQuiz} - Chi tiết điểm chính xác từng câu
        [HttpGet("diem/{maHocSinh:int}/{maQuiz:int}")]
        public async Task<ActionResult<QuizDiemChiTietDto>> GetDiemChiTiet(int maHocSinh, int maQuiz)
        {
            var ketQua = await _db.KetQuaQuizzes
                .Include(kq => kq.Quiz)
                    .ThenInclude(q => q.CauHois)
                        .ThenInclude(ch => ch.LuaChons)
                .FirstOrDefaultAsync(kq => kq.MaHocSinh == maHocSinh && kq.MaQuiz == maQuiz);

            if (ketQua == null)
                return NotFound("Không tìm thấy kết quả bài thi");

            // Lấy chi tiết trả lời thật từ bảng TraLoiChiTiet
            var traLois = await _db.TraLoiChiTiets
                .Where(tl => tl.MaKetQua == ketQua.MaKetQua)
                .ToDictionaryAsync(tl => tl.MaCauHoi);

            var chiTietCauHoi = ketQua.Quiz.CauHois.Select(ch =>
            {
                traLois.TryGetValue(ch.MaCauHoi, out var tl);

                // Chuẩn hóa loại câu hỏi nếu bên Teacher đang dùng TrueFalse / ShortAnswer
                var loai = (ch.LoaiCauHoi ?? "").Trim();
                if (loai.Equals("TrueFalse", StringComparison.OrdinalIgnoreCase)) loai = "SingleChoice";
                if (loai.Equals("ShortAnswer", StringComparison.OrdinalIgnoreCase)) loai = "Essay";

                // ===== ĐÁP ÁN ĐÚNG (hiển thị theo NoiDung) =====
                string dapAn = "Không có đáp án đúng";
                if (loai == "SingleChoice" || loai == "MultipleChoice")
                {
                    var luaChonDung = ch.LuaChons.FirstOrDefault(lc => lc.LaDapAnDung);
                    if (luaChonDung != null)
                        dapAn = luaChonDung.NoiDung;
                }
                else if (loai == "Essay")
                {
                    dapAn = "Câu hỏi mở - chấm thủ công";
                }

                // ===== TRẢ LỜI CỦA BẠN: ID -> NoiDung để hiển thị đúng =====
                string traLoiHienThi = tl?.TraLoi ?? "[Chưa trả lời]";

                if (loai == "SingleChoice" || loai == "MultipleChoice")
                {
                    // tl.TraLoi đang là MaLuaChon (vd: "5") => đổi sang NoiDung
                    if (tl != null && int.TryParse(tl.TraLoi, out int maLuaChonChon))
                    {
                        var luaChonChon = ch.LuaChons.FirstOrDefault(lc => lc.MaLuaChon == maLuaChonChon);
                        if (luaChonChon != null)
                            traLoiHienThi = luaChonChon.NoiDung;
                    }
                }

                return new ChiTietCauHoiDto
                {
                    NoiDung = ch.NoiDung,
                    Diem = ch.Diem,
                    TraLoi = traLoiHienThi,
                    DapAn = dapAn,
                    DungSai = tl?.DungSai ?? false
                };
            }).ToList();


            // Tính thời gian làm bài
            TimeSpan thoiGianLam = TimeSpan.Zero;
            if (ketQua.ThoiGianBatDau != null && ketQua.ThoiGianKetThuc != null)
            {
                thoiGianLam = ketQua.ThoiGianKetThuc.Value - ketQua.ThoiGianBatDau.Value;
            }
            else if (ketQua.ThoiGianBatDau != null && ketQua.NgayNop != null)
            {
                thoiGianLam = ketQua.NgayNop.Value - ketQua.ThoiGianBatDau.Value;
            }

            var result = new QuizDiemChiTietDto
            {
                MaQuiz = maQuiz,
                TenQuiz = ketQua.Quiz.TenQuiz,
                Diem = ketQua.Diem ?? 0,
                TongDiem = ketQua.TongDiem ?? 0,
                SoCauDung = ketQua.SoCauDung,
                TongCau = ketQua.TongCau,
                ThoiGianLam = thoiGianLam,
                NgayNop = ketQua.NgayNop ?? DateTime.Now,
                ChiTietCauHoi = chiTietCauHoi
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
        // GET: api/Quiz/khoahoc/{maKhoaHoc}
        [HttpGet("khoahoc/{maKhoaHoc:int}")]
        public async Task<ActionResult<IEnumerable<Quiz>>> GetQuizzesByKhoaHoc(int maKhoaHoc)
        {
            var quizzes = await _db.Quizzes
                .AsNoTracking()
                .Where(q => q.MaKhoaHoc == maKhoaHoc)
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
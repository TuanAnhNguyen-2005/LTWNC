using Microsoft.AspNetCore.Http;

namespace RestfullAPI_NTHL.Models
{
    public class HocLieuUploadDto
    {
        public string TieuDe { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public int MaKhoaHoc { get; set; }

        public int MaNguoiDung { get; set; }

        public IFormFile File { get; set; } = null!;
    }
}
using Newtonsoft.Json;
using System;

namespace MVC_STUDENT.Models
{
    public class HocLieuVm
    {
        [JsonProperty("maHocLieu")]
        public int MaHocLieu { get; set; }

        [JsonProperty("tieuDe")]
        public string TieuDe { get; set; }

        [JsonProperty("moTa")]
        public string MoTa { get; set; }

        [JsonProperty("duongDanTep")]
        public string DuongDanTep { get; set; }

        [JsonProperty("loaiTep")]
        public string LoaiTep { get; set; }

        [JsonProperty("kichThuocTep")]
        public long KichThuocTep { get; set; }

        [JsonProperty("ngayDang")]
        public DateTime? NgayDang { get; set; }
    }
}

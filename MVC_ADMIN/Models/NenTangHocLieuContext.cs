using System.Data.Entity;

namespace MVC_ADMIN.Models    // ← PHẢI LÀ MVC_ADMIN
{
    public class NenTangHocLieuContext : DbContext
    {
        public NenTangHocLieuContext() : base("name=NenTangHocLieuEntities")
        {
        }

        public virtual DbSet<HocLieu> HocLieus { get; set; }
        public virtual DbSet<ChuDe> ChuDes { get; set; }
        public virtual DbSet<MonHoc> MonHocs { get; set; }
        public virtual DbSet<LopHoc> LopHocs { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
    }
}
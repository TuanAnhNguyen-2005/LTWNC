using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RestfullAPI_NTHL.Models;

public partial class NenTangDbContext : DbContext
{
    public NenTangDbContext()
    {
    }

    public NenTangDbContext(DbContextOptions<NenTangDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BinhLuan> BinhLuan { get; set; }

    public virtual DbSet<ChuDe> ChuDe { get; set; }

    public virtual DbSet<HocLieu> HocLieu { get; set; }

    public virtual DbSet<LopHoc> LopHoc { get; set; }

    public virtual DbSet<MonHoc> MonHoc { get; set; }

    public virtual DbSet<NguoiDung> NguoiDung { get; set; }

    public virtual DbSet<VaiTro> VaiTro { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code.
        => optionsBuilder.UseSqlServer("Server=CHAODAIKA\\THAITHANHTU2340;Database=NenTangHocLieu;User Id=sa;Password=12345;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBinhLuan).HasName("PK__BinhLuan__87CB66A0947B5416");

            entity.Property(e => e.NgayDang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaBinhLuanChaNavigation).WithMany(p => p.InverseMaBinhLuanChaNavigation).HasConstraintName("FK_BinhLuan_Cha");

            entity.HasOne(d => d.MaHocLieuNavigation).WithMany(p => p.BinhLuan)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_BinhLuan_HocLieu");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BinhLuan)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BinhLuan_NguoiDung");
        });

        modelBuilder.Entity<ChuDe>(entity =>
        {
            entity.HasKey(e => e.MaChuDe).HasName("PK__ChuDe__358545110B11EDA5");
        });

        modelBuilder.Entity<HocLieu>(entity =>
        {
            entity.HasKey(e => e.MaHocLieu).HasName("PK__HocLieu__FB29E8C872F20561");

            entity.Property(e => e.DaDuyet).HasDefaultValue(false);
            entity.Property(e => e.DiemTrungBinh).HasDefaultValue(0.0);
            entity.Property(e => e.LuotXem).HasDefaultValue(0);
            entity.Property(e => e.NgayDang).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SoLuotTai).HasDefaultValue(0);
            entity.Property(e => e.TrangThaiDuyet).HasDefaultValue("Chờ duyệt");

            entity.HasOne(d => d.MaChuDeNavigation).WithMany(p => p.HocLieu)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_HocLieu_ChuDe");

            entity.HasOne(d => d.MaLopHocNavigation).WithMany(p => p.HocLieu)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_HocLieu_LopHoc");

            entity.HasOne(d => d.MaMonHocNavigation).WithMany(p => p.HocLieu)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_HocLieu_MonHoc");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.HocLieu)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_HocLieu_NguoiDung");
        });

        modelBuilder.Entity<LopHoc>(entity =>
        {
            entity.HasKey(e => e.MaLopHoc).HasName("PK__LopHoc__FEE0578487D0B26F");
        });

        modelBuilder.Entity<MonHoc>(entity =>
        {
            entity.HasKey(e => e.MaMonHoc).HasName("PK__MonHoc__4127737F8868B444");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762A068A65F");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.NguoiDung).HasConstraintName("FK_NguoiDung_VaiTro");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CF44CF560C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

using Microsoft.EntityFrameworkCore;

namespace RestFullAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("NguoiDung");
                entity.HasKey(e => e.MaNguoiDung);
                entity.Property(e => e.MaNguoiDung).HasColumnName("MaNguoiDung");
                entity.Property(e => e.HoTen).HasColumnName("HoTen").HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(100).IsRequired();
                entity.Property(e => e.MatKhau).HasColumnName("MatKhau").HasMaxLength(255).IsRequired();
                entity.Property(e => e.SoDienThoai).HasColumnName("SoDienThoai").HasMaxLength(15);
                entity.Property(e => e.DiaChi).HasColumnName("DiaChi").HasMaxLength(255);
                entity.Property(e => e.MaVaiTro).HasColumnName("MaVaiTro");
                entity.Property(e => e.NgayTao).HasColumnName("NgayTao");
                entity.Property(e => e.TrangThai).HasColumnName("TrangThai").HasDefaultValue(true);

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.MaVaiTro)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("ChuDe");
                entity.HasKey(e => e.MaChuDe);
                entity.Property(e => e.MaChuDe).HasColumnName("MaChuDe");
                entity.Property(e => e.TenChuDe).HasColumnName("TenChuDe").HasMaxLength(100).IsRequired();
                entity.Property(e => e.MoTa).HasColumnName("MoTa").HasMaxLength(255);
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("VaiTro");
                entity.HasKey(e => e.MaVaiTro);
                entity.Property(e => e.MaVaiTro).HasColumnName("MaVaiTro");
                entity.Property(e => e.TenVaiTro).HasColumnName("TenVaiTro").HasMaxLength(50).IsRequired();
            });

            // Permission - tạo bảng mới nếu chưa có
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");
                entity.HasKey(e => e.PermissionId);
                entity.Property(e => e.PermissionId).HasColumnName("PermissionId").ValueGeneratedOnAdd();
                entity.Property(e => e.PermissionName).HasColumnName("PermissionName").HasMaxLength(100).IsRequired();
                entity.Property(e => e.DisplayName).HasColumnName("DisplayName").HasMaxLength(200);
                entity.Property(e => e.Description).HasColumnName("Description").HasMaxLength(500);
                entity.Property(e => e.Module).HasColumnName("Module").HasMaxLength(50);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedDate).HasColumnName("CreatedDate");
                entity.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
            });
        }
    }
}


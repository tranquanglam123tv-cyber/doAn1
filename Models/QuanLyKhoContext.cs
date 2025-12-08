using Microsoft.EntityFrameworkCore;

namespace QuanLyKho.Models
{
    public class QuanLyKhoContext : DbContext
    {
        public QuanLyKhoContext(DbContextOptions<QuanLyKhoContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<NCC> NCCs { get; set; }
        public DbSet<KiemKeKho> KiemKeKhos { get; set; }
        public DbSet<NhapHang> NhapHangs { get; set; }
         public DbSet<LichSuGiaoDich> LichSuGiaoDichs { get; set; }

         public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }

         public DbSet<ChiTietPhieuXuat> ChiTietPhieuXuats { get; set; }
        
        public DbSet<DatHangNhap> DatHangNhaps { get; set; }
        public DbSet<HangHoa> HangHoas { get; set; }
        public DbSet<PhieuXuat> PhieuXuats { get; set; }
        public DbSet<PhieuNhap> PhieuNhaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fix cảnh báo decimal DonGia trong các entity


            modelBuilder.Entity<NhapHang>()
                .Property(p => p.DonGia)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models
{
    [Table("PhieuNhap")]
    public class PhieuNhap
    {
        // ============================
        // 1. Mã phiếu nhập
        // ============================
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string MaPN { get; set; }   // PN001

        // ============================
        // 2. Nhà cung cấp
        // ============================
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string MaNCC { get; set; }

        [ForeignKey("MaNCC")]
        public NCC NhaCungCap { get; set; }

        // ============================
        // 3. Ngày nhập
        // ============================
        [Column(TypeName = "date")]
        public DateTime? NgayNhap { get; set; } = DateTime.Now;

        // ============================
        // 4. Người tạo (Nhân viên)
        // ============================
        [Column(TypeName = "varchar(20)")]
        public string MaNV { get; set; }
        public Employee NhanVien { get; set; }

        // ============================
        // 5. Tổng giá trị
        // ============================
        [Column(TypeName = "decimal(18,0)")]
        public decimal? TongGiaTri { get; set; } = 0;

        // ============================
        // 6. Ghi chú (tùy chọn)
        // ============================
        [Column(TypeName = "nvarchar(255)")]
        public string? GhiChu { get; set; }

        // ============================
        // 7. Liên kết với phiếu đặt hàng (tùy chọn)
        // ============================
        [Column(TypeName = "varchar(20)")]
        public string? MaPhieuDat { get; set; }

        [ForeignKey("MaPhieuDat")]
        public DatHangNhap? DatHang { get; set; }
        public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhap { get; set; }
    }
}
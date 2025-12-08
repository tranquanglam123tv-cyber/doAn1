using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models
{
    [Table("PhieuXuat")]
    public class PhieuXuat
    {
        // ============================
        // 1. Mã phiếu xuất
        // ============================
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string MaPX { get; set; }   // PX001

        // ============================
        // 2. Nhà cung cấp
        // ============================
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string MaNCC { get; set; }

        [ForeignKey("MaNCC")]
        public NCC NhaCungCap { get; set; }

        // ============================
        // 3. Ngày xuất
        // ============================
        [Column(TypeName = "date")]
        public DateTime? NgayXuat { get; set; } = DateTime.Now;

        // ============================
        // 4. Người tạo (Nhân viên)
        // ============================
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string MaNV { get; set; }

        [ForeignKey("MaNV")]
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
        // 7. Optional: Liên kết phiếu đặt hàng
        // ============================
        public ICollection<ChiTietPhieuXuat> ChiTietPhieuXuat { get; set; }
        // public string? MaDatHang { get; set; }
        // [ForeignKey("MaDatHang")]
        // public DatHang DatHang { get; set; }
    }
}
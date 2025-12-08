using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models
{
    [Table("DatHangNhap")]
    public class DatHangNhap
    {
        // ======================================
        // 1. Khóa chính
        // ======================================
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string MaPhieuDat { get; set; }   // PO00452


        // ======================================
        // 2. Thời gian đặt
        // ======================================
        [Required]
        public DateTime ThoiGian { get; set; }


        // ======================================
        // 3. Nhà cung cấp (FK)
        // ======================================
        [Required]
        [Column(TypeName = "varchar(20)")]
        public string MaNCC { get; set; }

        [ForeignKey("MaNCC")]
        public NCC NhaCungCap { get; set; }


        // ======================================
        // 4. Ngày nhập dự kiến
        // ======================================
        [Required]
        public DateTime NgayNhapDuKien { get; set; }


        // ======================================
        // 5. Số ngày chờ
        // ======================================
        public int SoNgayCho { get; set; }


        // ======================================
        // 6. Tổng tiền
        // ======================================
        [Required]
        [Column(TypeName = "decimal(18, 0)")]
        public decimal TongTien { get; set; }


        // ======================================
        // 7. Trạng thái
        // ======================================
        [Required]
        [MaxLength(50)]
        public string TrangThai { get; set; }
        // "Phiếu tạm" | "Đã xác nhận NCC" | "Hoàn tất"


        // ======================================
        // 8. Liên kết với Phiếu Nhập (1 - 1)
        // ======================================
        public PhieuNhap PhieuNhap { get; set; }
    }
}
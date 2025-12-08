using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models
{
    [Table("LichSuGiaoDich")]
    public class LichSuGiaoDich
    {
        [Key]
        [Column(TypeName = "varchar(20)")]
        public string MaGiaoDich { get; set; }   // GD-00045

        [Column(TypeName = "varchar(20)")]
        public string MaThamChieu { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string LoaiGiaoDich { get; set; } // Nhập hàng, Xuất hàng...

        [Required]
        public DateTime ThoiGian { get; set; }   // 04/11/2025 09:20

        [Column(TypeName = "nvarchar(150)")]
        public string DoiTac { get; set; }


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTri { get; set; }      // 32500000

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string TrangThai { get; set; }    // Hoàn tất / Đang giao / Hủy
    }
}
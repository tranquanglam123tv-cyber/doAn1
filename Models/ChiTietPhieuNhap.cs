// Models/ChiTietPhieuNhap.cs (Tạo mới)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models
{
    [Table("ChiTietPhieuNhap")]
    public class ChiTietPhieuNhap
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string MaPN { get; set; }
        
        public HangHoa HangHoa { get; set; } 

        [ForeignKey("MaPN")]
        public PhieuNhap PhieuNhap { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string MaHH { get; set; } // Mã Hàng Hóa (MaHang trong HangHoa.cs)

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal DonGiaNhap { get; set; }
        
        [Column(TypeName = "decimal(18,0)")]
        public decimal ThanhTien { get; set; } // SoLuong * DonGiaNhap
    }
}
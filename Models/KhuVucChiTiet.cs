using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models
{
    public class KhuVucChiTiet
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string TenKhoCha { get; set; } // Kho cha (e.g., KHO XE)

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string TenKhuVuc { get; set; } // Tên khu vực cụ thể (e.g., Bãi Xe Hoàn Chỉnh)

        public int SoOToiDa { get; set; } // Sức chứa tối đa (Slots)

    }
}
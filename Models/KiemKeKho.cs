using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models
{
    public class KiemKeKho
    {
        [Key]
        public int Id { get; set; }

        // ---- FOREIGN KEY CHUẨN → trỏ qua HangHoa.Id ----
        [Required]
        public int HangHoaId { get; set; }

        [ForeignKey(nameof(HangHoaId))]
        public HangHoa HangHoa { get; set; }
        // -------------------------------------------------

        // Nếu vẫn cần lưu mã hàng để show ra bảng
        [StringLength(20)]
        public string MaHang { get; set; }

        public string Kho { get; set; }
        public int TonHeThong { get; set; }
        public int TonThucTe { get; set; }

        public DateTime NgayKiemKe { get; set; }

        [NotMapped]
        public int ChenhLech => TonThucTe - TonHeThong;
    }
}

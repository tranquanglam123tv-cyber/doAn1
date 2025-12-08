using System.ComponentModel.DataAnnotations;

namespace QuanLyKho.Models
{
    public class NhanVien
    {
        [Key]
        public int NhanVienId { get; set; }

        [Required]
        public string HoTen { get; set; }

        public string ChucVu { get; set; }

        public string SDT { get; set; }

        public string Email { get; set; }
    }
}

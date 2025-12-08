namespace QuanLyKho.Models
{
    public class NhapHang
    {
        public int Id { get; set; }
        public int HangHoaId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public DateTime NgayNhap { get; set; }

        public HangHoa? HangHoa { get; set; }
    }
}

// Models/PhieuNhapCreateModel.cs (Tạo mới)
using System.Collections.Generic;

namespace QuanLyKho.Models
{
    // DTO cho thông tin chi tiết từng sản phẩm trong phiếu nhập
    public class ChiTietPhieuNhapModel
    {
        public string MaHH { get; set; }  // Mã hàng hóa (MaHH)
        public int Sl { get; set; }     // Số lượng
        public decimal Dg { get; set; } // Đơn giá
        public decimal ThanhTien { get; set; }
    }

    // DTO chính cho POST request tạo phiếu nhập
    public class PhieuNhapCreateModel // DTO chính cho form
{
    public string MaNCC { get; set; }
    public string NgayNhap { get; set; } 
    public decimal TongTienHang { get; set; }
    public decimal TongGiaTri { get; set; } 
    public decimal TienDaThanhToan { get; set; } 
    public decimal TienConNo { get; set; }
        
    public string GhiChu { get; set; }
    public List<ChiTietPhieuNhapModel> ChiTiet { get; set; }
}
}
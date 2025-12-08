using System.Collections.Generic;

namespace QuanLyKho.Models
{
    // DTO cho thông tin chi tiết từng sản phẩm trong phiếu xuất
    public class ChiTietPhieuXuatModel
    {
        public string MaHH { get; set; }  // Mã hàng hóa
        public int Sl { get; set; }     // Số lượng
        public decimal Dg { get; set; } // Đơn giá xuất (Đơn giá này sẽ được mapping vào DonGiaNhap trong DB)
    }

    // DTO chính cho POST request tạo phiếu xuất
    public class PhieuXuatCreateModel // DTO chính cho form
    {
        // FIX: Bổ sung MaPX để khắc phục lỗi CS1061
        public string MaPX { get; set; } 

        public string MaNCC { get; set; } 
        
        // NgayNhap tương ứng với NgayXuat trong DB model (khắc phục lỗi CS1061 NgayXuat)
        public string NgayNhap { get; set; } 
        
        public decimal TongTienHang { get; set; } // Tổng tiền trước giảm giá
        
        // FIX: Bổ sung GiamGia để khắc phục lỗi CS1061
        public decimal GiamGia { get; set; }
        
        public string GhiChu { get; set; }
        
        public List<ChiTietPhieuXuatModel> ChiTiet { get; set; }
    }
}
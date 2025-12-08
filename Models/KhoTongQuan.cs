using System.Collections.Generic;

namespace QuanLyKho.Models
{
    public class KhoTongQuan
    {
        public required string TenKho { get; set; }

        public required string Icon { get; set; }

        public required string MauChuDao { get; set; }

        public int TongSoOToiDa { get; set; }
        public int TongSoOHienTaiDaSuDung { get; set; }
        public List<KhuVucChiTiet> DanhSachKhuVuc { get; set; } = new List<KhuVucChiTiet>(); 
    }
}
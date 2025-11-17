// File: Controllers/SanPhamController.cs

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Cần dùng List
using Test.Models; // Quan trọng: Đảm bảo đúng namespace của Model

namespace Test.Controllers
{
    public class SanPhamController : Controller
    {
        public IActionResult Index() // Action này sẽ hiển thị trang danh sách
        {
            // 1. TẠO DỮ LIỆU MẪU (Bình thường, bạn sẽ gọi hàm lấy dữ liệu từ Database ở đây)
            var danhSachSanPham = new List<SanPhamModel>
            {
                new SanPhamModel { 
                    MaSP = 1, 
                    TenSP = "Laptop Gaming X", 
                    SoLuongTon = 15, 
                    GiaBan = 25000000 
                },
                new SanPhamModel { 
                    MaSP = 2, 
                    TenSP = "Màn Hình 27\" LED", 
                    SoLuongTon = 42, 
                    GiaBan = 4500000 
                },
                new SanPhamModel { 
                    MaSP = 3, 
                    TenSP = "Chuột Logitech G102", 
                    SoLuongTon = 105, 
                    GiaBan = 550000 
                },
            };

            // 2. TRUYỀN DANH SÁCH SANG VIEW
            return View(danhSachSanPham);
        }
    }
}
// XuatHangController.cs (ĐÃ CẬP NHẬT: Khắc phục tất cả lỗi biên dịch và đồng bộ Model)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyKho.Controllers
{
    public class XuatHangController : Controller
    {
        private readonly QuanLyKhoContext _context;

        public XuatHangController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // Trang danh sách phiếu xuất (INDEX - GET)
        public IActionResult Index()
        {
            var danhSachPhieuXuat = _context.PhieuXuats
                                            .Include(px => px.NhanVien)   
                                            .Include(px => px.NhaCungCap)
                                            .OrderByDescending(px => px.NgayXuat)
                                            .Include(px => px.ChiTietPhieuXuat)
                                            .ToList();
            return View(danhSachPhieuXuat); 
        }
        [HttpGet]
        public IActionResult Create()
        {
            var lastMaPX = _context.PhieuXuats.OrderByDescending(p => p.MaPX).Select(p => p.MaPX).FirstOrDefault();
            var nextMaPX = GenerateNextMaPX(lastMaPX); 
            var model = new PhieuXuatCreateModel();
            ViewBag.NhaCungCaps = _context.NCCs.ToList();
            ViewBag.NextMaPX = nextMaPX;
            return View(model); 
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PhieuXuatCreateModel model)
        {
            string maPX = "";
            if(model != null && !string.IsNullOrEmpty(model.MaPX))
            {
                 maPX = model.MaPX;
            }
            // Fallback: Nếu không có MaPX từ client, tự tạo.
            if (string.IsNullOrEmpty(maPX))
            {
                var lastMaPX = _context.PhieuXuats.OrderByDescending(p => p.MaPX).Select(p => p.MaPX).FirstOrDefault();
                maPX = GenerateNextMaPX(lastMaPX); 
            }
            // Kiểm tra trùng lặp MaPX
            if (_context.PhieuXuats.Any(px => px.MaPX == maPX))
            {
                var lastMaPX = _context.PhieuXuats.OrderByDescending(p => p.MaPX).Select(p => p.MaPX).FirstOrDefault();
                maPX = GenerateNextMaPX(lastMaPX); 
            }
            
            if (model == null || model.ChiTiet == null || !model.ChiTiet.Any())
            {
                return Json(new { success = false, message = "Phiếu xuất không có chi tiết hàng hóa." });
            }

            var currentUserId = "NV001"; 
            DateTime? ngayXuat = null;
            if (DateTime.TryParse(model.NgayNhap, out DateTime parsedDate)) 
            {
                ngayXuat = parsedDate;
            }
            else
            {
                ngayXuat = DateTime.Now; 
            }

            var phieuXuat = new PhieuXuat
            {
                MaPX = maPX, 
                MaNCC = model.MaNCC, 
                NgayXuat = ngayXuat, 
                MaNV = currentUserId, 
                TongGiaTri = model.TongTienHang - model.GiamGia, 
                GhiChu = model.GhiChu
            };
            _context.PhieuXuats.Add(phieuXuat);
            foreach (var item in model.ChiTiet)
            {
                var chiTiet = new ChiTietPhieuXuat 
                {
                    MaPX = maPX, 
                    MaHH = item.MaHH,
                    SoLuong = item.Sl, 
                    DonGiaNhap = item.Dg, // item.Dg là đơn giá bán/xuất
                    ThanhTien = item.Sl * item.Dg // ThanhTien khớp với ChiTietPhieuXuat.cs
                };
                _context.ChiTietPhieuXuats.Add(chiTiet); 

                // Cập nhật tồn kho 
                var product = await _context.HangHoas.FirstOrDefaultAsync(p => p.MaHang == item.MaHH);
                if (product != null)
                {
                    product.TonKho -= item.Sl; 
                }
            }
            
            await _context.SaveChangesAsync();

            return Json(new { success = true, maPhieuXuat = maPX });
        }
        
        // API - Tìm kiếm hàng hóa
        [HttpGet]
        public IActionResult SearchProduct(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>()); 
            }
            
            var searchTerm = term.Trim().ToUpper();
            
            var products = _context.HangHoas 
                .Where(p => p.MaHang.ToUpper().Contains(searchTerm) || p.TenHang.ToUpper().Contains(searchTerm))
                .Select(p => new // Anonymous Type
                {
                    MaHH = p.MaHang,
                    TenHH = p.TenHang,
                    TonKho = p.TonKho, 
                    GiaBan = p.GiaBan,
                })
                .Take(10)
                .ToList();

            return Json(products);
        }
        
        // Trang chi tiết phiếu xuất (DETAILS - GET)
        public IActionResult Details(string id)
        {
            ViewBag.MaPhieu = id;
            return View();
        }

        // HÀM HỖ TRỢ: Tạo Mã Phiếu Tự động (Giữ nguyên)
        private string GenerateNextMaPX(string lastMaPX)
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            int nextNumber = 1;
            
            if (!string.IsNullOrEmpty(lastMaPX) && lastMaPX.StartsWith("PX-"))
            {
                var parts = lastMaPX.Split('-');
                if (parts.Length == 3 && parts[1] == today)
                    if (int.TryParse(parts[2], out int currentNumber))
                        nextNumber = currentNumber + 1;
            }
            return $"PX-{today}-{nextNumber.ToString("D3")}";
        }
    }
}
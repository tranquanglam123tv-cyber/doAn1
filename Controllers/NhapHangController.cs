
using Microsoft.AspNetCore.Mvc;
using QuanLyKho.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace QuanLyKho.Controllers
{
    public class NhapHangController : Controller
    {
        private readonly QuanLyKhoContext _context;

        public NhapHangController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // =========================================================
        // 1. ACTION INDEX (GET)
        // =========================================================
        public IActionResult Index()
        {
            var phieuNhaps = _context.PhieuNhaps
            .Include(p => p.NhaCungCap)
            .Include(p => p.NhanVien)
            .OrderByDescending(px => px.NgayNhap)
            .ToList();
            return View(phieuNhaps);
        }

        // =========================================================
        // 2. ACTION CREATE (GET)
        // =========================================================
        public IActionResult Create()
        {
            ViewBag.NhaCungCaps = _context.NCCs.ToList();
            return View(new PhieuNhapCreateModel()); 
        }

        // =========================================================
        // 3. ACTION CREATE (POST) - Thêm phiếu nhập và cập nhật tồn kho/giá vốn
        // =========================================================
        [HttpPost]
        public IActionResult Create([FromBody] PhieuNhapCreateModel model)
        {
            
            // Dù request là JSON, vẫn nên kiểm tra cơ bản
            if (model == null || model.ChiTiet == null || !model.ChiTiet.Any())
            {
                return Json(new { success = false, message = "Dữ liệu phiếu nhập không được trống." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // TÍNH TOÁN TỔNG GIÁ TRỊ PHIẾU NHẬP TRÊN SERVER (Đảm bảo tính nhất quán)
                    decimal totalCalculated = model.ChiTiet.Sum(ct => ct.Sl * ct.Dg); 

                    // ----------------------------------------------------
                    // LƯU Ý: Nếu hệ thống xác thực chưa có hoặc trả về null, sử dụng giá trị mặc định "MANV001".
                    string maNhanVien = User.Identity.Name; 
                    if (string.IsNullOrEmpty(maNhanVien))
                    {
                        maNhanVien = "MANV001"; // Giá trị MaNV mặc định/tạm thời (PHẢI TỒN TẠI TRONG BẢNG NhanVien)
                    }
                    // ----------------------------------------------------

                    // 1. Tự động tạo mã PN mới (Ví dụ: PN0001)
                    var lastMaPN = _context.PhieuNhaps.OrderByDescending(p => p.MaPN).Select(p => p.MaPN).FirstOrDefault();
                    int nextIdPN = 1;
                    if (!string.IsNullOrEmpty(lastMaPN) && lastMaPN.StartsWith("PN") && int.TryParse(lastMaPN.Substring(2), out int currentId)) 
                        nextIdPN = currentId + 1;
                    string newMaPN = "PN" + nextIdPN.ToString("D4");

                    // 2. Parse Ngày Nhập
                    DateTime? ngayNhapValue = null;
                    if (!string.IsNullOrEmpty(model.NgayNhap))
                    {
                        if (DateTime.TryParseExact(model.NgayNhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                        {
                            ngayNhapValue = parsedDate.Date;
                        }
                    }
                    
                    // 3. Tạo đối tượng PhieuNhap
                    var phieuNhap = new PhieuNhap
                    {
                        MaPN = newMaPN, 
                        MaNCC = model.MaNCC, 
                        NgayNhap = ngayNhapValue.HasValue ? ngayNhapValue.Value : DateTime.Now.Date,
                        TongGiaTri = totalCalculated, // <<< SỬ DỤNG GIÁ TRỊ TÍNH TOÁN TỪ SERVER
                        GhiChu = model.GhiChu,
                        MaNV = maNhanVien
                    };
                    _context.PhieuNhaps.Add(phieuNhap);
                    
                    // 4. Lặp qua chi tiết, cập nhật tồn kho/giá vốn và tạo ChiTietPhieuNhap
                    foreach (var chiTietDto in model.ChiTiet)
                    {
                        var hangHoa = _context.HangHoas.FirstOrDefault(h => h.MaHang == chiTietDto.MaHH);
                        
                        if (hangHoa != null)
                        {
                            // Cập nhật tồn kho
                            hangHoa.TonKho += chiTietDto.Sl;
                            
                            // Cập nhật giá vốn - LƯU Ý: Đây là phương pháp Giá Vốn Nhập Sau Cùng (LIC)
                            hangHoa.GiaVon = chiTietDto.Dg; 
                            
                            // Nếu muốn dùng Giá Vốn Trung Bình, cần tính toán phức tạp hơn:
                            // decimal oldTotalCost = hangHoa.TonKho * hangHoa.GiaVon;
                            // decimal newTotalCost = oldTotalCost + (chiTietDto.Sl * chiTietDto.Dg);
                            // hangHoa.GiaVon = newTotalCost / (hangHoa.TonKho + chiTietDto.Sl);

                            _context.HangHoas.Update(hangHoa);
                        }
                        else
                        {
                            // Hàng hóa CHƯA TỒN TẠI: TẠO MỚI bản ghi HangHoa
                            // LƯU Ý: Nên có logic kiểm tra tính hợp lệ của MaHang, TenHang nếu nó được điền từ form.
                            hangHoa = new HangHoa 
                            {
                                MaHang = chiTietDto.MaHH,
                                TenHang = "Hàng hóa mới: " + chiTietDto.MaHH, 
                                LoaiHang = "Chưa rõ", 
                                GiaBan = chiTietDto.Dg * 1.2M, 
                                GiaVon = chiTietDto.Dg,
                                TonKho = chiTietDto.Sl,
                                ThoiGianTao = DateTime.Now
                            };
                            _context.HangHoas.Add(hangHoa); 
                        }

                        // b. TẠO VÀ THÊM CHI TIẾT PHIẾU NHẬP MỚI
                        var chiTiet = new ChiTietPhieuNhap 
                        {
                            MaPN = newMaPN, MaHH = chiTietDto.MaHH, SoLuong = chiTietDto.Sl,
                            DonGiaNhap = chiTietDto.Dg, ThanhTien = chiTietDto.Sl * chiTietDto.Dg 
                        };
                        _context.ChiTietPhieuNhaps.Add(chiTiet);
                    }
                    
                    // 5. CẬP NHẬT TỔNG MUA CỦA NCC
                    var ncc = _context.NCCs.FirstOrDefault(n => n.MaNCC == model.MaNCC);
                    if (ncc != null) 
                    {
                        ncc.TongMua += totalCalculated; // <<< SỬ DỤNG GIÁ TRỊ TÍNH TOÁN TRÊN SERVER
                        _context.NCCs.Update(ncc); 
                    }


                    // 6. Thêm giao dịch vào lịch sử (Ví dụ: GD-00001)
                    var lastMaGD = _context.LichSuGiaoDichs.OrderByDescending(g => g.MaGiaoDich).Select(g => g.MaGiaoDich).FirstOrDefault();
                    int nextGdId = 1;
                    if (!string.IsNullOrEmpty(lastMaGD) && lastMaGD.StartsWith("GD-") && int.TryParse(lastMaGD.Substring(3), out int currentGdId)) 
                    {
                        nextGdId = currentGdId + 1;
                    }
                    string newMaGD = "GD-" + nextGdId.ToString("D5");
                    
                    var giaoDich = new LichSuGiaoDich
                    {
                        MaGiaoDich = newMaGD, 
                        MaThamChieu = newMaPN, 
                        LoaiGiaoDich = "Nhập hàng",
                        ThoiGian = DateTime.Now, 
                        DoiTac = ncc?.TenNCC ?? "N/A",
                        GiaTri = totalCalculated, // <<< SỬ DỤNG GIÁ TRỊ TÍNH TOÁN TRÊN SERVER
                        TrangThai = "Hoàn thành" 
                    };
                    _context.LichSuGiaoDichs.Add(giaoDich);
                    
                    // 7. LƯU TẤT CẢ THAY ĐỔI trong một transaction
                    _context.SaveChanges(); 

                    return Json(new { success = true, message = "Thêm phiếu nhập thành công.", maPN = newMaPN });
                }
                catch (DbUpdateException ex)
                {
                    var innerExceptionMessage = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message;
                    // Log the full exception detail for server-side debugging
                    Console.WriteLine("DbUpdateException: " + ex.ToString());
                    return Json(new { success = false, message = "Lỗi Database: Kiểm tra khóa ngoại, giá trị NULL hoặc kiểu dữ liệu. Chi tiết: " + (innerExceptionMessage ?? ex.Message) });
                }
                catch (Exception ex)
                {
                    // Log the full exception detail for server-side debugging
                    Console.WriteLine("General Exception: " + ex.ToString());
                    return Json(new { success = false, message = "Lỗi khi lưu dữ liệu: " + ex.Message });
                }
            }
            
            // Trả về JSON lỗi khi ModelState không hợp lệ
            var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors }).ToList();
            return Json(new { success = false, message = "Dữ liệu đầu vào không hợp lệ.", errors = errors });
        }

        // =========================================================
        // 4. ACTION SEARCH HÀNG HÓA (AJAX) - Dùng cho Select2/Autocomplete
        // =========================================================
        [HttpGet]
        public IActionResult SearchHangHoa(string term)
        {
            // Kiểm tra và chuẩn hóa chuỗi tìm kiếm
            if (string.IsNullOrEmpty(term))
            {
                // Trả về một danh sách nhỏ các hàng hóa gần đây hoặc trống
                var initialResults = _context.HangHoas
                    .OrderByDescending(h => h.ThoiGianTao)
                    .Take(10) // Lấy 10 mục gần nhất
                    .Select(h => new
                    {
                        id = h.MaHang,
                        text = $"{h.MaHang} - {h.TenHang} (Tồn: {h.TonKho})",
                        maHang = h.MaHang,
                        tenHang = h.TenHang,
                        loaiHang = h.LoaiHang,
                        giaVon = h.GiaVon,
                        tonKho = h.TonKho
                    }).ToList();
                
                return Json(new { results = initialResults });
            }
            var searchTerm = term.Trim().ToUpper();
            var results = _context.HangHoas
                .Where(h => h.MaHang.ToUpper().Contains(searchTerm) || h.TenHang.ToUpper().Contains(searchTerm))
                .Take(50) // Giới hạn kết quả trả về để tối ưu hiệu suất
                .Select(h => new
                {
                    id = h.MaHang, // BẮT BUỘC phải là 'id' (theo chuẩn Select2)
                    text = $"{h.MaHang} - {h.TenHang} (Tồn: {h.TonKho})", // BẮT BUỘC phải là 'text' (theo chuẩn Select2)
                    maHang = h.MaHang,
                    tenHang = h.TenHang,
                    loaiHang = h.LoaiHang,
                    giaVon = h.GiaVon,
                    tonKho = h.TonKho
                })
                .ToList();
                return Json(new { results = results });
        }

    } 
}

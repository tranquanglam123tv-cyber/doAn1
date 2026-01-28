using Microsoft.AspNetCore.Mvc;
using QuanLyKho.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.Transactions; // Cần dùng để quản lý Transaction thủ công nếu không dùng transaction của EF Core

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
            .Include(p => p.NhaCungCap) // Đổi tên thành NCC nếu không khớp
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
            ViewBag.NhaCungCaps = _context.NCCs.ToList(); // Đảm bảo tên Entity là NCCs
            return View(new PhieuNhapCreateModel()); 
        }

        // =========================================================
        // 3. ACTION CREATE (POST) - Thêm phiếu nhập và cập nhật tồn kho/giá vốn
        // SỬ DỤNG DB TRANSACTION ĐỂ ĐẢM BẢO TÍNH TOÀN VẸN
        // =========================================================
        [HttpPost]
        public IActionResult Create([FromBody] PhieuNhapCreateModel model)
        {
            if (model == null || model.ChiTiet == null || !model.ChiTiet.Any())
            {
                return Json(new { success = false, message = "Dữ liệu phiếu nhập không được trống." });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors }).ToList();
                return Json(new { success = false, message = "Dữ liệu đầu vào không hợp lệ.", errors = errors });
            }

            // Bắt đầu Transaction để đảm bảo tất cả thay đổi được lưu hoặc không lưu
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // TÍNH TOÁN TỔNG GIÁ TRỊ PHIẾU NHẬP TRÊN SERVER
                    decimal totalCalculated = model.ChiTiet.Sum(ct => ct.Sl * ct.Dg); 

                    string maNhanVien = User.Identity.Name; 
                    if (string.IsNullOrEmpty(maNhanVien))
                    {
                        maNhanVien = "E0013"; 
                    }

                    // 1. Tự động tạo mã PN mới (Ví dụ: PN0001)
                    var lastMaPN = _context.PhieuNhaps.OrderByDescending(p => p.MaPN).Select(p => p.MaPN).FirstOrDefault();
                    int nextIdPN = 1;
                    if (!string.IsNullOrEmpty(lastMaPN) && lastMaPN.StartsWith("PN") && int.TryParse(lastMaPN.Substring(2), out int currentId)) 
                        nextIdPN = currentId + 1;
                    string newMaPN = "PN" + nextIdPN.ToString("D4");

                    // 2. Parse Ngày Nhập
                    DateTime? ngayNhapValue = null;
                    if (!string.IsNullOrEmpty(model.NgayNhap) && DateTime.TryParseExact(model.NgayNhap, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        ngayNhapValue = parsedDate.Date;
                    }
                    
                    // 3. Tạo đối tượng PhieuNhap
                    var phieuNhap = new PhieuNhap
                    {
                        MaPN = newMaPN, 
                        MaNCC = model.MaNCC, 
                        NgayNhap = ngayNhapValue.HasValue ? ngayNhapValue.Value : DateTime.Now.Date,
                        TongGiaTri = totalCalculated,
                        GhiChu = model.GhiChu,
                        MaNV = maNhanVien,

                    };
                    _context.PhieuNhaps.Add(phieuNhap);
                    
                    // 4. Lặp qua chi tiết, cập nhật tồn kho/giá vốn và tạo ChiTietPhieuNhap
                    foreach (var chiTietDto in model.ChiTiet)
                    {
                        var hangHoa = _context.HangHoas.FirstOrDefault(h => h.MaHang == chiTietDto.MaHH);
                        
                        if (hangHoa != null)
                        {
                            // Cập nhật tồn kho và Giá Vốn (LIC)
                            hangHoa.TonKho += chiTietDto.Sl;
                            hangHoa.GiaVon = chiTietDto.Dg; 
                            _context.HangHoas.Update(hangHoa);
                        }
                        else
                        {
                            // Hàng hóa CHƯA TỒN TẠI: TẠO MỚI bản ghi HangHoa
                            hangHoa = new HangHoa 
                            {
                                MaHang = chiTietDto.MaHH,
                                TenHang = chiTietDto.MaHH, // Đặt tên tạm là Mã hàng (cần sửa lại sau)
                                LoaiHang = "Chưa rõ", 
                                GiaBan = chiTietDto.Dg * 1.2M, // Giá bán mặc định
                                GiaVon = chiTietDto.Dg,
                                TonKho = chiTietDto.Sl,
                                ThoiGianTao = DateTime.Now
                            };
                            _context.HangHoas.Add(hangHoa); 
                        }

                        // TẠO VÀ THÊM CHI TIẾT PHIẾU NHẬP MỚI
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
                        ncc.TongMua += totalCalculated; 
                        _context.NCCs.Update(ncc); 
                    }

                    // 6. Thêm giao dịch vào lịch sử
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
                        GiaTri = totalCalculated, 
                        TrangThai = model.TienConNo > 0 ? "Công nợ" : "Đã thanh toán"
                    };
                    _context.LichSuGiaoDichs.Add(giaoDich);

                    // 7. LƯU VÀ COMMIT
                    _context.SaveChanges(); 
                    transaction.Commit(); // Commit transaction khi tất cả đã thành công

                    return Json(new { success = true, message = "Thêm phiếu nhập thành công.", maPN = newMaPN });
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); // Rollback nếu có lỗi
                    Console.WriteLine("Lỗi khi tạo Phiếu Nhập: " + ex.ToString());
                    return Json(new { success = false, message = "Lỗi khi lưu dữ liệu. Chi tiết: " + ex.Message });
                }
            }
        }

        // =========================================================
        // 4. ACTION SEARCH HÀNG HÓA (AJAX) - ĐỔI TÊN THÀNH SearchProduct
        // =========================================================
        [HttpGet]
        public IActionResult SearchProduct(string term)
        {
            var searchTerm = string.IsNullOrEmpty(term) ? "" : term.Trim().ToUpper();
            
            var query = _context.HangHoas.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(h => h.MaHang.ToUpper().Contains(searchTerm) || h.TenHang.ToUpper().Contains(searchTerm));
            }
            else
            {
                 // Nếu không có term, trả về 10 mục gần nhất
                 query = query.OrderByDescending(h => h.ThoiGianTao).Take(10);
            }

            var results = query
                .Take(50) // Giới hạn kết quả trả về
                .Select(h => new
                {
                    // LƯU Ý: Đổi tên các trường này để khớp với logic client-side của bạn
                    id = h.MaHang,
                    text = $"{h.MaHang} - {h.TenHang}", // Select2 sẽ hiển thị text này mặc định
                    maHang = h.MaHang,
                    tenHang = h.TenHang,
                    loaiHang = h.LoaiHang,
                    giaVon = h.GiaVon,
                    // Nếu muốn client dùng GiaBan làm giá mặc định:
                    giaBan = h.GiaBan, 
                    tonKho = h.TonKho
                })
                .ToList();
            
            // Trả về trực tiếp list object. Client sẽ tự map id, text
            // Bạn cần đảm bảo JS của bạn trong hàm processResults đang mong đợi một mảng (list)
            return Json(results); 
        }
        
        // =========================================================
        // 5. ACTION DETAILS (GET)
        // =========================================================
        // (Bạn nên thêm logic này nếu muốn xem chi tiết phiếu nhập)
        // [HttpGet]
        // public IActionResult Details(string id) { /* ... */ }

        // =========================================================
        // 6. ACTION EDIT (GET/POST)
        // =========================================================
        // (Bạn nên thêm logic này nếu muốn sửa phiếu nhập)
        // [HttpGet]
        // public IActionResult Edit(string id) { /* ... */ }
        // [HttpPost]
        // public IActionResult Edit(string id, [FromBody] PhieuNhapCreateModel model) { /* ... */ }

        // =========================================================
        // 7. ACTION DELETE (POST)
        // =========================================================
        // (Bạn nên thêm logic này nếu muốn xóa phiếu nhập)
        // [HttpPost, ActionName("Delete")]
        // public IActionResult DeleteConfirmed(string id) { /* ... */ }

    } 
}
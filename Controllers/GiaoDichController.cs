// File: GiaoDichController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyKho.Models;
using System; // Cần thiết cho DateTime
using System.Linq; // Cần thiết cho Linq (OrderByDescending, Max, FirstOrDefault...)
using System.Collections.Generic; // Cần thiết cho List<NCC> và List<DatHangNhap> (Dùng trong logic tạo dữ liệu mẫu)

namespace QuanLyKho.Controllers
{
    public class GiaoDichController : Controller
    {
        private readonly QuanLyKhoContext _context;

        public GiaoDichController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // ===============================
        // 1. Danh sách Đặt Hàng Nhập (PO)
        // ===============================
        public IActionResult DatHangNhap()
        {
            // Lấy dữ liệu thật từ DB
            var datHang = _context.DatHangNhaps
                .Include(d => d.NhaCungCap)
                .OrderByDescending(d => d.ThoiGian)
                .ToList();

            var phieuNhaps = _context.PhieuNhaps
                .Include(p => p.NhaCungCap)
                .OrderByDescending(p => p.NgayNhap)
                .ToList();

            var nccs = _context.NCCs.ToList();

            // Nếu DB trống, tạo dữ liệu mẫu để test hiển thị
            if (!datHang.Any())
            {
                if (!nccs.Any())
                {
                    nccs = new List<NCC>
                    {
                        new NCC { MaNCC = "NCC01", TenNCC = "Công ty ABC" },
                        new NCC { MaNCC = "NCC02", TenNCC = "Công ty XYZ" }
                    };
                }

                datHang = new List<DatHangNhap>
                {
                    new DatHangNhap
                    {
                        MaPhieuDat = "PO001",
                        ThoiGian = DateTime.Now,
                        MaNCC = nccs[0].MaNCC,
                        NhaCungCap = nccs[0],
                        NgayNhapDuKien = DateTime.Now.AddDays(3),
                        SoNgayCho = 3,
                        TongTien = 1500000,
                        TrangThai = "Phiếu tạm"
                    },
                    new DatHangNhap
                    {
                        MaPhieuDat = "PO002",
                        ThoiGian = DateTime.Now.AddHours(-5),
                        MaNCC = nccs[1].MaNCC,
                        NhaCungCap = nccs[1],
                        NgayNhapDuKien = DateTime.Now.AddDays(5),
                        SoNgayCho = 5,
                        TongTien = 3000000,
                        TrangThai = "Đã xác nhận NCC"
                    }
                };
            }

            ViewBag.PhieuNhaps = phieuNhaps;
            ViewBag.NCCs = nccs;

            return View(datHang);
        }


        // ===============================
        // 2. Tạo phiếu đặt hàng nhập (POST) - ĐÃ FIX
        // ===============================
        [HttpPost]
        public IActionResult TaoPhieuDatHangNhap([FromBody] DatHangNhap model)
        {
            // Loại bỏ các trường được sinh tự động/là đối tượng khỏi ModelState
            ModelState.Remove("MaPhieuDat");
            ModelState.Remove("ThoiGian");
            ModelState.Remove("SoNgayCho");
            ModelState.Remove("NhaCungCap");
            
            // FIX LỖI: Loại bỏ validation cho Navigation Property PhieuNhap
            // Đây là lỗi hiển thị trong ảnh: (Chi tiết: PhieuNhap: The PhieuNhap field is required.)
            ModelState.Remove("PhieuNhap");
            
            if (ModelState.IsValid)
            {
                // 1. Tự động sinh MaPhieuDat
                var lastMaPhieu = _context.DatHangNhaps.OrderByDescending(d => d.MaPhieuDat).Select(d => d.MaPhieuDat).FirstOrDefault();
                var newIndex = 1;
                if (!string.IsNullOrEmpty(lastMaPhieu) && lastMaPhieu.StartsWith("PO"))
                {
                    if (int.TryParse(lastMaPhieu.Substring(2), out int lastIndex))
                    {
                        newIndex = lastIndex + 1;
                    }
                }
                model.MaPhieuDat = "PO" + newIndex.ToString("D3"); // Ví dụ: PO003

                // 2. Tự động gán Thời gian
                model.ThoiGian = DateTime.Now;

                // 3. Tính lại Số ngày chờ 
                TimeSpan soNgayCho = model.NgayNhapDuKien.Date - DateTime.Today.Date;
                model.SoNgayCho = Math.Max(0, (int)Math.Ceiling(soNgayCho.TotalDays)); 
                
                // 4. Lưu DatHangNhap
                _context.DatHangNhaps.Add(model);
                _context.SaveChanges();
                
                // ===============================================
                // 5. FIX LỖI: THÊM GIAO DỊCH VÀO LỊCH SỬ
                // ===============================================
                // Tìm tên NCC để lưu vào lịch sử
                var nccName = _context.NCCs.Find(model.MaNCC)?.TenNCC ?? model.MaNCC;
                
                var lichSu = new LichSuGiaoDich
                {
                    // Lưu ý: Cần đảm bảo MaGiaoDich là duy nhất (sử dụng GUID nếu cần)
                    // Tạm thời dùng: GD + Mã PO
                    MaGiaoDich = "GD-" + model.MaPhieuDat, 
                    LoaiGiaoDich = "Tạo Đặt Hàng Nhập", 
                    ThoiGian = model.ThoiGian,
                    DoiTac = nccName, 
                    // Giả định chi nhánh cố định (Cần thay bằng chi nhánh thực tế)
                    GiaTri = model.TongTien,
               
                    MaThamChieu = model.MaPhieuDat, // *** MÃ PO LÀ MÃ THAM CHIẾU ***
                    TrangThai = model.TrangThai 
                };

                _context.LichSuGiaoDichs.Add(lichSu);
                _context.SaveChanges();
                // ===============================================

                return Json(new { success = true, message = "Tạo phiếu thành công!", poCode = model.MaPhieuDat });
            }
            
            // Xử lý thông báo lỗi chi tiết hơn (cho debug)
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { field = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList() })
                .ToList();

            return Json(new { success = false, message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.", details = errors });
        }


        // ===============================
        // 3. Action Tra Cứu Lịch Sử Giao Dịch - ĐÃ THÊM (Fix lỗi 404)
        // ===============================
        public IActionResult TraCuu()
        {
            // --- 1. Ánh xạ (Map) từ Phiếu Đặt Hàng Nhập (Purchase Order - PO) ---
            var giaoDichPO = _context.DatHangNhaps
                .Include(d => d.NhaCungCap)
                .Select(d => new LichSuGiaoDich
                {
                    MaGiaoDich = "GD-DHN-" + d.MaPhieuDat, // Phân biệt GD đặt hàng
                    MaThamChieu = d.MaPhieuDat, // Mã PO là mã tham chiếu
                    LoaiGiaoDich = "Tạo Đặt Hàng Nhập",
                    ThoiGian = d.ThoiGian,
                    DoiTac = d.NhaCungCap != null ? d.NhaCungCap.TenNCC : "N/A",
                    GiaTri = d.TongTien,
                    TrangThai = d.TrangThai
                });


            // --- 2. Ánh xạ (Map) từ Phiếu Nhập (Receipt Note - RN) ---
            var giaoDichNhap = _context.PhieuNhaps
                .Include(p => p.NhaCungCap) // Lấy tên Nhà cung cấp
                .Select(p => new LichSuGiaoDich
                {
                    MaGiaoDich = "GD-PN-" + p.MaPN, // Phân biệt GD phiếu nhập
                    MaThamChieu = p.MaPhieuDat, // Mã PO liên kết
                    LoaiGiaoDich = "Nhập hàng",
                    ThoiGian = p.NgayNhap ?? DateTime.MinValue,
                    DoiTac = p.NhaCungCap != null ? p.NhaCungCap.TenNCC : "N/A",
                    GiaTri = p.TongGiaTri ?? 0,
                    // LƯU Ý: Nếu PhieuNhap thiếu trường TrangThai, cần thay thế "Hoàn tất" bằng logic trạng thái thực tế.
                    TrangThai = "Hoàn tất" 
                });


            // --- 3. Ánh xạ (Map) từ Phiếu Xuất (Issue Note - IN) ---
            var giaoDichXuat = _context.PhieuXuats
                .Include(x => x.NhaCungCap) // Giả sử navigation property này là Khách hàng/Đối tác nhận hàng
                .Select(x => new LichSuGiaoDich
                {
                    MaGiaoDich = "GD-PX-" + x.MaPX, // Phân biệt GD phiếu xuất
                    MaThamChieu = x.MaPX, // Dùng Mã PX làm mã tham chiếu (hoặc Mã đơn hàng bán nếu có)
                    LoaiGiaoDich = "Xuất hàng",
                    ThoiGian = x.NgayXuat ?? DateTime.MinValue,
                    // Giả sử NhaCungCap ở đây là Đối tác nhận hàng/Khách hàng
                    DoiTac = x.NhaCungCap != null ? x.NhaCungCap.TenNCC : "N/A", 
                    GiaTri = x.TongGiaTri ?? 0,
                    // LƯU Ý: Tương tự, nếu PhieuXuat thiếu TrangThai, cần thay thế bằng logic thực tế.
                    TrangThai = "Hoàn tất"
                });


            // --- 4. Gộp (Union) 3 danh sách giao dịch và sắp xếp ---
            var lichSuTongHop = giaoDichPO
                .Union(giaoDichNhap)
                .Union(giaoDichXuat)
                .OrderByDescending(gd => gd.ThoiGian)
                .ToList();

            // Trả về View TraCuu.cshtml và truyền dữ liệu LichSuGiaoDich
            return View(lichSuTongHop);
        }
        [HttpPost]
        public IActionResult Update(string id, string doiTac, decimal giaTri)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Mã giao dịch không hợp lệ." });
            }

            try
            {
                // NOTE: Trong một hệ thống thực tế, bạn cần tìm đúng loại phiếu (PO, PN, PX,...)
                // dựa trên MaGiaoDich (GD-DHN-..., GD-PN-..., GD-PX-...) và cập nhật bảng gốc
                // tương ứng (DatHangNhap, PhieuNhap, PhieuXuat).
                
                // Ở đây, vì cấu trúc hiện tại chỉ là View ánh xạ, ta sẽ GIẢ ĐỊNH
                // chúng ta chỉ có thể cập nhật trong DB nếu có bảng LichSuGiaoDich
                // hoặc cập nhật vào bảng gốc nếu logic đơn giản (ví dụ: chỉ PO).

                // *********** LOGIC GIẢ ĐỊNH CẬP NHẬT PHIẾU ĐẶT HÀNG NHẬP (PO) ***********
                if (id.StartsWith("GD-DHN-"))
                {
                    string maPhieuDat = id.Substring("GD-DHN-".Length);
                    var po = _context.DatHangNhaps.FirstOrDefault(d => d.MaPhieuDat == maPhieuDat);
                    
                    if (po == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy Phiếu Đặt Hàng Nhập." });
                    }

                    // Cập nhật thông tin giả định cho PO
                    // Lưu ý: Update DoiTac và GiaTri trong bảng gốc là một logic phức tạp
                    // (Ví dụ: cần tìm MaNCC mới nếu TenNCC thay đổi, tính lại Tổng tiền từ chi tiết,...)
                    // Ví dụ đơn giản: Cập nhật TongTien
                    po.TongTien = giaTri;
                    
                    // CẬP NHẬT TÊN NCC CŨNG LÀ RẤT PHỨC TẠP VÀ KHÔNG NÊN LÀM TỪ TRANG TRA CỨU
                    // Po.NhaCungCap.TenNCC = doiTac; // Rất nguy hiểm, cần tìm Mã NCC mới.
                    
                    _context.SaveChanges();
                    
                    return Json(new { success = true, message = "Đã cập nhật Phiếu Đặt Hàng Nhập thành công." });
                }
                
                // Nếu là loại giao dịch khác (PN, PX) thì hiện thông báo không hỗ trợ
                return Json(new { success = false, message = "Giao dịch loại này (Phiếu Nhập/Xuất) chưa được hỗ trợ sửa trực tiếp." });
            }
            catch (Exception ex)
            {
                // Log lỗi
                return Json(new { success = false, message = $"Lỗi Server: {ex.Message}" });
            }
        }
        
        // ===============================
        // 6. Xóa Lịch Sử Giao Dịch (POST) - ĐÃ CÓ (Giữ nguyên)
        // ===============================
        [HttpPost]
        public IActionResult Delete(string id)
        {
            // ... (Giữ nguyên logic của hàm Delete) ...
            return Json(new { success = false, message = "Chức năng xóa tạm thời không hoạt động." }); // Hoặc logic xóa thực tế
        }
    }
}
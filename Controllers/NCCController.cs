// File: NCCController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using QuanLyKho.Models; 

namespace QuanLyKho.Controllers
{
    // ĐỊNH NGHĨA DTO 
    public class NhaCungCapFilterDTO
    {
        public string SearchTerm { get; set; }
        public decimal? TongMuaMin { get; set; }
        public decimal? TongMuaMax { get; set; }
        public decimal? NoHienTaiMin { get; set; }
        public decimal? NoHienTaiMax { get; set; }
    }

    public class NCCController : Controller
    {
        private readonly QuanLyKhoContext _context; 

        public NCCController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------------
        // ACTION 1: Hiển thị View DanhSachNCC 
        // ------------------------------------------------------------------
        public async Task<IActionResult> DanhSachNCC()
        {
            var nccs = await _context.NCCs.ToListAsync();
            return View(nccs); 
        }

        // ------------------------------------------------------------------
        // ACTION 2: Xử lý AJAX POST để TRUY VẤN/LỌC Danh sách NCC
        // ------------------------------------------------------------------
        [HttpPost] 
        public async Task<IActionResult> GetDanhSachNCC([FromBody] NhaCungCapFilterDTO filters)
        {
            IQueryable<NCC> query = _context.NCCs; 

            if (filters.TongMuaMin.HasValue)
            {
                query = query.Where(n => n.TongMua >= filters.TongMuaMin.Value);
            }
            if (filters.TongMuaMax.HasValue)
            {
                query = query.Where(n => n.TongMua <= filters.TongMuaMax.Value);
            }
            if (filters.NoHienTaiMin.HasValue)
            {
                query = query.Where(n => n.NoHienTai >= filters.NoHienTaiMin.Value);
            }
            if (filters.NoHienTaiMax.HasValue)
            {
                query = query.Where(n => n.NoHienTai <= filters.NoHienTaiMax.Value);
            }
            
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                string search = filters.SearchTerm.ToLower();
                query = query.Where(n => 
                    n.MaNCC.ToLower().Contains(search) || 
                    n.TenNCC.ToLower().Contains(search) || 
                    (n.DienThoai != null && n.DienThoai.Contains(search)));
            }

            var filteredList = await query.OrderBy(n => n.TenNCC).ToListAsync(); 
            
            // Trả về 5 trường dữ liệu khớp với DataTables (không tính cột detail)
            var result = filteredList
                .Select(n => new {
                    ma = n.MaNCC,
                    ten = n.TenNCC,
                    dienthoai = n.DienThoai,
                    noHienTai = n.NoHienTai,
                    tongMua = n.TongMua,
                })
                .ToList();

            return Json(result);
        }
        
        // ------------------------------------------------------------------
        // ACTION 3: Xử lý AJAX POST để THÊM MỚI Nhà Cung Cấp
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromBody] NCC newSupplier)
        {
            if (!ModelState.IsValid || newSupplier.MaNCC == null || newSupplier.TenNCC == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra Mã và Tên NCC." });
            }

            try
            {
                // Kiểm tra trùng Mã NCC
                bool exists = await _context.NCCs.AnyAsync(n => n.MaNCC == newSupplier.MaNCC);
                if (exists)
                {
                    return Conflict(new { success = false, message = $"Mã NCC '{newSupplier.MaNCC}' đã tồn tại." });
                }
                
                _context.NCCs.Add(newSupplier);
                await _context.SaveChangesAsync();
                
                return Ok(new 
                { 
                    success = true, 
                    message = "Thêm NCC thành công!", 
                    data = newSupplier
                });
            }
            catch (Exception ex) 
            {
                string innerError = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { success = false, message = "Lỗi Server khi lưu dữ liệu. Chi tiết: " + innerError, error = ex.Message });
            }
        }
        
        // ------------------------------------------------------------------
        // ACTION 4: Lấy thông tin NCC theo Mã (Phục vụ cho Chỉnh sửa)
        // ------------------------------------------------------------------
        [HttpGet] 
        public async Task<IActionResult> GetNCCById(string maNCC)
        {
            if (string.IsNullOrEmpty(maNCC))
            {
                return BadRequest(new { success = false, message = "Mã NCC không hợp lệ." });
            }
            
            var ncc = await _context.NCCs.FirstOrDefaultAsync(n => n.MaNCC == maNCC);
            
            if (ncc == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy Nhà Cung Cấp." });
            }

            return Ok(new { success = true, data = ncc });
        }
        
        // ------------------------------------------------------------------
        // ACTION 5: Xử lý AJAX POST để CẬP NHẬT Nhà Cung Cấp (ĐÃ FIX LỖI)
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateSupplier([FromBody] NCC updatedSupplier)
        {
            if (!ModelState.IsValid || updatedSupplier.MaNCC == null || updatedSupplier.TenNCC == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu cập nhật không hợp lệ." });
            }

            var existingSupplier = await _context.NCCs.FirstOrDefaultAsync(n => n.MaNCC == updatedSupplier.MaNCC);

            if (existingSupplier == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy NCC có mã {updatedSupplier.MaNCC} để cập nhật." });
            }

            try
            {
                existingSupplier.TenNCC = updatedSupplier.TenNCC;
                existingSupplier.DienThoai = updatedSupplier.DienThoai;
                
                // ⭐️ FIX LỖI: Chỉ gán nếu giá trị mới khác null (vì NoHienTai/TongMua có thể là kiểu non-nullable)
                if (updatedSupplier.NoHienTai != null) 
                {
                    existingSupplier.NoHienTai = updatedSupplier.NoHienTai;
                }
                
                if (updatedSupplier.TongMua != null) 
                {
                    existingSupplier.TongMua = updatedSupplier.TongMua;
                }

                _context.NCCs.Update(existingSupplier);
                await _context.SaveChangesAsync();
                
                return Ok(new { success = true, message = "Cập nhật Nhà Cung Cấp thành công.", data = existingSupplier });
            }
            catch (Exception ex)
            {
                string innerError = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { success = false, message = "Lỗi Server khi cập nhật dữ liệu. Chi tiết: " + innerError, error = ex.Message });
            }
        }

        // ------------------------------------------------------------------
        // ACTION 6: Xử lý AJAX POST để XÓA Nhà Cung Cấp 
        // ------------------------------------------------------------------
        public class DeleteSupplierRequest
        {
            public string MaNCC { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSupplier([FromBody] DeleteSupplierRequest model)
        {
            if (string.IsNullOrEmpty(model.MaNCC))
            {
                return BadRequest(new { success = false, message = "Mã Nhà cung cấp không hợp lệ." });
            }

            try
            {
                var nccToDelete = await _context.NCCs.FirstOrDefaultAsync(n => n.MaNCC == model.MaNCC);

                if (nccToDelete == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy NCC cần xóa." });
                }

                // Thực hiện xóa
                _context.NCCs.Remove(nccToDelete);
                await _context.SaveChangesAsync();
                
                return Ok(new { success = true, message = $"Đã xóa NCC '{model.MaNCC}' thành công." });
            }
            catch (Exception ex)
            {
                string innerError = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { success = false, message = "Lỗi Server khi xóa: " + innerError });
            }
        }

        // ------------------------------------------------------------------
        // ACTION 7: Nhập
        // ------------------------------------------------------------------
        public async Task<IActionResult> Nhap()
        {
            var phieuNhapList = await _context.PhieuNhaps
            .Include(px => px.NhaCungCap) 
            .Include(px => px.NhanVien)   
            .ToListAsync();
        
            var supplierList = await _context.NCCs.ToListAsync();
            ViewData["Suppliers"] = supplierList; 
            
            return View(phieuNhapList); 
        }

        // ------------------------------------------------------------------
        // ACTION 8: Xuất
        // ------------------------------------------------------------------
        public async Task<IActionResult> Xuat()
        {
            var phieuXuatList = await _context.PhieuXuats
                .Include(px => px.NhaCungCap) 
                .Include(px => px.NhanVien)   
                .ToListAsync();
            
            var supplierList = await _context.NCCs.ToListAsync();
            ViewData["Suppliers"] = supplierList; 
            
            return View(phieuXuatList); 
        }
    }
}
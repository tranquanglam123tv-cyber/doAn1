// File: NhanVienController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using QuanLyKho.Models; // Giả định Employee là Model trong Models namespace

namespace QuanLyKho.Controllers
{
    // --- 1. ĐÃ CẬP NHẬT DTO: Thêm StatusFilter ---
    public class NhanVienFilterDTO
    {
        public string SearchTerm { get; set; }
        public string StatusFilter { get; set; } // <--- ĐÃ THÊM THAM SỐ NÀY
    }
    
    // DTO cho cập nhật trạng thái
    public class EmployeeStatusUpdateDTO
    {
        public string MaNV { get; set; }
        public string NewStatus { get; set; }
    }

    public class NhanVienController : Controller
    {
        private readonly QuanLyKhoContext _context; 

        // Khởi tạo context
        public NhanVienController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // ------------------------------------------------------------------
        // ACTION 1: Hiển thị View NhanVien 
        // ------------------------------------------------------------------
        public IActionResult NhanVien()
        {
            return View();
        }
        
        // ------------------------------------------------------------------
        // ACTION 2: Lấy danh sách NV (AJAX) - ĐÃ CẬP NHẬT LỌC TRẠNG THÁI
        // ------------------------------------------------------------------
        [HttpPost] 
        public async Task<IActionResult> GetDanhSachNhanVien([FromBody] NhanVienFilterDTO filter)
        {
            IQueryable<Employee> query = _context.Employees; 

            // 1. Lọc theo SearchTerm
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower().Trim();
                query = query.Where(e => 
                    e.MaNV.ToLower().Contains(term) ||
                    e.TenNV.ToLower().Contains(term) ||
                    e.ViTri.ToLower().Contains(term) ||
                    e.DienThoai.ToLower().Contains(term)
                );
            }
            
            // 2. Lọc theo StatusFilter (ĐÃ THÊM LOGIC NÀY)
            if (!string.IsNullOrEmpty(filter.StatusFilter))
            {
                query = query.Where(e => e.TrangThai == filter.StatusFilter);
            }

            try
            {
                var result = await query.OrderBy(e => e.MaNV).ToListAsync();
                
                // Chuyển về camelCase để dễ xử lý trong JavaScript
                var response = result.Select(e => new 
                {
                    maNV = e.MaNV,
                    tenNV = e.TenNV,
                    viTri = e.ViTri,
                    dienThoai = e.DienThoai,
                    trangThai = e.TrangThai
                }).ToList();

                return Json(response);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu kết nối DB thất bại
                return StatusCode(500, new { success = false, message = "Lỗi Server khi tải dữ liệu: " + ex.Message });
            }
        }
        
        // ------------------------------------------------------------------
        // ACTION 3: Thêm nhân viên
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> AddNhanVien([FromBody] Employee employee)
        {
            if (await _context.Employees.AnyAsync(e => e.MaNV == employee.MaNV.Trim()))
            {
                return Conflict(new { success = false, message = $"Mã nhân viên '{employee.MaNV}' đã tồn tại." });
            }
            
            employee.TrangThai = "Hoạt động"; // Đặt trạng thái mặc định
            employee.MaNV = employee.MaNV.Trim();

            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = $"Đã thêm nhân viên: {employee.TenNV}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi Server khi thêm: " + ex.InnerException?.Message ?? ex.Message });
            }
        }
        
        // ------------------------------------------------------------------
        // ACTION 4: Cập nhật nhân viên (ĐÃ CẬP NHẬT XỬ LÝ TRẠNG THÁI)
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateNhanVien([FromBody] Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.MaNV);
            if (existingEmployee == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy nhân viên có mã '{employee.MaNV}'." });
            }

            try
            {
                // Cập nhật các trường
                existingEmployee.TenNV = employee.TenNV;
                existingEmployee.ViTri = employee.ViTri;
                existingEmployee.DienThoai = employee.DienThoai;
                
                // Cập nhật trạng thái chỉ khi nó được gửi từ form (mode 'edit')
                if (!string.IsNullOrEmpty(employee.TrangThai))
                {
                    existingEmployee.TrangThai = employee.TrangThai;
                }
                
                _context.Employees.Update(existingEmployee);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = $"Đã cập nhật nhân viên: {employee.TenNV}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi Server khi cập nhật: " + ex.InnerException?.Message ?? ex.Message });
            }
        }

        // ------------------------------------------------------------------
        // ACTION 5: Xóa nhân viên
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> DeleteNhanVien([FromBody] Employee employee)
        {
            var existingEmployee = await _context.Employees.FindAsync(employee.MaNV);
            if (existingEmployee == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy nhân viên có mã '{employee.MaNV}'." });
            }

            try
            {
                _context.Employees.Remove(existingEmployee);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = $"Đã xóa nhân viên: {existingEmployee.TenNV}." });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có ràng buộc khóa ngoại
                return StatusCode(500, new { success = false, message = "Lỗi Server khi xóa (Kiểm tra ràng buộc khóa ngoại): " + ex.InnerException?.Message ?? ex.Message });
            }
        }

        // ------------------------------------------------------------------
        // ACTION 6: Cập nhật trạng thái (Khóa/Mở tài khoản)
        // ------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateTrangThai([FromBody] EmployeeStatusUpdateDTO dto)
        {
            var existingEmployee = await _context.Employees.FindAsync(dto.MaNV);
            if (existingEmployee == null)
            {
                return NotFound(new { success = false, message = $"Không tìm thấy nhân viên có mã '{dto.MaNV}'." });
            }

            try
            {
                existingEmployee.TrangThai = dto.NewStatus;
                _context.Employees.Update(existingEmployee);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = $"Đã cập nhật trạng thái nhân viên '{existingEmployee.TenNV}' sang '{dto.NewStatus}'." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi Server khi cập nhật trạng thái: " + ex.InnerException?.Message ?? ex.Message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
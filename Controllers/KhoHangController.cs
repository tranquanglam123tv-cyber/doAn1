using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;        // Required for Include()
using QuanLyKho.Models;                     // Required for QuanLyKhoContext

namespace QuanLyKho.Controllers
{
    public class KhoHangController : Controller
    {
        private readonly QuanLyKhoContext _context;

        public KhoHangController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // ==============================
        // THỐNG KÊ SAI LỆCH KHO
        // ==============================
        public IActionResult ThongKeSaiLech()
        {
            var data = _context.KiemKeKhos
                .Include(x => x.HangHoa)
                .OrderByDescending(x => x.NgayKiemKe)
                .ToList();

            return View(data);
        }

        // ==============================
        // CHUYỂN KHO
        // ==============================
        public IActionResult ChuyenKho()
        {
            return View();
        }

        // ==============================
        // CHỌN KHO ĐỂ CHUYỂN
        // ==============================
        public IActionResult ChonKhoDeChuyen()
        {
            return View();
        }
    }
}

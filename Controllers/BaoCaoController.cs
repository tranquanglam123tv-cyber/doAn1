using Microsoft.AspNetCore.Mvc;

namespace QuanLyKho.Controllers
{
    public class BaoCaoController : Controller
    {
        public IActionResult DanhSachBC()
        {
            return View();
        }

        public IActionResult BCNhap()
        {
            return View();
        }

        public IActionResult BCXuat()
        {
            return View();
        }
    
    }
}
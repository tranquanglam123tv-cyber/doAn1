using Microsoft.AspNetCore.Mvc;

namespace QuanLyKho.Controllers
{
    public class TaoBaoCaoController : Controller
    {
        public IActionResult BaoCaoNhap()
        {
            return View();
        }

        public IActionResult BaoCaoXuat()
        {
            return View();
        }
    }
}
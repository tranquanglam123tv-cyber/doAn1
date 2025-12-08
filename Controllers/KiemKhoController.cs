using Microsoft.AspNetCore.Mvc;

namespace QuanLyKho.Controllers
{
    public class KiemKhoController : Controller
{
    // Trang danh sách phiếu kiểm kho
    public ActionResult Index()
    {
        return View();
    }

    // Trang tạo phiếu kiểm kho (hiện giao diện như bạn gửi ảnh)
    public ActionResult Create()
    {
        return View();
    }
}
}
using Microsoft.AspNetCore.Mvc;
using QuanLyKho.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http; // ĐÃ THÊM: Cần thiết cho Session
using System.Linq; 
using System; 

namespace QuanLyKho.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyKhoContext _context;

        public AccountController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // 1. GET: Hiển thị form đăng nhập
        [HttpGet]
        public IActionResult Login(string role = "Manager")  
        {
            ViewBag.DesiredRole = role; 
            return View();
        }

[HttpPost]
public IActionResult Login(string username, string password, string desiredRole)
{
    var user = _context.Accounts
        .FirstOrDefault(a => a.Username == username && a.Password == password);

    if (user != null)
    {
        string roleInDb = user.Role?.Trim() ?? "";

       bool isManager = string.Equals(roleInDb, "Manager", StringComparison.OrdinalIgnoreCase) 
                      || string.Equals(roleInDb, "QuanLy", StringComparison.OrdinalIgnoreCase);

        bool isEmployee = string.Equals(roleInDb, "Employee", StringComparison.OrdinalIgnoreCase) 
                       || string.Equals(roleInDb, "NhanVien", StringComparison.OrdinalIgnoreCase);
        if (desiredRole == "Manager" && !isManager)
        {
            ViewBag.Error = "Tài khoản này không có quyền Quản lý!";
            ViewBag.DesiredRole = desiredRole;
            return View();
        }
        if (desiredRole == "Employee" && !isEmployee)
        {
            ViewBag.Error = "Tài khoản này không có quyền Nhân viên!";
            ViewBag.DesiredRole = desiredRole;
            return View();
        }

        HttpContext.Session.SetString("Username", user.Username);
        
        HttpContext.Session.SetString("Role", roleInDb);

        if (isManager)
        {
            return RedirectToAction("Index", "Home"); 
        }
        else if (isEmployee)
        {
            
            return RedirectToAction("Index", "NhanVien"); 
        }
        else 
        {
            ViewBag.Error = "Lỗi: Quyền hạn không xác định (" + roleInDb + ")";
            return View();
        }
    }

    ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
    ViewBag.DesiredRole = desiredRole;
    return View();
}

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
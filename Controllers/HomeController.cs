using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyKho.Models;

namespace QuanLyKho.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
      
    var role = HttpContext.Session.GetString("VaiTro");
    if (role == "QuanLy")
        ViewBag.Layout = "_Layout";
    else
        ViewBag.Layout = "_Layout";
    return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}

using Microsoft.AspNetCore.Mvc;
using QuanLyKho.Models;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace QuanLyKho.Controllers
{
    // Model để nhận JSON khi xóa
    public class DeleteRequestModel
    {
        [Required]
        public string MaHang { get; set; }
    }

    public class HangHoaController : Controller
    {
        private readonly QuanLyKhoContext _context;

        public HangHoaController(QuanLyKhoContext context)
        {
            _context = context;
        }

        // ============================
        // 0. LẤY TẤT CẢ DỮ LIỆU (AJAX)
        // ============================
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var data = _context.HangHoas
                    .OrderByDescending(x => x.ThoiGianTao)
                    .Select(x => new
                    {
                        x.MaHang,
                        x.TenHang,
                        x.LoaiHang,
                        x.GiaBan,
                        x.GiaVon,
                        x.TonKho,
                        x.KhachDat,
                        ThoiGianTao = x.ThoiGianTao.ToString("yyyy-MM-ddTHH:mm:ss"),
                        x.DatNCC
                    })
                    .ToList();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server." });
            }
        }

        // ============================
        // 1. LẤY DỮ LIỆU XE
        // ============================
        [HttpGet]
        public IActionResult GetAllXe()
        {
            try
            {
                var data = _context.HangHoas
                    .Where(x => x.LoaiHang != null && x.LoaiHang.Contains("Xe"))
                    .OrderByDescending(x => x.ThoiGianTao)
                    .Select(x => new
                    {
                        x.MaHang,
                        x.TenHang,
                        x.LoaiHang,
                        x.GiaBan,
                        x.GiaVon,
                        x.TonKho,
                        x.KhachDat,
                        ThoiGianTao = x.ThoiGianTao.ToString("yyyy-MM-ddTHH:mm:ss"),
                        x.DatNCC
                    })
                    .ToList();

                return Json(data);
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Lỗi server." });
            }
        }

        // ============================
        // 2. LẤY LINH KIỆN
        // ============================
        [HttpGet]
        public IActionResult GetAllLinhKien()
        {
            try
            {
                var data = _context.HangHoas
                    .Where(x => x.LoaiHang == null || !x.LoaiHang.Contains("Xe"))
                    .OrderByDescending(x => x.ThoiGianTao)
                    .Select(x => new
                    {
                        x.MaHang,
                        x.TenHang,
                        x.LoaiHang,
                        x.GiaBan,
                        x.GiaVon,
                        x.TonKho,
                        x.KhachDat,
                        ThoiGianTao = x.ThoiGianTao.ToString("yyyy-MM-ddTHH:mm:ss"),
                        x.DatNCC
                    })
                    .ToList();

                return Json(data);
            }
            catch
            {
                return StatusCode(500, new { success = false, message = "Lỗi server." });
            }
        }

        // ============================
        // 3. TẠO MỚI HÀNG HÓA
        // ============================
        [HttpPost]
        public IActionResult Create([FromBody] HangHoa model)
        {
            if (model == null)
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            if (string.IsNullOrWhiteSpace(model.TenHang))
                return Json(new { success = false, message = "Tên hàng không được để trống." });

            try
            {
                // Tự sinh mã
                if (string.IsNullOrWhiteSpace(model.MaHang))
                {
                    string prefix = (model.LoaiHang != null && model.LoaiHang.Contains("Xe")) ? "XD" : "LK";

                    var last = _context.HangHoas
                        .Where(x => x.MaHang.StartsWith(prefix))
                        .OrderByDescending(x => x.MaHang)
                        .FirstOrDefault();

                    int lastNum = 0;
                    if (last != null)
                    {
                        int.TryParse(last.MaHang.Substring(prefix.Length), out lastNum);
                    }

                    model.MaHang = prefix + (lastNum + 1).ToString("D3");
                }

                model.ThoiGianTao = DateTime.Now;

                model.GiaBan = Math.Max(0, model.GiaBan);
                model.GiaVon = Math.Max(0, model.GiaVon);
                model.TonKho = Math.Max(0, model.TonKho);
                model.KhachDat = Math.Max(0, model.KhachDat);

                _context.HangHoas.Add(model);
                _context.SaveChanges();

                return Json(new { success = true, maHang = model.MaHang });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        // ============================
        // 4. LẤY HÀNG THEO MÃ
        // ============================
        [HttpGet]
        public IActionResult GetById(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma))
                return BadRequest(new { success = false, message = "Mã không hợp lệ." });

            var hh = _context.HangHoas
                .Where(x => x.MaHang == ma)
                .Select(x => new
                {
                    x.MaHang,
                    x.TenHang,
                    x.LoaiHang,
                    x.GiaBan,
                    x.GiaVon,
                    x.TonKho,
                    x.KhachDat,
                    ThoiGianTao = x.ThoiGianTao.ToString("yyyy-MM-ddTHH:mm:ss"),
                    x.DatNCC
                })
                .FirstOrDefault();

            if (hh == null)
                return NotFound(new { success = false, message = "Không tìm thấy." });

            return Json(hh);
        }

        // ============================
        // 5. EDIT AJAX
        // ============================
        [HttpPost]
        public IActionResult EditAjax([FromBody] HangHoa model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.MaHang))
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });

            var hh = _context.HangHoas.FirstOrDefault(x => x.MaHang == model.MaHang);
            if (hh == null)
                return NotFound(new { success = false, message = "Không tìm thấy hàng." });

            try
            {
                hh.TenHang = model.TenHang;
                hh.LoaiHang = model.LoaiHang;
                hh.GiaBan = model.GiaBan;
                hh.GiaVon = model.GiaVon;
                hh.TonKho = model.TonKho;
                hh.KhachDat = model.KhachDat;
                hh.DatNCC = model.DatNCC;

                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        // ============================
        // 6. DELETE AJAX
        // ============================
        [HttpPost]
        public IActionResult DeleteAjax([FromBody] DeleteRequestModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.MaHang))
                return Json(new { success = false, message = "Mã không hợp lệ." });

            try
            {
                var hh = _context.HangHoas.FirstOrDefault(x => x.MaHang == model.MaHang);
                if (hh == null)
                    return NotFound(new { success = false, message = "Không tìm thấy hàng." });

                _context.HangHoas.Remove(hh);
                _context.SaveChanges();

                return Json(new { success = true, message = "Đã xóa." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }

        // ============================
        // VIEW
        // ============================
        public IActionResult DanhSach() => View();
        public IActionResult QuanLyXe() => View();
        public IActionResult QuanLyLinhKien() => View();
    }
}
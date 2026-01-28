// File: NhanVien.cs (Đổi tên thành Employee.cs để khớp với nội dung cũ)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models;

[Table("Employee")] 
public class Employee
{
    [Key]
    [Column(TypeName = "varchar(20)")]
    public string MaNV { get; set; }

    [Required]
    [MaxLength(100)]
    public string TenNV { get; set; }

    // ⭐️ THÊM: Vị trí (Position)
    [MaxLength(100)]
    public string ViTri { get; set; } 

    // ⭐️ THÊM: Điện thoại (Phone)
    [Column(TypeName = "varchar(20)")]
    public string DienThoai { get; set; } 

    [Required]
    [Column(TypeName = "varchar(50)")]
    public string TrangThai { get; set; } // 'Hoạt động', 'Tạm khóa', v.v.
}
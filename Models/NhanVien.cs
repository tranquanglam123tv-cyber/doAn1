
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
    [Column(TypeName = "nvarchar(100)")]
    public string TenNV { get; set; }

    [MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string ViTri { get; set; } 

    [Column(TypeName = "varchar(20)")]
    public string DienThoai { get; set; } 

    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string TrangThai { get; set; } 
}
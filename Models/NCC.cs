// File: NCC.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyKho.Models;

[Table("NhaCungCap")] 
public class NCC
{
    [Key]
    [Column(TypeName = "varchar(20)")]
    public string MaNCC { get; set; }

    [Required]
    [MaxLength(100)]
    [Column(TypeName = "nvarchar(200)")]
    public string TenNCC { get; set; }
    
    [Column(TypeName = "varchar(20)")]
    public string DienThoai { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal NoHienTai { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal TongMua { get; set; }
    


}
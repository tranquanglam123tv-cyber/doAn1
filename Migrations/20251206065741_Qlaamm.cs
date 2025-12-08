using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Migrations
{
    /// <inheritdoc />
    public partial class Qlaamm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // BƯỚC 1: Xóa Khóa Ngoại MaNCC trước khi thay đổi cột (FIX LỖI 1)
            migrationBuilder.DropForeignKey(
                name: "FK_DatHangNhap_NhaCungCap_MaNCC",
                table: "DatHangNhap");

            migrationBuilder.AlterColumn<string>(
                name: "TenHang",
                table: "HangHoas",
                type: "nvarchar(100)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "KhachDat",
                table: "HangHoas",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DatNCC",
                table: "HangHoas",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "Employee",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "DatHangNhap",
                type: "nvarchar(100)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            // Lệnh AlterColumn cho MaNCC
            migrationBuilder.AlterColumn<string>(
                name: "MaNCC",
                table: "DatHangNhap",
                type: "nvarchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            // BƯỚC 2: Thêm Khóa Ngoại trở lại (ĐÃ SỬA TÊN BẢNG)
            migrationBuilder.AddForeignKey(
                name: "FK_DatHangNhap_NhaCungCap_MaNCC",
                table: "DatHangNhap",
                column: "MaNCC",
                principalTable: "NhaCungCap", // <-- ĐÃ SỬA
                principalColumn: "MaNCC",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // BƯỚC 1: Xóa Khóa Ngoại MaNCC trước khi khôi phục cột
            migrationBuilder.DropForeignKey(
                name: "FK_DatHangNhap_NhaCungCap_MaNCC",
                table: "DatHangNhap");


            migrationBuilder.AlterColumn<string>(
                name: "TenHang",
                table: "HangHoas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "KhachDat",
                table: "HangHoas",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AlterColumn<int>(
                name: "DatNCC",
                table: "HangHoas",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "Employee",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "DatHangNhap",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "MaNCC",
                table: "DatHangNhap",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)");
            
            // BƯỚC 2: Thêm Khóa Ngoại trở lại (ĐÃ SỬA TÊN BẢNG)
            migrationBuilder.AddForeignKey(
                name: "FK_DatHangNhap_NhaCungCap_MaNCC",
                table: "DatHangNhap",
                column: "MaNCC",
                principalTable: "NhaCungCap", // <-- ĐÃ SỬA
                principalColumn: "MaNCC",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
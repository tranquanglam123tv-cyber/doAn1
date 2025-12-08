using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Migrations
{
    /// <inheritdoc />
    public partial class Qlam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhieuNhap_Employee_NhanVienMaNV",
                table: "PhieuNhap");

            migrationBuilder.DropIndex(
                name: "IX_PhieuNhap_NhanVienMaNV",
                table: "PhieuNhap");

            migrationBuilder.DropColumn(
                name: "NhanVienMaNV",
                table: "PhieuNhap");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_HangHoas_MaHang",
                table: "HangHoas",
                column: "MaHang");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_MaNV",
                table: "PhieuNhap",
                column: "MaNV");

            migrationBuilder.CreateIndex(
                name: "IX_HangHoas_MaHang",
                table: "HangHoas",
                column: "MaHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_MaHH",
                table: "ChiTietPhieuNhap",
                column: "MaHH");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietPhieuNhap_HangHoas_MaHH",
                table: "ChiTietPhieuNhap",
                column: "MaHH",
                principalTable: "HangHoas",
                principalColumn: "MaHang",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuNhap_Employee_MaNV",
                table: "PhieuNhap",
                column: "MaNV",
                principalTable: "Employee",
                principalColumn: "MaNV",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietPhieuNhap_HangHoas_MaHH",
                table: "ChiTietPhieuNhap");

            migrationBuilder.DropForeignKey(
                name: "FK_PhieuNhap_Employee_MaNV",
                table: "PhieuNhap");

            migrationBuilder.DropIndex(
                name: "IX_PhieuNhap_MaNV",
                table: "PhieuNhap");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_HangHoas_MaHang",
                table: "HangHoas");

            migrationBuilder.DropIndex(
                name: "IX_HangHoas_MaHang",
                table: "HangHoas");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietPhieuNhap_MaHH",
                table: "ChiTietPhieuNhap");

            migrationBuilder.AddColumn<string>(
                name: "NhanVienMaNV",
                table: "PhieuNhap",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_NhanVienMaNV",
                table: "PhieuNhap",
                column: "NhanVienMaNV");

            migrationBuilder.AddForeignKey(
                name: "FK_PhieuNhap_Employee_NhanVienMaNV",
                table: "PhieuNhap",
                column: "NhanVienMaNV",
                principalTable: "Employee",
                principalColumn: "MaNV");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKho.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    MaNV = table.Column<string>(type: "varchar(20)", nullable: false),
                    TenNV = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DienThoai = table.Column<string>(type: "varchar(20)", nullable: false),
                    TrangThai = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "HangHoas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenHang = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LoaiHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiaVon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TonKho = table.Column<int>(type: "int", nullable: false),
                    TenKhoCha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KhachDat = table.Column<int>(type: "int", nullable: false),
                    DatNCC = table.Column<int>(type: "int", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangHoas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LichSuGiaoDich",
                columns: table => new
                {
                    MaGiaoDich = table.Column<string>(type: "varchar(20)", nullable: false),
                    MaThamChieu = table.Column<string>(type: "varchar(20)", nullable: false),
                    LoaiGiaoDich = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoiTac = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    GiaTri = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuGiaoDich", x => x.MaGiaoDich);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    MaNCC = table.Column<string>(type: "varchar(20)", nullable: false),
                    TenNCC = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DienThoai = table.Column<string>(type: "varchar(20)", nullable: false),
                    NoHienTai = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TongMua = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.MaNCC);
                });

            migrationBuilder.CreateTable(
                name: "KiemKeKhos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HangHoaId = table.Column<int>(type: "int", nullable: false),
                    MaHang = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Kho = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TonHeThong = table.Column<int>(type: "int", nullable: false),
                    TonThucTe = table.Column<int>(type: "int", nullable: false),
                    NgayKiemKe = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KiemKeKhos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KiemKeKhos_HangHoas_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "HangHoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NhapHangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HangHoaId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhapHangs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhapHangs_HangHoas_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "HangHoas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatHangNhap",
                columns: table => new
                {
                    MaPhieuDat = table.Column<string>(type: "varchar(20)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNCC = table.Column<string>(type: "varchar(20)", nullable: false),
                    NgayNhapDuKien = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNgayCho = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatHangNhap", x => x.MaPhieuDat);
                    table.ForeignKey(
                        name: "FK_DatHangNhap_NhaCungCap_MaNCC",
                        column: x => x.MaNCC,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNCC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuXuat",
                columns: table => new
                {
                    MaPX = table.Column<string>(type: "varchar(20)", nullable: false),
                    MaNCC = table.Column<string>(type: "varchar(20)", nullable: false),
                    NgayXuat = table.Column<DateTime>(type: "date", nullable: true),
                    MaNV = table.Column<string>(type: "varchar(20)", nullable: false),
                    TongGiaTri = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuXuat", x => x.MaPX);
                    table.ForeignKey(
                        name: "FK_PhieuXuat_Employee_MaNV",
                        column: x => x.MaNV,
                        principalTable: "Employee",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuXuat_NhaCungCap_MaNCC",
                        column: x => x.MaNCC,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNCC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhap",
                columns: table => new
                {
                    MaPN = table.Column<string>(type: "varchar(20)", nullable: false),
                    MaNCC = table.Column<string>(type: "varchar(20)", nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "date", nullable: true),
                    MaNV = table.Column<string>(type: "varchar(20)", nullable: false),
                    NhanVienMaNV = table.Column<string>(type: "varchar(20)", nullable: true),
                    TongGiaTri = table.Column<decimal>(type: "decimal(18,0)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    MaPhieuDat = table.Column<string>(type: "varchar(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhap", x => x.MaPN);
                    table.ForeignKey(
                        name: "FK_PhieuNhap_DatHangNhap_MaPhieuDat",
                        column: x => x.MaPhieuDat,
                        principalTable: "DatHangNhap",
                        principalColumn: "MaPhieuDat");
                    table.ForeignKey(
                        name: "FK_PhieuNhap_Employee_NhanVienMaNV",
                        column: x => x.NhanVienMaNV,
                        principalTable: "Employee",
                        principalColumn: "MaNV");
                    table.ForeignKey(
                        name: "FK_PhieuNhap_NhaCungCap_MaNCC",
                        column: x => x.MaNCC,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNCC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuXuat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPX = table.Column<string>(type: "varchar(20)", nullable: false),
                    MaHH = table.Column<string>(type: "varchar(20)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuXuat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuXuat_PhieuXuat_MaPX",
                        column: x => x.MaPX,
                        principalTable: "PhieuXuat",
                        principalColumn: "MaPX",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuNhap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPN = table.Column<string>(type: "varchar(20)", nullable: false),
                    MaHH = table.Column<string>(type: "varchar(20)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaNhap = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuNhap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_PhieuNhap_MaPN",
                        column: x => x.MaPN,
                        principalTable: "PhieuNhap",
                        principalColumn: "MaPN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_MaPN",
                table: "ChiTietPhieuNhap",
                column: "MaPN");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuXuat_MaPX",
                table: "ChiTietPhieuXuat",
                column: "MaPX");

            migrationBuilder.CreateIndex(
                name: "IX_DatHangNhap_MaNCC",
                table: "DatHangNhap",
                column: "MaNCC");

            migrationBuilder.CreateIndex(
                name: "IX_KiemKeKhos_HangHoaId",
                table: "KiemKeKhos",
                column: "HangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_NhapHangs_HangHoaId",
                table: "NhapHangs",
                column: "HangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_MaNCC",
                table: "PhieuNhap",
                column: "MaNCC");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_MaPhieuDat",
                table: "PhieuNhap",
                column: "MaPhieuDat",
                unique: true,
                filter: "[MaPhieuDat] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhap_NhanVienMaNV",
                table: "PhieuNhap",
                column: "NhanVienMaNV");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuXuat_MaNCC",
                table: "PhieuXuat",
                column: "MaNCC");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuXuat_MaNV",
                table: "PhieuXuat",
                column: "MaNV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhap");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuXuat");

            migrationBuilder.DropTable(
                name: "KiemKeKhos");

            migrationBuilder.DropTable(
                name: "LichSuGiaoDich");

            migrationBuilder.DropTable(
                name: "NhapHangs");

            migrationBuilder.DropTable(
                name: "PhieuNhap");

            migrationBuilder.DropTable(
                name: "PhieuXuat");

            migrationBuilder.DropTable(
                name: "HangHoas");

            migrationBuilder.DropTable(
                name: "DatHangNhap");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "NhaCungCap");
        }
    }
}

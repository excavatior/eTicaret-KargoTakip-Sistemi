using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MerhabaDunyaApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmisyonFaktorleri",
                columns: table => new
                {
                    Kimlik = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    YakitTipi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KgCO2PerLitre = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmisyonFaktorleri", x => x.Kimlik);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Kimlik = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EPosta = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SifreSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Kimlik);
                });

            migrationBuilder.CreateTable(
                name: "Rozetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredSavePct = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rozetler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Siparisler",
                columns: table => new
                {
                    Kimlik = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciKimlik = table.Column<int>(type: "int", nullable: false),
                    SiparisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Siparisler", x => x.Kimlik);
                    table.ForeignKey(
                        name: "FK_Siparisler_Kullanicilar_KullaniciKimlik",
                        column: x => x.KullaniciKimlik,
                        principalTable: "Kullanicilar",
                        principalColumn: "Kimlik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciRozetleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciKimlik = table.Column<int>(type: "int", nullable: false),
                    RozetId = table.Column<int>(type: "int", nullable: false),
                    VerilisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciRozetleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_Kullanicilar_KullaniciKimlik",
                        column: x => x.KullaniciKimlik,
                        principalTable: "Kullanicilar",
                        principalColumn: "Kimlik",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciRozetleri_Rozetler_RozetId",
                        column: x => x.RozetId,
                        principalTable: "Rozetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kargolar",
                columns: table => new
                {
                    Kimlik = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiparisKimlik = table.Column<int>(type: "int", nullable: false),
                    TakipNumarasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GonderimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Durum = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kargolar", x => x.Kimlik);
                    table.ForeignKey(
                        name: "FK_Kargolar_Siparisler_SiparisKimlik",
                        column: x => x.SiparisKimlik,
                        principalTable: "Siparisler",
                        principalColumn: "Kimlik",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kargolar_SiparisKimlik",
                table: "Kargolar",
                column: "SiparisKimlik",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_KullaniciKimlik",
                table: "KullaniciRozetleri",
                column: "KullaniciKimlik");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciRozetleri_RozetId",
                table: "KullaniciRozetleri",
                column: "RozetId");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_KullaniciKimlik",
                table: "Siparisler",
                column: "KullaniciKimlik");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmisyonFaktorleri");

            migrationBuilder.DropTable(
                name: "Kargolar");

            migrationBuilder.DropTable(
                name: "KullaniciRozetleri");

            migrationBuilder.DropTable(
                name: "Siparisler");

            migrationBuilder.DropTable(
                name: "Rozetler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}

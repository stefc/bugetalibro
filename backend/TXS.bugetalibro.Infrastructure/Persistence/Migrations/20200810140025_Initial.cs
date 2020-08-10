using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TXS.bugetalibro.Infrastructure.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auszahlung",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Datum = table.Column<DateTime>(nullable: false),
                    Betrag = table.Column<double>(nullable: false),
                    Notiz = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auszahlung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Einzahlung",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Datum = table.Column<DateTime>(nullable: false),
                    Betrag = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Einzahlung", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auszahlung");

            migrationBuilder.DropTable(
                name: "Einzahlung");
        }
    }
}

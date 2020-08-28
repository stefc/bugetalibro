using Microsoft.EntityFrameworkCore.Migrations;

namespace TXS.bugetalibro.Infrastructure.Persistence.Migrations
{
    public partial class delete_auszahlung : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Auszahlung_Kategorie_KategorieId",
            //     table: "Auszahlung");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Auszahlung_Kategorie_KategorieId",
            //     table: "Auszahlung",
            //     column: "KategorieId",
            //     principalTable: "Kategorie",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropForeignKey(
            //     name: "FK_Auszahlung_Kategorie_KategorieId",
            //     table: "Auszahlung");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Auszahlung_Kategorie_KategorieId",
            //     table: "Auszahlung",
            //     column: "KategorieId",
            //     principalTable: "Kategorie",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);
        }
    }
}

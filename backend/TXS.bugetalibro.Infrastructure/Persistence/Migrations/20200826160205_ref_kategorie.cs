﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TXS.bugetalibro.Infrastructure.Persistence.Migrations
{
    public partial class ref_kategorie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "KategorieId",
                table: "Auszahlung",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Auszahlung_KategorieId",
                table: "Auszahlung",
                column: "KategorieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auszahlung_Kategorie_KategorieId",
                table: "Auszahlung",
                column: "KategorieId",
                principalTable: "Kategorie",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auszahlung_Kategorie_KategorieId",
                table: "Auszahlung");

            migrationBuilder.DropIndex(
                name: "IX_Auszahlung_KategorieId",
                table: "Auszahlung");

            migrationBuilder.DropColumn(
                name: "KategorieId",
                table: "Auszahlung");
        }
    }
}

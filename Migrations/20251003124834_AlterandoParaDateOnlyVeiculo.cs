using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MINIMAL_API.Migrations
{
    /// <inheritdoc />
    public partial class AlterandoParaDateOnlyVeiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ano",
                table: "Veiculos");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Data",
                table: "Veiculos",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "Veiculos");

            migrationBuilder.AddColumn<int>(
                name: "Ano",
                table: "Veiculos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StockMonitor.Migrations
{
    public partial class Rate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentPrice",
                table: "Transactions",
                newName: "Rate");

            migrationBuilder.AddColumn<float>(
                name: "InitialPrice",
                table: "Transactions",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialPrice",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Rate",
                table: "Transactions",
                newName: "CurrentPrice");
        }
    }
}

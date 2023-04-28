using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurKoCarRental.Migrations
{
    public partial class changeddamageandrentalrequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamageRequestDate",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "DamageStatus",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "PaidDate",
                table: "Damages");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Damages");

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "RentalRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "RentalRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "damage",
                table: "RentalRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Paid",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "damage",
                table: "RentalRequests");

            migrationBuilder.AddColumn<DateTime>(
                name: "DamageRequestDate",
                table: "Damages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DamageStatus",
                table: "Damages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidDate",
                table: "Damages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Damages",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

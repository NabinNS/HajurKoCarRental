using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurKoCarRental.Migrations
{
    public partial class updaterentalrequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "RentalRequests");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorizedBy",
                table: "RentalRequests",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AuthorizedBy",
                table: "RentalRequests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "RentalRequests",
                type: "datetime2",
                nullable: true);
        }
    }
}

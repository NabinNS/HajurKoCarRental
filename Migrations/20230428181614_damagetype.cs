using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurKoCarRental.Migrations
{
    public partial class damagetype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DamageType",
                table: "Damages",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamageType",
                table: "Damages");
        }
    }
}

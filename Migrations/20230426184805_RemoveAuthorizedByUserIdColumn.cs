using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HajurKoCarRental.Migrations
{

    public partial class RemoveAuthorizedByUserIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalRequests_AspNetUsers_AuthorizedByUserId",
                table: "RentalRequests");

            migrationBuilder.DropIndex(
                name: "IX_RentalRequests_AuthorizedByUserId",
                table: "RentalRequests");

            migrationBuilder.DropColumn(
                name: "AuthorizedByUserId",
                table: "RentalRequests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizedByUserId",
                table: "RentalRequests",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RentalRequests_AuthorizedByUserId",
                table: "RentalRequests",
                column: "AuthorizedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalRequests_AspNetUsers_AuthorizedByUserId",
                table: "RentalRequests",
                column: "AuthorizedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class AddUserpagemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPage",
                table: "UserPage");

            migrationBuilder.RenameTable(
                name: "UserPage",
                newName: "UserPages");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserPages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPages",
                table: "UserPages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserPages_UserId",
                table: "UserPages",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPages_Users_UserId",
                table: "UserPages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPages_Users_UserId",
                table: "UserPages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPages",
                table: "UserPages");

            migrationBuilder.DropIndex(
                name: "IX_UserPages_UserId",
                table: "UserPages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserPages");

            migrationBuilder.RenameTable(
                name: "UserPages",
                newName: "UserPage");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPage",
                table: "UserPage",
                column: "Id");
        }
    }
}

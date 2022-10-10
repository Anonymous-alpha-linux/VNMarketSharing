using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class adduserIdanduserPageIdpropertytoreplytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Replies",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserPageId",
                table: "Replies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Replies_UserId",
                table: "Replies",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_UserPageId",
                table: "Replies",
                column: "UserPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_Users_UserId",
                table: "Replies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Replies_UserPages_UserPageId",
                table: "Replies",
                column: "UserPageId",
                principalTable: "UserPages",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Replies_Users_UserId",
                table: "Replies");

            migrationBuilder.DropForeignKey(
                name: "FK_Replies_UserPages_UserPageId",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_UserId",
                table: "Replies");

            migrationBuilder.DropIndex(
                name: "IX_Replies_UserPageId",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "UserPageId",
                table: "Replies");
        }
    }
}

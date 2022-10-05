using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class Modifypropofuserpage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BannerUrl",
                table: "UserPages");

            migrationBuilder.AddColumn<int>(
                name: "BannerUrlId",
                table: "UserPages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserPages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageAvatarId",
                table: "UserPages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "UserPages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPages_BannerUrlId",
                table: "UserPages",
                column: "BannerUrlId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPages_PageAvatarId",
                table: "UserPages",
                column: "PageAvatarId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPages_Attachments_BannerUrlId",
                table: "UserPages",
                column: "BannerUrlId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPages_Attachments_PageAvatarId",
                table: "UserPages",
                column: "PageAvatarId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductClassfiyDetails_ProductClassifyTypes_ClassifyTypeKeyId",
                table: "ProductClassfiyDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductClassfiyDetails_ProductClassifyTypes_ClassifyTypeValueId",
                table: "ProductClassfiyDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPages_Attachments_BannerUrlId",
                table: "UserPages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPages_Attachments_PageAvatarId",
                table: "UserPages");

            migrationBuilder.DropIndex(
                name: "IX_UserPages_BannerUrlId",
                table: "UserPages");

            migrationBuilder.DropIndex(
                name: "IX_UserPages_PageAvatarId",
                table: "UserPages");

            migrationBuilder.DropColumn(
                name: "BannerUrlId",
                table: "UserPages");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserPages");

            migrationBuilder.DropColumn(
                name: "PageAvatarId",
                table: "UserPages");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "UserPages");

            migrationBuilder.AddColumn<string>(
                name: "BannerUrl",
                table: "UserPages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductClassfiyDetails_ProductClassifyTypes_ClassifyTypeKeyId",
                table: "ProductClassfiyDetails",
                column: "ClassifyTypeKeyId",
                principalTable: "ProductClassifyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductClassfiyDetails_ProductClassifyTypes_ClassifyTypeValueId",
                table: "ProductClassfiyDetails",
                column: "ClassifyTypeValueId",
                principalTable: "ProductClassifyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

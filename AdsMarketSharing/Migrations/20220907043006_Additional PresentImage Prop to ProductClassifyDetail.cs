using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class AdditionalPresentImageProptoProductClassifyDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PresentImageId",
                table: "ProductClassfiyDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassfiyDetails_PresentImageId",
                table: "ProductClassfiyDetails",
                column: "PresentImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductClassfiyDetails_Attachments_PresentImageId",
                table: "ProductClassfiyDetails",
                column: "PresentImageId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductClassfiyDetails_Attachments_PresentImageId",
                table: "ProductClassfiyDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductClassfiyDetails_PresentImageId",
                table: "ProductClassfiyDetails");

            migrationBuilder.DropColumn(
                name: "PresentImageId",
                table: "ProductClassfiyDetails");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class additionalproducttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttachment_Attachments_AttachmentId",
                table: "ProductAttachments");

            migrationBuilder.AddColumn<bool>(
                name: "HasDeleted",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Products",
                nullable: false,
                defaultValue: "New");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttachments_Attachments_AttachmentId",
                table: "ProductAttachments",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttachments_Attachments_AttachmentId",
                table: "ProductAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttachments_Products_ProductId",
                table: "ProductAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductAttachments",
                table: "ProductAttachments");

            migrationBuilder.DropColumn(
                name: "HasDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "ProductAttachments",
                newName: "ProductAttachment");

            migrationBuilder.RenameIndex(
                name: "IX_ProductAttachments_AttachmentId",
                table: "ProductAttachment",
                newName: "IX_ProductAttachment_AttachmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductAttachment",
                table: "ProductAttachment",
                columns: new[] { "ProductId", "AttachmentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttachment_Attachments_AttachmentId",
                table: "ProductAttachment",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttachment_Products_ProductId",
                table: "ProductAttachment",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class fixtherelationshipofproductandattachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Products_ProductId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ProductId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Attachments");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Attachments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Attachments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));

            migrationBuilder.CreateTable(
                name: "ProductAttachment",
                columns: table => new
                {
                    ProductId = table.Column<int>(nullable: false),
                    AttachmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttachment", x => new { x.ProductId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_ProductAttachment_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAttachment_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttachment_AttachmentId",
                table: "ProductAttachment",
                column: "AttachmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAttachment");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Attachments");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ProductId",
                table: "Attachments",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Products_ProductId",
                table: "Attachments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

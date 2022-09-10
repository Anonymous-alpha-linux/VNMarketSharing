using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class AddCreatedAtandUpdatedAtinProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassfiyDetails_ClassifyTypeKeyId",
                table: "ProductClassfiyDetails",
                column: "ClassifyTypeKeyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductClassfiyDetails",
                table: "ProductClassfiyDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductClassfiyDetails_ClassifyTypeKeyId",
                table: "ProductClassfiyDetails");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductClassfiyDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductClassfiyDetails",
                table: "ProductClassfiyDetails",
                columns: new[] { "ClassifyTypeKeyId", "ClassifyTypeValueId" });
        }
    }
}

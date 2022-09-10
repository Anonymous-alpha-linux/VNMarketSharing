using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class AddProductClassifyClassifyTypeandClassifyTypeDetailEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasAccepted",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProductClassifies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ProductId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassifies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductClassifies_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductClassifyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ProductClassifyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassifyTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductClassifyTypes_ProductClassifies_ProductClassifyId",
                        column: x => x.ProductClassifyId,
                        principalTable: "ProductClassifies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductClassfiyDetails",
                columns: table => new
                {
                    ClassifyTypeKeyId = table.Column<int>(nullable: false),
                    ClassifyTypeValueId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    Inventory = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductClassfiyDetails", x => new { x.ClassifyTypeKeyId, x.ClassifyTypeValueId });
                    table.ForeignKey(
                        name: "FK_ProductClassfiyDetails_ProductClassifyTypes_ClassifyTypeKeyId",
                        column: x => x.ClassifyTypeKeyId,
                        principalTable: "ProductClassifyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductClassfiyDetails_ProductClassifyTypes_ClassifyTypeValueId",
                        column: x => x.ClassifyTypeValueId,
                        principalTable: "ProductClassifyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassfiyDetails_ClassifyTypeValueId",
                table: "ProductClassfiyDetails",
                column: "ClassifyTypeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassifies_ProductId",
                table: "ProductClassifies",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductClassifyTypes_ProductClassifyId",
                table: "ProductClassifyTypes",
                column: "ProductClassifyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductClassfiyDetails");

            migrationBuilder.DropTable(
                name: "ProductClassifyTypes");

            migrationBuilder.DropTable(
                name: "ProductClassifies");

            migrationBuilder.DropColumn(
                name: "HasAccepted",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

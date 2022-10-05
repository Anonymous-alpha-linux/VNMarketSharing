using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class addhasPaidpropertytoinvoicetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPaid",
                table: "Invoices",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPaid",
                table: "Invoices");
        }
    }
}

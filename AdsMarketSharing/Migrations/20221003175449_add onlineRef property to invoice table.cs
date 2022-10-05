using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class addonlineRefpropertytoinvoicetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnlineRef",
                table: "Invoices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineRef",
                table: "Invoices");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class adddistrictandwardpropertytoReceiverAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Province",
                table: "ReceiverAddresses");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserPages",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "ReceiverAddresses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "ReceiverAddresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "District",
                table: "ReceiverAddresses");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "ReceiverAddresses");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserPages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "ReceiverAddresses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

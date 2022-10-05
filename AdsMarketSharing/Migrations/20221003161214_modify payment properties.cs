using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class modifypaymentproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ReceiverAddresses_AddressId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Orders",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Invoices",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Shipping",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ReceiverAddresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "ReceiverAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ReceiverAddresses_AddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Shipping",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "AddressId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ReceiverAddresses_AddressId",
                table: "Orders",
                column: "AddressId",
                principalTable: "ReceiverAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class modifynullableattachmentIdpropfromusertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Users_Attachments_AttachmentId",
            //    table: "Users");

            //migrationBuilder.DropIndex(
            //    name: "IX_Users_AttachmentId",
            //    table: "Users");

            //migrationBuilder.AlterColumn<int>(
            //    name: "AttachmentId",
            //    table: "Users",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_AttachmentId",
            //    table: "Users",
            //    column: "AttachmentId",
            //    unique: true,
            //    filter: "[AttachmentId] IS NOT NULL");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Users_Attachments_AttachmentId",
            //    table: "Users",
            //    column: "AttachmentId",
            //    principalTable: "Attachments",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Attachments_AttachmentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AttachmentId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "AttachmentId",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AttachmentId",
                table: "Users",
                column: "AttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Attachments_AttachmentId",
                table: "Users",
                column: "AttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

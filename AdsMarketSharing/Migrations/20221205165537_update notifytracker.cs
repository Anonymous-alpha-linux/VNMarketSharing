using Microsoft.EntityFrameworkCore.Migrations;

namespace AdsMarketSharing.Migrations
{
    public partial class updatenotifytracker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifytrackers_Notifications_NotificationId",
                table: "Notifytrackers");

            migrationBuilder.DropIndex(
                name: "IX_Notifytrackers_NotificationId",
                table: "Notifytrackers");

            migrationBuilder.DropColumn(
                name: "NotificationId",
                table: "Notifytrackers");

            migrationBuilder.CreateIndex(
                name: "IX_Notifytrackers_NotifyId",
                table: "Notifytrackers",
                column: "NotifyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifytrackers_Notifications_NotifyId",
                table: "Notifytrackers",
                column: "NotifyId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifytrackers_Notifications_NotifyId",
                table: "Notifytrackers");

            migrationBuilder.DropIndex(
                name: "IX_Notifytrackers_NotifyId",
                table: "Notifytrackers");

            migrationBuilder.AddColumn<int>(
                name: "NotificationId",
                table: "Notifytrackers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifytrackers_NotificationId",
                table: "Notifytrackers",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifytrackers_Notifications_NotificationId",
                table: "Notifytrackers",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

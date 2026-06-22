using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NarrowCasting_V5.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnouncementScreenAndMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MediaFileId",
                table: "Announcements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScreenId",
                table: "Announcements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_MediaFileId",
                table: "Announcements",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_ScreenId",
                table: "Announcements",
                column: "ScreenId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_MediaFiles_MediaFileId",
                table: "Announcements",
                column: "MediaFileId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Screens_ScreenId",
                table: "Announcements",
                column: "ScreenId",
                principalTable: "Screens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_MediaFiles_MediaFileId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Screens_ScreenId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_MediaFileId",
                table: "Announcements");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_ScreenId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "MediaFileId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "ScreenId",
                table: "Announcements");
        }
    }
}

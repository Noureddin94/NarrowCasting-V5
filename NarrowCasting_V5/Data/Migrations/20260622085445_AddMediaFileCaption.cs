using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NarrowCasting_V5.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaFileCaption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "MediaFiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "MediaFiles");
        }
    }
}

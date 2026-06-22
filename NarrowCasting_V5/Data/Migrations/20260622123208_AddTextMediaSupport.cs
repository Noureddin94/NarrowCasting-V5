using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NarrowCasting_V5.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTextMediaSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextContent",
                table: "MediaFiles",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextContent",
                table: "MediaFiles");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api_sk1_02files.Migrations
{
    /// <inheritdoc />
    public partial class InlineThumbnails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Thumbnail",
                table: "Items",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Items");
        }
    }
}

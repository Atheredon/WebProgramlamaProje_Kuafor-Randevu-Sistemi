using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaförRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AvailabletyAddedToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Users",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available",
                table: "Users");
        }
    }
}

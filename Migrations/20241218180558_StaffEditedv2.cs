using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaförRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class StaffEditedv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffServices");

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Services",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_StaffId",
                table: "Services",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_StaffId",
                table: "Services",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_StaffId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_StaffId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Services");

            migrationBuilder.CreateTable(
                name: "StaffServices",
                columns: table => new
                {
                    ServicesId = table.Column<int>(type: "integer", nullable: false),
                    StaffId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffServices", x => new { x.ServicesId, x.StaffId });
                    table.ForeignKey(
                        name: "FK_StaffServices_Services_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffServices_Users_StaffId",
                        column: x => x.StaffId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StaffServices_StaffId",
                table: "StaffServices",
                column: "StaffId");
        }
    }
}

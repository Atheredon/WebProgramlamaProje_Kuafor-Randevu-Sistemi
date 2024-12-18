using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuaförRandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class StaffEditedv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_StaffId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_StaffId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Services");

            migrationBuilder.AddColumn<int>(
                name: "SpecialtyId",
                table: "Users",
                type: "integer",
                nullable: true);

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
                name: "IX_Users_SpecialtyId",
                table: "Users",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffServices_StaffId",
                table: "StaffServices",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Services_SpecialtyId",
                table: "Users",
                column: "SpecialtyId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Services_SpecialtyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "StaffServices");

            migrationBuilder.DropIndex(
                name: "IX_Users_SpecialtyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

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
    }
}

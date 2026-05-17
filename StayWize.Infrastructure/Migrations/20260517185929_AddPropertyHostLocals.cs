using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayWize.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyHostLocals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "HostLocals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PropertyHostLocals",
                columns: table => new
                {
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostLocalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyHostLocals", x => new { x.PropertyId, x.HostLocalId });
                    table.ForeignKey(
                        name: "FK_PropertyHostLocals_HostLocals_HostLocalId",
                        column: x => x.HostLocalId,
                        principalTable: "HostLocals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyHostLocals_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyHostLocals_HostLocalId",
                table: "PropertyHostLocals",
                column: "HostLocalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyHostLocals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HostLocals");
        }
    }
}

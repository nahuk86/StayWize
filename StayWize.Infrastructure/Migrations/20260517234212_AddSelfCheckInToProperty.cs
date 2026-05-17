using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayWize.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSelfCheckInToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelfCheckIn",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSelfCheckIn",
                table: "Properties");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayWize.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLockDeviceIdToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LockDeviceId",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockDeviceId",
                table: "Properties");
        }
    }
}

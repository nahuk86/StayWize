using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayWize.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientRegistrationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientRegistrationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRegistrationRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientRegistrationRequests_DocumentNumber",
                table: "ClientRegistrationRequests",
                column: "DocumentNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRegistrationRequests_Email",
                table: "ClientRegistrationRequests",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRegistrationRequests_Status",
                table: "ClientRegistrationRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientRegistrationRequests");
        }
    }
}

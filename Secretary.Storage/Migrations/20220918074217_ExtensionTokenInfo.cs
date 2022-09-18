using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Secretary.Storage.Migrations
{
    public partial class ExtensionTokenInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokenCreationTime",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TokenExpirationSeconds",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenCreationTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TokenExpirationSeconds",
                table: "Users");
        }
    }
}

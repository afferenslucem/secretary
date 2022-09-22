using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Secretary.Storage.Migrations
{
    public partial class UserAddRemindLogTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RemindLogTime",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemindLogTime",
                table: "Users");
        }
    }
}

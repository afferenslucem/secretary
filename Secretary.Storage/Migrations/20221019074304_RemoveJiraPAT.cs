using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Secretary.Storage.Migrations
{
    public partial class RemoveJiraPAT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JiraPersonalAccessToken",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JiraPersonalAccessToken",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Secretary.Storage.Migrations
{
    public partial class AddedEventLogEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_ChatId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "Documents",
                newName: "UserChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_ChatId",
                table: "Documents",
                newName: "IX_Documents_UserChatId");

            migrationBuilder.CreateTable(
                name: "EventLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UserChatId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventLogs_Users_UserChatId",
                        column: x => x.UserChatId,
                        principalTable: "Users",
                        principalColumn: "ChatId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventLogs_UserChatId",
                table: "EventLogs",
                column: "UserChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_UserChatId",
                table: "Documents",
                column: "UserChatId",
                principalTable: "Users",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_UserChatId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "EventLogs");

            migrationBuilder.RenameColumn(
                name: "UserChatId",
                table: "Documents",
                newName: "ChatId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_UserChatId",
                table: "Documents",
                newName: "IX_Documents_ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_ChatId",
                table: "Documents",
                column: "ChatId",
                principalTable: "Users",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

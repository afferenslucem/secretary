using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Secretary.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddedForeignEmailDocumentKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Emails_DocumentId",
                table: "Emails",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ChatId",
                table: "Documents",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_ChatId",
                table: "Documents",
                column: "ChatId",
                principalTable: "Users",
                principalColumn: "ChatId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_Documents_DocumentId",
                table: "Emails",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_ChatId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_Documents_DocumentId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_DocumentId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ChatId",
                table: "Documents");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectLicenta.Migrations
{
    /// <inheritdoc />
    public partial class AddedMessagesList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_SongId",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SongId",
                table: "Messages",
                column: "SongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_SongId",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SongId",
                table: "Messages",
                column: "SongId",
                unique: true);
        }
    }
}

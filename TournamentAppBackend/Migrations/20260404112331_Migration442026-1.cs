using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class Migration4420261 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchLengthInMinutes",
                table: "Tournaments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchLengthInMinutes",
                table: "Tournaments");
        }
    }
}

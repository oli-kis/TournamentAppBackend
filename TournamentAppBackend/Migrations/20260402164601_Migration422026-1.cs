using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentAppBackend.Migrations
{
    /// <inheritdoc />
    public partial class Migration4220261 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "KnockoutStarted",
                table: "Tournaments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "KnockoutTeamCount",
                table: "Tournaments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "KnockoutMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    BracketRound = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    AwayTeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    Played = table.Column<bool>(type: "boolean", nullable: false),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsThirdPlace = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnockoutMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnockoutMatches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlacementMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TournamentId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    AwayTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    HomeScore = table.Column<int>(type: "integer", nullable: true),
                    AwayScore = table.Column<int>(type: "integer", nullable: true),
                    Played = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacementMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlacementMatches_Tournaments_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "Tournaments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnockoutMatches_TournamentId",
                table: "KnockoutMatches",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlacementMatches_TournamentId",
                table: "PlacementMatches",
                column: "TournamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnockoutMatches");

            migrationBuilder.DropTable(
                name: "PlacementMatches");

            migrationBuilder.DropColumn(
                name: "KnockoutStarted",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "KnockoutTeamCount",
                table: "Tournaments");
        }
    }
}

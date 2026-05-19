using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentPlatformSystemWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserThemePreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "preferences_setup_completed",
                table: "user_details",
                type: "boolean",
                nullable: false,
                defaultValueSql: "false");

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                table: "tournament_theme",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_tournament_theme_preference",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    theme_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_tournament_theme_preference_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_tournament_theme_preference_theme_id_fkey",
                        column: x => x.theme_id,
                        principalTable: "tournament_theme",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "user_tournament_theme_preference_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_tournament_theme_preference_theme_id",
                table: "user_tournament_theme_preference",
                column: "theme_id");

            migrationBuilder.CreateIndex(
                name: "unique_user_theme_preference",
                table: "user_tournament_theme_preference",
                columns: new[] { "user_id", "theme_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_tournament_theme_preference");

            migrationBuilder.DropColumn(
                name: "preferences_setup_completed",
                table: "user_details");

            migrationBuilder.DropColumn(
                name: "image_url",
                table: "tournament_theme");
        }
    }
}

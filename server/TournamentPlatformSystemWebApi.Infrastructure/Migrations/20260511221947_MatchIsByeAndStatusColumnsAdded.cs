using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentPlatformSystemWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MatchIsByeAndStatusColumnsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBye",
                table: "match",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "match",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBye",
                table: "match");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "match");
        }
    }
}

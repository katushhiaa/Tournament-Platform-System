using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace TournamentPlatformSystemWebApi.Infrastructure.Migrations
{
    public partial class MatchIsByeAndStatusColumnsAdded : Migration
    {
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
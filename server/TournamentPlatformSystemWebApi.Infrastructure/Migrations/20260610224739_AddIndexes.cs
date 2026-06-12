using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentPlatformSystemWebApi.Infrastructure.Migrations
{
    
    public partial class AddIndexes : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_tournament_status",
                table: "tournament",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_refresh_token_token",
                table: "refresh_token",
                column: "token");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_tournament_status",
                table: "tournament");

            migrationBuilder.DropIndex(
                name: "idx_refresh_token_token",
                table: "refresh_token");
        }
    }
}

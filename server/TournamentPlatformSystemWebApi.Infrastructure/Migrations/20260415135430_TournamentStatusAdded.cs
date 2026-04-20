using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentPlatformSystemWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TournamentStatusAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:tournament_status", "registration_open,registration_closed,in_progress,completed");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "tournament",
                type: "tournament_status",
                nullable: false,
                defaultValue: "registration_open");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "tournament");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:tournament_status", "REGISTRATION_OPEN,REGISTRATION_CLOSED,IN_PROGRESS,COMPLETED")
                .OldAnnotation("Npgsql:Enum:tournament_status.tournament_status_type", "registration_open,registration_closed,in_progress,completed");
        }
    }
}

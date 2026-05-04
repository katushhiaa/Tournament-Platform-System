using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TournamentPlatformSystemWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_state",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("account_state_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tournament_theme",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournament_theme_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_organizer = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    account_state_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_account_state_id_fkey",
                        column: x => x.account_state_id,
                        principalTable: "account_state",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    jwt_id = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    expires_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("refresh_token_pkey", x => x.id);
                    table.ForeignKey(
                        name: "refresh_token_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tournament",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    organizer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    theme_id = table.Column<Guid>(type: "uuid", nullable: false),
                    max_teams = table.Column<int>(type: "integer", nullable: false),
                    background_img = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    registration_deadline = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    conditions = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tournament_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tournament_organizer_id_fkey",
                        column: x => x.organizer_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "tournament_theme_id_fkey",
                        column: x => x.theme_id,
                        principalTable: "tournament_theme",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_details_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_details_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_phone",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_phone_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_phone_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "team",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tournament_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_disqualified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("team_pkey", x => x.id);
                    table.ForeignKey(
                        name: "team_tournament_id_fkey",
                        column: x => x.tournament_id,
                        principalTable: "tournament",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "match",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    tournament_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_a_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_b_id = table.Column<Guid>(type: "uuid", nullable: true),
                    winner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    level = table.Column<int>(type: "integer", nullable: false),
                    order_number = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    team_a_score = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    team_b_score = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    is_valid = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("match_pkey", x => x.id);
                    table.ForeignKey(
                        name: "match_team_a_id_fkey",
                        column: x => x.team_a_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "match_team_b_id_fkey",
                        column: x => x.team_b_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "match_tournament_id_fkey",
                        column: x => x.tournament_id,
                        principalTable: "tournament",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "match_winner_id_fkey",
                        column: x => x.winner_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_team",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    team_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_team_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_team_team_id_fkey",
                        column: x => x.team_id,
                        principalTable: "team",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "user_team_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "account_state_name_key",
                table: "account_state",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_match_is_valid",
                table: "match",
                column: "is_valid");

            migrationBuilder.CreateIndex(
                name: "idx_match_level",
                table: "match",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "idx_match_team_a_id",
                table: "match",
                column: "team_a_id");

            migrationBuilder.CreateIndex(
                name: "idx_match_team_b_id",
                table: "match",
                column: "team_b_id");

            migrationBuilder.CreateIndex(
                name: "idx_match_tournament_id",
                table: "match",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "idx_match_winner_id",
                table: "match",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "unique_match_position",
                table: "match",
                columns: new[] { "tournament_id", "level", "order_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_user_id",
                table: "refresh_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_team_is_disqualified",
                table: "team",
                column: "is_disqualified");

            migrationBuilder.CreateIndex(
                name: "idx_team_tournament_id",
                table: "team",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "unique_team_name_per_tournament",
                table: "team",
                columns: new[] { "name", "tournament_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_tournament_organizer_id",
                table: "tournament",
                column: "organizer_id");

            migrationBuilder.CreateIndex(
                name: "idx_tournament_registration_deadline",
                table: "tournament",
                column: "registration_deadline");

            migrationBuilder.CreateIndex(
                name: "idx_tournament_start_date",
                table: "tournament",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "idx_tournament_theme_id",
                table: "tournament",
                column: "theme_id");

            migrationBuilder.CreateIndex(
                name: "tournament_theme_name_key",
                table: "tournament_theme",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_account_state_id",
                table: "user",
                column: "account_state_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_deleted_at",
                table: "user",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "idx_user_is_organizer",
                table: "user",
                column: "is_organizer");

            migrationBuilder.CreateIndex(
                name: "idx_user_details_email",
                table: "user_details",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "idx_user_details_user_id",
                table: "user_details",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "user_details_email_key",
                table: "user_details",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_details_user_id_key",
                table: "user_details",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_user_phone_phone_number",
                table: "user_phone",
                column: "phone_number");

            migrationBuilder.CreateIndex(
                name: "idx_user_phone_user_id",
                table: "user_phone",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_team_team_id",
                table: "user_team",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_team_user_id",
                table: "user_team",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "unique_user_team",
                table: "user_team",
                columns: new[] { "user_id", "team_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "user_details");

            migrationBuilder.DropTable(
                name: "user_phone");

            migrationBuilder.DropTable(
                name: "user_team");

            migrationBuilder.DropTable(
                name: "team");

            migrationBuilder.DropTable(
                name: "tournament");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "tournament_theme");

            migrationBuilder.DropTable(
                name: "account_state");
        }
    }
}

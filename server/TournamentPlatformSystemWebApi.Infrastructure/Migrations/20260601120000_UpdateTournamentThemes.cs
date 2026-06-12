using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TournamentPlatformSystemWebApi.Infrastructure.Context;
#nullable disable
namespace TournamentPlatformSystemWebApi.Infrastructure.Migrations
{
    [DbContext(typeof(TournamentdbContext))]
    [Migration("20260601120000_UpdateTournamentThemes")]
    public partial class UpdateTournamentThemes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/chess_sport_image.png' WHERE name = 'Chess';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/tennis_sport_image.png' WHERE name = 'Tennis';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/shooting_sport_image.png' WHERE name = 'Shooting';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/boxing_sport_image.png' WHERE name = 'Boxing';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/rocket_league_sport_image.png' WHERE name = 'Rocket League';
INSERT INTO tournament_theme (name, image_url)
SELECT 'Armwrestling', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/armwrestling_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Armwrestling');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Badminton', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/badminton_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Badminton');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Billiards', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/billiards_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Billiards');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Darts', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/darts_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Darts');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Fencing', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/fencing_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Fencing');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Judo', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/judo_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Judo');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Karate', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/karate_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Karate');
INSERT INTO tournament_theme (name, image_url)
SELECT 'MMA', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/mma_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'MMA');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Muay Thai', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/muay_thai_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Muay Thai');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Table Tennis', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/table_tennis_sport_image.png'
WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Table Tennis');
");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/chess.jpg' WHERE name = 'Chess';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/tennis.jpg' WHERE name = 'Tennis';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/shooting.jpg' WHERE name = 'Shooting';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/boxing.jpg' WHERE name = 'Boxing';
UPDATE tournament_theme SET image_url = 'https://storage.googleapis.com/tournament-zvytiaga-images/rocket-league.jpg' WHERE name = 'Rocket League';
DELETE FROM tournament_theme t
WHERE t.name IN (
    'Armwrestling',
    'Badminton',
    'Billiards',
    'Darts',
    'Fencing',
    'Judo',
    'Karate',
    'MMA',
    'Muay Thai',
    'Table Tennis'
)
AND NOT EXISTS (SELECT 1 FROM tournament WHERE theme_id = t.id)
AND NOT EXISTS (SELECT 1 FROM user_tournament_theme_preference WHERE theme_id = t.id);
");
        }
    }
}
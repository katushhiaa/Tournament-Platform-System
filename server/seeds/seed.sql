INSERT INTO account_state (name, description)
SELECT 'active', 'User account is active'
WHERE NOT EXISTS (SELECT 1 FROM account_state WHERE name = 'active');
INSERT INTO account_state (name, description)
SELECT 'inactive', 'User account is inactive'
WHERE NOT EXISTS (SELECT 1 FROM account_state WHERE name = 'inactive');
INSERT INTO account_state (name, description)
SELECT 'suspended', 'User account is suspended'
WHERE NOT EXISTS (SELECT 1 FROM account_state WHERE name = 'suspended');
INSERT INTO account_state (name, description)
SELECT 'banned', 'User account is banned'
WHERE NOT EXISTS (SELECT 1 FROM account_state WHERE name = 'banned');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Armwrestling', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/armwrestling_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Armwrestling');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Badminton', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/badminton_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Badminton');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Billiards', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/billiards_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Billiards');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Boxing', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/boxing_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Boxing');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Chess', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/chess_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Chess');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Darts', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/darts_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Darts');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Fencing', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/fencing_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Fencing');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Judo', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/judo_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Judo');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Karate', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/karate_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Karate');
INSERT INTO tournament_theme (name, image_url)
SELECT 'MMA', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/mma_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'MMA');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Muay Thai', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/muay_thai_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Muay Thai');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Rocket League', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/rocket_league_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Rocket League');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Shooting', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/shooting_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Shooting');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Table Tennis', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/table_tennis_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Table Tennis');
INSERT INTO tournament_theme (name, image_url)
SELECT 'Tennis', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/tennis_sport_image.png' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Tennis');

INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    '00000000-0000-0000-0000-000000000001'::uuid,
    'John Smith',
    '$2a$12$ExmFCWGakMcYGC5KcD65pe9EcGWyJqBwJwyasBwjvw90LoIzVzakW',
    true,
    id
FROM account_state 
WHERE name = 'active'
AND NOT EXISTS (SELECT 1 FROM "user" WHERE id = '00000000-0000-0000-0000-000000000001'::uuid);


INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    '00000000-0000-0000-0000-000000000002'::uuid,
    'Jane Williams',
    '$2a$12$ExmFCWGakMcYGC5KcD65pe9EcGWyJqBwJwyasBwjvw90LoIzVzakW',
    true,
    id
FROM account_state 
WHERE name = 'active'
AND NOT EXISTS (SELECT 1 FROM "user" WHERE id = '00000000-0000-0000-0000-000000000002'::uuid);


INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    ('00000000-0000-0000-0000-0000000000' || LPAD(players.num::text, 2, '0'))::uuid,
    players.name,
    '$2a$12$ExmFCWGakMcYGC5KcD65pe9EcGWyJqBwJwyasBwjvw90LoIzVzakW',
    false,
    (SELECT id FROM account_state WHERE name = 'active')
FROM (
    VALUES
    (3, 'Alex Johnson'),
    (4, 'Michael Brown'),
    (5, 'Sarah Davis'),
    (6, 'David Miller'),
    (7, 'Emily Wilson'),
    (8, 'Chris Anderson'),
    (9, 'Lisa Thomas'),
    (10, 'Kevin Martinez'),
    (11, 'Jessica Garcia'),
    (12, 'Ryan Lee')
) AS players(num, name)
WHERE NOT EXISTS (
    SELECT 1 FROM "user" 
    WHERE id = ('00000000-0000-0000-0000-0000000000' || LPAD(players.num::text, 2, '0'))::uuid
);
INSERT INTO tournament (
    name,
    organizer_id,
    theme_id,
    max_teams,
    background_img,
    start_date,
    registration_deadline,
    end_date,
    description,
    conditions,
    status
)
SELECT
    'Spring Chess Open',
    u.id,
    t.id,
    16,
    t.image_url,
    '2026-06-10 10:00:00',
    '2026-06-05 23:59:00',
    '2026-06-12 18:00:00',
    'A fast-paced weekend Swiss-to-elimination showcase for intermediate players.',
    'Open to ages 16+. Time control: 10+5. Fair play rules enforced; bring your own clock if possible.',
    0
FROM "user" u
JOIN tournament_theme t ON t.name = 'Chess'
WHERE u.id = '00000000-0000-0000-0000-000000000001'::uuid
    AND NOT EXISTS (SELECT 1 FROM tournament WHERE name = 'Spring Chess Open');
INSERT INTO tournament (
    name,
    organizer_id,
    theme_id,
    max_teams,
    background_img,
    start_date,
    registration_deadline,
    end_date,
    description,
    conditions,
    status
)
SELECT
    'City Tennis Cup',
    u.id,
    t.id,
    32,
    t.image_url,
    '2026-07-03 09:00:00',
    '2026-06-25 20:00:00',
    '2026-07-05 20:00:00',
    'Outdoor summer cup with group play and a knockout final.',
    'Bring your own racket. Match format: best of 3 short sets. Dress code: light sportwear.',
    0
FROM "user" u
JOIN tournament_theme t ON t.name = 'Tennis'
WHERE u.id = '00000000-0000-0000-0000-000000000002'::uuid
    AND NOT EXISTS (SELECT 1 FROM tournament WHERE name = 'City Tennis Cup');
INSERT INTO tournament (
    name,
    organizer_id,
    theme_id,
    max_teams,
    background_img,
    start_date,
    registration_deadline,
    end_date,
    description,
    conditions,
    status
)
SELECT
    'Rocket League Night League',
    u.id,
    t.id,
    8,
    t.image_url,
    '2026-06-20 19:00:00',
    '2026-06-18 23:00:00',
    '2026-06-20 23:00:00',
    'Evening bracket for casual teams with quick rounds and highlight matches.',
    'All players must join the event Discord. Best of 1 until semifinals, best of 3 for finals.',
    0
FROM "user" u
JOIN tournament_theme t ON t.name = 'Rocket League'
WHERE u.id = '00000000-0000-0000-0000-000000000001'::uuid
    AND NOT EXISTS (SELECT 1 FROM tournament WHERE name = 'Rocket League Night League');
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000001'::uuid,
    'john.smith@example.com',
    '1985-03-15'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000001'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000002'::uuid,
    'jane.williams@example.com',
    '1990-07-22'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000002'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000003'::uuid,
    'alex.johnson@example.com',
    '1995-01-10'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000003'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000004'::uuid,
    'michael.brown@example.com',
    '1992-05-18'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000004'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000005'::uuid,
    'sarah.davis@example.com',
    '1998-11-30'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000005'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000006'::uuid,
    'david.miller@example.com',
    '1988-09-25'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000006'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000007'::uuid,
    'emily.wilson@example.com',
    '1996-02-14'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000007'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000008'::uuid,
    'chris.anderson@example.com',
    '1991-04-08'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000008'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000009'::uuid,
    'lisa.thomas@example.com',
    '1994-08-19'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000009'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000010'::uuid,
    'kevin.martinez@example.com',
    '1993-12-03'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000010'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000011'::uuid,
    'jessica.garcia@example.com',
    '1997-06-27'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000011'::uuid);
INSERT INTO user_details (user_id, email, date_of_birth)
SELECT 
    '00000000-0000-0000-0000-000000000012'::uuid,
    'ryan.lee@example.com',
    '1989-10-11'
WHERE NOT EXISTS (SELECT 1 FROM user_details WHERE user_id = '00000000-0000-0000-0000-000000000012'::uuid);
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000001'::uuid, '+1-555-0101'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000001'::uuid AND phone_number = '+1-555-0101');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000001'::uuid, '+1-555-0102'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000001'::uuid AND phone_number = '+1-555-0102');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000002'::uuid, '+1-555-0201'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000002'::uuid AND phone_number = '+1-555-0201');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000003'::uuid, '+1-555-0301'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000003'::uuid AND phone_number = '+1-555-0301');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000004'::uuid, '+1-555-0401'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000004'::uuid AND phone_number = '+1-555-0401');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000005'::uuid, '+1-555-0501'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000005'::uuid AND phone_number = '+1-555-0501');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000006'::uuid, '+1-555-0601'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000006'::uuid AND phone_number = '+1-555-0601');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000007'::uuid, '+1-555-0701'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000007'::uuid AND phone_number = '+1-555-0701');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000008'::uuid, '+1-555-0801'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000008'::uuid AND phone_number = '+1-555-0801');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000009'::uuid, '+1-555-0901'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000009'::uuid AND phone_number = '+1-555-0901');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000010'::uuid, '+1-555-1001'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000010'::uuid AND phone_number = '+1-555-1001');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000011'::uuid, '+1-555-1101'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000011'::uuid AND phone_number = '+1-555-1101');
INSERT INTO user_phone (user_id, phone_number)
SELECT '00000000-0000-0000-0000-000000000012'::uuid, '+1-555-1201'
WHERE NOT EXISTS (SELECT 1 FROM user_phone WHERE user_id = '00000000-0000-0000-0000-000000000012'::uuid AND phone_number = '+1-555-1201');
INSERT INTO user_tournament_theme_preference (user_id, theme_id)
SELECT u.id, t.id
FROM "user" u
CROSS JOIN tournament_theme t
WHERE u.id = '00000000-0000-0000-0000-000000000001'::uuid AND t.name = 'Chess'
AND NOT EXISTS (SELECT 1 FROM user_tournament_theme_preference WHERE user_id = u.id AND theme_id = t.id);
INSERT INTO user_tournament_theme_preference (user_id, theme_id)
SELECT u.id, t.id
FROM "user" u
CROSS JOIN tournament_theme t
WHERE u.id = '00000000-0000-0000-0000-000000000002'::uuid AND t.name = 'Tennis'
AND NOT EXISTS (SELECT 1 FROM user_tournament_theme_preference WHERE user_id = u.id AND theme_id = t.id);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000001'::uuid, 'Knights', id FROM tournament WHERE name = 'Spring Chess Open'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000001'::uuid);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000002'::uuid, 'Bishops', id FROM tournament WHERE name = 'Spring Chess Open'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000002'::uuid);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000003'::uuid, 'Rooks', id FROM tournament WHERE name = 'Spring Chess Open'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000003'::uuid);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000004'::uuid, 'Pawns', id FROM tournament WHERE name = 'Spring Chess Open'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000004'::uuid);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000005'::uuid, 'Aces', id FROM tournament WHERE name = 'City Tennis Cup'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000005'::uuid);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000006'::uuid, 'Smashers', id FROM tournament WHERE name = 'City Tennis Cup'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000006'::uuid);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000007'::uuid, 'Boosters', id FROM tournament WHERE name = 'Rocket League Night League'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000007'::uuid);
INSERT INTO team (id, name, tournament_id)
SELECT '11111111-0000-0000-0000-000000000008'::uuid, 'Aerials', id FROM tournament WHERE name = 'Rocket League Night League'
AND NOT EXISTS (SELECT 1 FROM team WHERE id = '11111111-0000-0000-0000-000000000008'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000003'::uuid, '11111111-0000-0000-0000-000000000001'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000003'::uuid AND team_id = '11111111-0000-0000-0000-000000000001'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000004'::uuid, '11111111-0000-0000-0000-000000000002'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000004'::uuid AND team_id = '11111111-0000-0000-0000-000000000002'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000005'::uuid, '11111111-0000-0000-0000-000000000003'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000005'::uuid AND team_id = '11111111-0000-0000-0000-000000000003'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000006'::uuid, '11111111-0000-0000-0000-000000000004'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000006'::uuid AND team_id = '11111111-0000-0000-0000-000000000004'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000007'::uuid, '11111111-0000-0000-0000-000000000005'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000007'::uuid AND team_id = '11111111-0000-0000-0000-000000000005'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000008'::uuid, '11111111-0000-0000-0000-000000000006'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000008'::uuid AND team_id = '11111111-0000-0000-0000-000000000006'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000009'::uuid, '11111111-0000-0000-0000-000000000007'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000009'::uuid AND team_id = '11111111-0000-0000-0000-000000000007'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000010'::uuid, '11111111-0000-0000-0000-000000000007'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000010'::uuid AND team_id = '11111111-0000-0000-0000-000000000007'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000011'::uuid, '11111111-0000-0000-0000-000000000008'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000011'::uuid AND team_id = '11111111-0000-0000-0000-000000000008'::uuid);
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000012'::uuid, '11111111-0000-0000-0000-000000000008'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000012'::uuid AND team_id = '11111111-0000-0000-0000-000000000008'::uuid);
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, level, order_number, start_date)
SELECT 
    '22222222-0000-0000-0000-000000000001'::uuid, id, '11111111-0000-0000-0000-000000000001'::uuid, '11111111-0000-0000-0000-000000000002'::uuid, 1, 1, '2026-06-10 10:30:00'
FROM tournament WHERE name = 'Spring Chess Open'
AND NOT EXISTS (SELECT 1 FROM match WHERE id = '22222222-0000-0000-0000-000000000001'::uuid);
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, level, order_number, start_date)
SELECT 
    '22222222-0000-0000-0000-000000000002'::uuid, id, '11111111-0000-0000-0000-000000000003'::uuid, '11111111-0000-0000-0000-000000000004'::uuid, 1, 2, '2026-06-10 10:30:00'
FROM tournament WHERE name = 'Spring Chess Open'
AND NOT EXISTS (SELECT 1 FROM match WHERE id = '22222222-0000-0000-0000-000000000002'::uuid);
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, level, order_number, start_date)
SELECT 
    '22222222-0000-0000-0000-000000000003'::uuid, id, '11111111-0000-0000-0000-000000000005'::uuid, '11111111-0000-0000-0000-000000000006'::uuid, 1, 1, '2026-07-03 10:00:00'
FROM tournament WHERE name = 'City Tennis Cup'
AND NOT EXISTS (SELECT 1 FROM match WHERE id = '22222222-0000-0000-0000-000000000003'::uuid);
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, level, order_number, start_date)
SELECT 
    '22222222-0000-0000-0000-000000000004'::uuid, id, '11111111-0000-0000-0000-000000000007'::uuid, '11111111-0000-0000-0000-000000000008'::uuid, 1, 1, '2026-06-20 19:30:00'
FROM tournament WHERE name = 'Rocket League Night League'
AND NOT EXISTS (SELECT 1 FROM match WHERE id = '22222222-0000-0000-0000-000000000004'::uuid);
INSERT INTO tournament (
    id, name, organizer_id, theme_id, max_teams, background_img, start_date, registration_deadline, end_date, description, conditions, status
)
SELECT
    '33333333-0000-0000-0000-000000000001'::uuid, 'Winter Boxing Championship', u.id, t.id, 4, t.image_url, '2025-12-01 10:00:00', '2025-11-25 23:59:00', '2025-12-05 18:00:00', 'Historical completed tournament.', 'Pro rules.', 3
FROM "user" u
JOIN tournament_theme t ON t.name = 'Boxing'
WHERE u.id = '00000000-0000-0000-0000-000000000001'::uuid
    AND NOT EXISTS (SELECT 1 FROM tournament WHERE id = '33333333-0000-0000-0000-000000000001'::uuid);
INSERT INTO team (id, name, tournament_id) VALUES
('44444444-0000-0000-0000-000000000001'::uuid, 'Tigers', '33333333-0000-0000-0000-000000000001'::uuid),
('44444444-0000-0000-0000-000000000002'::uuid, 'Lions', '33333333-0000-0000-0000-000000000001'::uuid),
('44444444-0000-0000-0000-000000000003'::uuid, 'Bears', '33333333-0000-0000-0000-000000000001'::uuid),
('44444444-0000-0000-0000-000000000004'::uuid, 'Wolves', '33333333-0000-0000-0000-000000000001'::uuid)
ON CONFLICT DO NOTHING;
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, "Status") VALUES
('55555555-0000-0000-0000-000000000001'::uuid, '33333333-0000-0000-0000-000000000001'::uuid, '44444444-0000-0000-0000-000000000001'::uuid, '44444444-0000-0000-0000-000000000002'::uuid, '44444444-0000-0000-0000-000000000001'::uuid, 1, 1, '2025-12-01 10:30:00', 3, 1, 'completed'),
('55555555-0000-0000-0000-000000000002'::uuid, '33333333-0000-0000-0000-000000000001'::uuid, '44444444-0000-0000-0000-000000000003'::uuid, '44444444-0000-0000-0000-000000000004'::uuid, '44444444-0000-0000-0000-000000000004'::uuid, 1, 2, '2025-12-01 12:30:00', 0, 2, 'completed')
ON CONFLICT DO NOTHING;
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, "Status") VALUES
('55555555-0000-0000-0000-000000000003'::uuid, '33333333-0000-0000-0000-000000000001'::uuid, '44444444-0000-0000-0000-000000000001'::uuid, '44444444-0000-0000-0000-000000000004'::uuid, '44444444-0000-0000-0000-000000000001'::uuid, 2, 1, '2025-12-05 15:30:00', 5, 4, 'completed')
ON CONFLICT DO NOTHING;
INSERT INTO tournament (
    id, name, organizer_id, theme_id, max_teams, background_img, start_date, registration_deadline, end_date, description, conditions, status
)
SELECT
    '66666666-0000-0000-0000-000000000001'::uuid, 'Summer Billiards Masters', u.id, t.id, 4, t.image_url, CURRENT_TIMESTAMP - INTERVAL '1 day', CURRENT_TIMESTAMP - INTERVAL '2 days', CURRENT_TIMESTAMP + INTERVAL '2 days', 'Ongoing Billiards masterclass.', 'Bring your own cue.', 2
FROM "user" u
JOIN tournament_theme t ON t.name = 'Billiards'
WHERE u.id = '00000000-0000-0000-0000-000000000002'::uuid
    AND NOT EXISTS (SELECT 1 FROM tournament WHERE id = '66666666-0000-0000-0000-000000000001'::uuid);
INSERT INTO team (id, name, tournament_id) VALUES
('77777777-0000-0000-0000-000000000001'::uuid, 'Sharks', '66666666-0000-0000-0000-000000000001'::uuid),
('77777777-0000-0000-0000-000000000002'::uuid, 'Eagles', '66666666-0000-0000-0000-000000000001'::uuid),
('77777777-0000-0000-0000-000000000003'::uuid, 'Falcons', '66666666-0000-0000-0000-000000000001'::uuid),
('77777777-0000-0000-0000-000000000004'::uuid, 'Hawks', '66666666-0000-0000-0000-000000000001'::uuid)
ON CONFLICT DO NOTHING;
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, "Status") VALUES
('88888888-0000-0000-0000-000000000001'::uuid, '66666666-0000-0000-0000-000000000001'::uuid, '77777777-0000-0000-0000-000000000001'::uuid, '77777777-0000-0000-0000-000000000002'::uuid, '77777777-0000-0000-0000-000000000001'::uuid, 1, 1, CURRENT_TIMESTAMP - INTERVAL '12 hours', 8, 5, 'completed')
ON CONFLICT DO NOTHING;
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, level, order_number, start_date, team_a_score, team_b_score, "Status") VALUES
('88888888-0000-0000-0000-000000000002'::uuid, '66666666-0000-0000-0000-000000000001'::uuid, '77777777-0000-0000-0000-000000000003'::uuid, '77777777-0000-0000-0000-000000000004'::uuid, 1, 2, CURRENT_TIMESTAMP - INTERVAL '2 hours', 3, 4, 'in_progress')
ON CONFLICT DO NOTHING;
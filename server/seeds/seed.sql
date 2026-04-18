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

INSERT INTO tournament_theme (name)
SELECT 'Chess' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Chess');
INSERT INTO tournament_theme (name)
SELECT 'Tennis' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Tennis');
INSERT INTO tournament_theme (name)
SELECT 'Shooting' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Shooting');
INSERT INTO tournament_theme (name)
SELECT 'Boxing' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Boxing');
INSERT INTO tournament_theme (name)
SELECT 'Rocket League' WHERE NOT EXISTS (SELECT 1 FROM tournament_theme WHERE name = 'Rocket League');
-- ============================================================================
-- SEED DATA FOR TOURNAMENT PLATFORM
-- ============================================================================
-- This file populates the database with realistic test data.
-- Run AFTER init_db.sql has created all tables and enums.
-- Uses WHERE NOT EXISTS checks to prevent duplicate data on re-runs.
-- ============================================================================

-- ============================================================================
-- 1. INSERT USERS (Mix of organizers and regular players)
-- ============================================================================

INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    '00000000-0000-0000-0000-000000000001'::uuid,
    'John Smith',
    '$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS',
    true,
    id
FROM account_state 
WHERE name = 'active'
AND NOT EXISTS (SELECT 1 FROM "user" WHERE id = '00000000-0000-0000-0000-000000000001'::uuid);

INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    '00000000-0000-0000-0000-000000000002'::uuid,
    'Jane Williams',
    '$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS',
    true,
    id
FROM account_state 
WHERE name = 'active'
AND NOT EXISTS (SELECT 1 FROM "user" WHERE id = '00000000-0000-0000-0000-000000000002'::uuid);

-- Regular Players (IDs: 3-12)
INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    ('00000000-0000-0000-0000-0000000000' || LPAD(players.num::text, 2, '0'))::uuid,
    players.name,
    '$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS',
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

-- ============================================================================
-- 2. INSERT USER DETAILS
-- ============================================================================

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

-- ============================================================================
-- 3. INSERT USER PHONE NUMBERS
-- ============================================================================

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

-- ============================================================================
-- 4. INSERT TOURNAMENTS
-- ============================================================================

INSERT INTO tournament (id, name, organizer_id, theme_id, max_teams, background_img, start_date, registration_deadline, end_date, description, conditions, status)
SELECT 
    '10000000-0000-0000-0000-000000000001'::uuid,
    'Spring Chess Championship 2026',
    '00000000-0000-0000-0000-000000000001'::uuid,
    id,
    16,
    'https://example.com/chess-bg.jpg',
    '2026-05-15 10:00:00',
    '2026-05-10 23:59:59',
    '2026-05-17 18:00:00',
    'A competitive chess tournament for intermediate and advanced players',
    'Players must be 16+ years old. Time limit: 30 minutes per game.',
    'registration_open'::tournament_status
FROM tournament_theme 
WHERE name = 'Chess'
AND NOT EXISTS (SELECT 1 FROM tournament WHERE id = '10000000-0000-0000-0000-000000000001'::uuid);

INSERT INTO tournament (id, name, organizer_id, theme_id, max_teams, background_img, start_date, registration_deadline, end_date, description, conditions, status)
SELECT 
    '10000000-0000-0000-0000-000000000002'::uuid,
    'Summer Tennis Masters 2026',
    '00000000-0000-0000-0000-000000000002'::uuid,
    id,
    8,
    'https://example.com/tennis-bg.jpg',
    '2026-04-20 09:00:00',
    '2026-04-18 23:59:59',
    '2026-04-25 17:00:00',
    'High-energy tennis tournament with singles and doubles matches',
    'Professional rackets required. Best of 3 sets.',
    'in_progress'::tournament_status
FROM tournament_theme 
WHERE name = 'Tennis'
AND NOT EXISTS (SELECT 1 FROM tournament WHERE id = '10000000-0000-0000-0000-000000000002'::uuid);

INSERT INTO tournament (id, name, organizer_id, theme_id, max_teams, background_img, start_date, registration_deadline, end_date, description, conditions, status)
SELECT 
    '10000000-0000-0000-0000-000000000003'::uuid,
    'Rocket League Spring Series 2026',
    '00000000-0000-0000-0000-000000000001'::uuid,
    id,
    12,
    'https://example.com/rocketleague-bg.jpg',
    '2026-05-01 19:00:00',
    '2026-04-25 23:59:59',
    '2026-05-03 22:00:00',
    'Online Rocket League tournament with 3v3 matches',
    'Teams must have at least 3 players. All players must be registered on Epic Games.',
    'registration_closed'::tournament_status
FROM tournament_theme 
WHERE name = 'Rocket League'
AND NOT EXISTS (SELECT 1 FROM tournament WHERE id = '10000000-0000-0000-0000-000000000003'::uuid);

INSERT INTO tournament (id, name, organizer_id, theme_id, max_teams, background_img, start_date, registration_deadline, end_date, description, conditions, status)
SELECT 
    '10000000-0000-0000-0000-000000000004'::uuid,
    'Winter Boxing Championship 2026',
    '00000000-0000-0000-0000-000000000002'::uuid,
    id,
    8,
    'https://example.com/boxing-bg.jpg',
    '2026-02-01 18:00:00',
    '2026-01-25 23:59:59',
    '2026-02-05 20:00:00',
    'Amateur boxing championship with weight classes',
    'Competitors must be 18+. All safety equipment provided.',
    'completed'::tournament_status
FROM tournament_theme 
WHERE name = 'Boxing'
AND NOT EXISTS (SELECT 1 FROM tournament WHERE id = '10000000-0000-0000-0000-000000000004'::uuid);

-- ============================================================================
-- 5. INSERT TEAMS
-- ============================================================================

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000001'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Alpha Knights', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000001'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000002'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Beta Bishops', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000002'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000003'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Gamma Rooks', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000003'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000004'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Delta Queens', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000004'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000005'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Ace Strikers', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000005'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000006'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Net Masters', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000006'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000007'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Court Kings', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000007'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000008'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Rally Warriors', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000008'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000009'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Turbo Boosters', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000009'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000010'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Goal Hammers', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000010'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000011'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Sky Riders', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000011'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000012'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Flip Masters', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000012'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000013'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Iron Fists', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000013'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000014'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Thunder Punchers', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000014'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000015'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Lightning Strikers', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000015'::uuid);

INSERT INTO team (id, tournament_id, name, is_disqualified)
SELECT '20000000-0000-0000-0000-000000000016'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Champion Force', false
WHERE NOT EXISTS (SELECT 1 FROM team WHERE id = '20000000-0000-0000-0000-000000000016'::uuid);

-- ============================================================================
-- 6. INSERT USER-TEAM MEMBERSHIPS
-- ============================================================================

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000001'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000003'::uuid AND team_id = '20000000-0000-0000-0000-000000000001'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000001'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000004'::uuid AND team_id = '20000000-0000-0000-0000-000000000001'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000002'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000005'::uuid AND team_id = '20000000-0000-0000-0000-000000000002'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000002'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000006'::uuid AND team_id = '20000000-0000-0000-0000-000000000002'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000003'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000007'::uuid AND team_id = '20000000-0000-0000-0000-000000000003'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000003'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000008'::uuid AND team_id = '20000000-0000-0000-0000-000000000003'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000009'::uuid, '20000000-0000-0000-0000-000000000004'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000009'::uuid AND team_id = '20000000-0000-0000-0000-000000000004'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000010'::uuid, '20000000-0000-0000-0000-000000000004'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000010'::uuid AND team_id = '20000000-0000-0000-0000-000000000004'::uuid);

-- Tennis Teams
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000005'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000003'::uuid AND team_id = '20000000-0000-0000-0000-000000000005'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000005'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000004'::uuid AND team_id = '20000000-0000-0000-0000-000000000005'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000006'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000005'::uuid AND team_id = '20000000-0000-0000-0000-000000000006'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000006'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000006'::uuid AND team_id = '20000000-0000-0000-0000-000000000006'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000007'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000007'::uuid AND team_id = '20000000-0000-0000-0000-000000000007'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000007'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000008'::uuid AND team_id = '20000000-0000-0000-0000-000000000007'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000009'::uuid, '20000000-0000-0000-0000-000000000008'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000009'::uuid AND team_id = '20000000-0000-0000-0000-000000000008'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000010'::uuid, '20000000-0000-0000-0000-000000000008'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000010'::uuid AND team_id = '20000000-0000-0000-0000-000000000008'::uuid);

-- Rocket League Teams
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000009'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000003'::uuid AND team_id = '20000000-0000-0000-0000-000000000009'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000009'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000004'::uuid AND team_id = '20000000-0000-0000-0000-000000000009'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000011'::uuid, '20000000-0000-0000-0000-000000000009'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000011'::uuid AND team_id = '20000000-0000-0000-0000-000000000009'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000010'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000005'::uuid AND team_id = '20000000-0000-0000-0000-000000000010'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000010'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000006'::uuid AND team_id = '20000000-0000-0000-0000-000000000010'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000012'::uuid, '20000000-0000-0000-0000-000000000010'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000012'::uuid AND team_id = '20000000-0000-0000-0000-000000000010'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000011'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000007'::uuid AND team_id = '20000000-0000-0000-0000-000000000011'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000011'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000008'::uuid AND team_id = '20000000-0000-0000-0000-000000000011'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000009'::uuid, '20000000-0000-0000-0000-000000000011'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000009'::uuid AND team_id = '20000000-0000-0000-0000-000000000011'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000010'::uuid, '20000000-0000-0000-0000-000000000012'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000010'::uuid AND team_id = '20000000-0000-0000-0000-000000000012'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000011'::uuid, '20000000-0000-0000-0000-000000000012'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000011'::uuid AND team_id = '20000000-0000-0000-0000-000000000012'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000012'::uuid, '20000000-0000-0000-0000-000000000012'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000012'::uuid AND team_id = '20000000-0000-0000-0000-000000000012'::uuid);

-- Boxing Teams
INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000013'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000003'::uuid AND team_id = '20000000-0000-0000-0000-000000000013'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000014'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000004'::uuid AND team_id = '20000000-0000-0000-0000-000000000014'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000015'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000005'::uuid AND team_id = '20000000-0000-0000-0000-000000000015'::uuid);

INSERT INTO user_team (user_id, team_id)
SELECT '00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000016'::uuid
WHERE NOT EXISTS (SELECT 1 FROM user_team WHERE user_id = '00000000-0000-0000-0000-000000000006'::uuid AND team_id = '20000000-0000-0000-0000-000000000016'::uuid);

-- ============================================================================
-- 7. INSERT MATCHES
-- ============================================================================

INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
SELECT 
    '30000000-0000-0000-0000-000000000001'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, 1, 1, '2026-04-24 10:00:00', 2, 1, true
WHERE NOT EXISTS (SELECT 1 FROM match WHERE id = '30000000-0000-0000-0000-000000000001'::uuid);

INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
SELECT 
    '30000000-0000-0000-0000-000000000002'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, '20000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000008'::uuid, 1, 2, '2026-04-24 12:00:00', 1, 2, true
WHERE NOT EXISTS (SELECT 1 FROM match WHERE id = '30000000-0000-0000-0000-000000000002'::uuid);

INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
SELECT 
    '30000000-0000-0000-0000-000000000003'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, 2, 1, '2026-04-25 14:00:00', 2, 0, true
WHERE NOT EXISTS (SELECT 1 FROM match WHERE id = '30000000-0000-0000-0000-000000000003'::uuid);

INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
SELECT 
    '30000000-0000-0000-0000-000000000004'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, '20000000-0000-0000-0000-000000000014'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, 1, 1, '2026-02-02 18:00:00', 10, 8, true
WHERE NOT EXISTS (SELECT 1 FROM match WHERE id = '30000000-0000-0000-0000-000000000004'::uuid);

INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
SELECT 
    '30000000-0000-0000-0000-000000000005'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000015'::uuid, '20000000-0000-0000-0000-000000000016'::uuid, '20000000-0000-0000-0000-000000000016'::uuid, 1, 2, '2026-02-02 19:00:00', 7, 12, true
WHERE NOT EXISTS (SELECT 1 FROM match WHERE id = '30000000-0000-0000-0000-000000000005'::uuid);

INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
SELECT 
    '30000000-0000-0000-0000-000000000006'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, '20000000-0000-0000-0000-000000000016'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, 2, 1, '2026-02-05 19:30:00', 11, 9, true
WHERE NOT EXISTS (SELECT 1 FROM match WHERE id = '30000000-0000-0000-0000-000000000006'::uuid);

-- ============================================================================
-- SEED DATA FOR TOURNAMENT PLATFORM
-- ============================================================================
-- This file populates the database with realistic test data.
-- Run AFTER init_db.sql has created all tables and enums.
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
FROM account_state WHERE name = 'active';

INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    '00000000-0000-0000-0000-000000000002'::uuid,
    'Jane Williams',
    '$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS',
    true,
    id
FROM account_state WHERE name = 'active';

-- Regular Players (IDs: 3-12)
INSERT INTO "user" (id, full_name, password_hash, is_organizer, account_state_id)
SELECT 
    ('00000000-0000-0000-0000-00000000000' || LPAD(num::text, 2, '0'))::uuid,
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
) AS players(num, name);

-- ============================================================================
-- 2. INSERT USER DETAILS
-- ============================================================================

INSERT INTO user_details (user_id, email, date_of_birth)
VALUES
    ('00000000-0000-0000-0000-000000000001'::uuid, 'john.smith@example.com', '1985-03-15'),
    ('00000000-0000-0000-0000-000000000002'::uuid, 'jane.williams@example.com', '1990-07-22'),
    ('00000000-0000-0000-0000-000000000003'::uuid, 'alex.johnson@example.com', '1995-01-10'),
    ('00000000-0000-0000-0000-000000000004'::uuid, 'michael.brown@example.com', '1992-05-18'),
    ('00000000-0000-0000-0000-000000000005'::uuid, 'sarah.davis@example.com', '1998-11-30'),
    ('00000000-0000-0000-0000-000000000006'::uuid, 'david.miller@example.com', '1988-09-25'),
    ('00000000-0000-0000-0000-000000000007'::uuid, 'emily.wilson@example.com', '1996-02-14'),
    ('00000000-0000-0000-0000-000000000008'::uuid, 'chris.anderson@example.com', '1991-04-08'),
    ('00000000-0000-0000-0000-000000000009'::uuid, 'lisa.thomas@example.com', '1994-08-19'),
    ('00000000-0000-0000-0000-000000000010'::uuid, 'kevin.martinez@example.com', '1993-12-03'),
    ('00000000-0000-0000-0000-000000000011'::uuid, 'jessica.garcia@example.com', '1997-06-27'),
    ('00000000-0000-0000-0000-000000000012'::uuid, 'ryan.lee@example.com', '1989-10-11');

-- ============================================================================
-- 3. INSERT USER PHONE NUMBERS
-- ============================================================================

INSERT INTO user_phone (user_id, phone_number)
VALUES
    ('00000000-0000-0000-0000-000000000001'::uuid, '+1-555-0101'),
    ('00000000-0000-0000-0000-000000000001'::uuid, '+1-555-0102'),
    ('00000000-0000-0000-0000-000000000002'::uuid, '+1-555-0201'),
    ('00000000-0000-0000-0000-000000000003'::uuid, '+1-555-0301'),
    ('00000000-0000-0000-0000-000000000004'::uuid, '+1-555-0401'),
    ('00000000-0000-0000-0000-000000000005'::uuid, '+1-555-0501'),
    ('00000000-0000-0000-0000-000000000006'::uuid, '+1-555-0601'),
    ('00000000-0000-0000-0000-000000000007'::uuid, '+1-555-0701'),
    ('00000000-0000-0000-0000-000000000008'::uuid, '+1-555-0801'),
    ('00000000-0000-0000-0000-000000000009'::uuid, '+1-555-0901'),
    ('00000000-0000-0000-0000-000000000010'::uuid, '+1-555-1001'),
    ('00000000-0000-0000-0000-000000000011'::uuid, '+1-555-1101'),
    ('00000000-0000-0000-0000-000000000012'::uuid, '+1-555-1201');

-- ============================================================================
-- 4. INSERT TOURNAMENTS (WITH CORRECTED DATES)
-- ============================================================================

-- Chess Tournament: Registration Open
-- CORRECTED: registration_deadline (May 10) <= start_date (May 15)
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
    'REGISTRATION_OPEN'::tournament_status
FROM tournament_theme WHERE name = 'Chess';

-- Tennis Tournament: In Progress
-- CORRECTED: registration_deadline (Apr 18) <= start_date (Apr 20)
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
    'IN_PROGRESS'::tournament_status
FROM tournament_theme WHERE name = 'Tennis';

-- Rocket League Tournament: Registration Closed
-- CORRECTED: registration_deadline (Apr 25) <= start_date (May 1)
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
    'REGISTRATION_CLOSED'::tournament_status
FROM tournament_theme WHERE name = 'Rocket League';

-- Boxing Tournament: Completed
-- CORRECTED: registration_deadline (Jan 25) <= start_date (Feb 1)
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
    'COMPLETED'::tournament_status
FROM tournament_theme WHERE name = 'Boxing';

-- ============================================================================
-- 5. INSERT TEAMS
-- ============================================================================

INSERT INTO team (id, tournament_id, name, is_disqualified)
VALUES
    ('20000000-0000-0000-0000-000000000001'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Alpha Knights', false),
    ('20000000-0000-0000-0000-000000000002'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Beta Bishops', false),
    ('20000000-0000-0000-0000-000000000003'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Gamma Rooks', false),
    ('20000000-0000-0000-0000-000000000004'::uuid, '10000000-0000-0000-0000-000000000001'::uuid, 'Delta Queens', false),
    ('20000000-0000-0000-0000-000000000005'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Ace Strikers', false),
    ('20000000-0000-0000-0000-000000000006'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Net Masters', false),
    ('20000000-0000-0000-0000-000000000007'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Court Kings', false),
    ('20000000-0000-0000-0000-000000000008'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, 'Rally Warriors', false),
    ('20000000-0000-0000-0000-000000000009'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Turbo Boosters', false),
    ('20000000-0000-0000-0000-000000000010'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Goal Hammers', false),
    ('20000000-0000-0000-0000-000000000011'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Sky Riders', false),
    ('20000000-0000-0000-0000-000000000012'::uuid, '10000000-0000-0000-0000-000000000003'::uuid, 'Flip Masters', false),
    ('20000000-0000-0000-0000-000000000013'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Iron Fists', false),
    ('20000000-0000-0000-0000-000000000014'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Thunder Punchers', false),
    ('20000000-0000-0000-0000-000000000015'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Lightning Strikers', false),
    ('20000000-0000-0000-0000-000000000016'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, 'Champion Force', false);

-- ============================================================================
-- 6. INSERT USER-TEAM MEMBERSHIPS
-- ============================================================================

INSERT INTO user_team (user_id, team_id)
VALUES
    -- Chess Teams
    ('00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000001'::uuid),
    ('00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000001'::uuid),
    ('00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000002'::uuid),
    ('00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000002'::uuid),
    ('00000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000003'::uuid),
    ('00000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000003'::uuid),
    ('00000000-0000-0000-0000-000000000009'::uuid, '20000000-0000-0000-0000-000000000004'::uuid),
    ('00000000-0000-0000-0000-000000000010'::uuid, '20000000-0000-0000-0000-000000000004'::uuid),
    -- Tennis Teams
    ('00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000005'::uuid),
    ('00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000005'::uuid),
    ('00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000006'::uuid),
    ('00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000006'::uuid),
    ('00000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000007'::uuid),
    ('00000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000007'::uuid),
    ('00000000-0000-0000-0000-000000000009'::uuid, '20000000-0000-0000-0000-000000000008'::uuid),
    ('00000000-0000-0000-0000-000000000010'::uuid, '20000000-0000-0000-0000-000000000008'::uuid),
    -- Rocket League Teams
    ('00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000009'::uuid),
    ('00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000009'::uuid),
    ('00000000-0000-0000-0000-000000000011'::uuid, '20000000-0000-0000-0000-000000000009'::uuid),
    ('00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000010'::uuid),
    ('00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000010'::uuid),
    ('00000000-0000-0000-0000-000000000012'::uuid, '20000000-0000-0000-0000-000000000010'::uuid),
    ('00000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000011'::uuid),
    ('00000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000011'::uuid),
    ('00000000-0000-0000-0000-000000000009'::uuid, '20000000-0000-0000-0000-000000000011'::uuid),
    ('00000000-0000-0000-0000-000000000010'::uuid, '20000000-0000-0000-0000-000000000012'::uuid),
    ('00000000-0000-0000-0000-000000000011'::uuid, '20000000-0000-0000-0000-000000000012'::uuid),
    ('00000000-0000-0000-0000-000000000012'::uuid, '20000000-0000-0000-0000-000000000012'::uuid),
    -- Boxing Teams
    ('00000000-0000-0000-0000-000000000003'::uuid, '20000000-0000-0000-0000-000000000013'::uuid),
    ('00000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000014'::uuid),
    ('00000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000015'::uuid),
    ('00000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000016'::uuid);

-- ============================================================================
-- 7. INSERT MATCHES
-- ============================================================================

-- Tennis Tournament Matches (Completed tournament for demonstration)
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
VALUES
    ('30000000-0000-0000-0000-000000000001'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000006'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, 1, 1, '2026-04-24 10:00:00', 2, 1, true),
    ('30000000-0000-0000-0000-000000000002'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, '20000000-0000-0000-0000-000000000007'::uuid, '20000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000008'::uuid, 1, 2, '2026-04-24 12:00:00', 1, 2, true),
    ('30000000-0000-0000-0000-000000000003'::uuid, '10000000-0000-0000-0000-000000000002'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, '20000000-0000-0000-0000-000000000008'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, 2, 1, '2026-04-25 14:00:00', 2, 0, true);

-- Boxing Tournament Matches
INSERT INTO match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid)
VALUES
    ('30000000-0000-0000-0000-000000000004'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, '20000000-0000-0000-0000-000000000014'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, 1, 1, '2026-02-02 18:00:00', 10, 8, true),
    ('30000000-0000-0000-0000-000000000005'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000015'::uuid, '20000000-0000-0000-0000-000000000016'::uuid, '20000000-0000-0000-0000-000000000016'::uuid, 1, 2, '2026-02-02 19:00:00', 7, 12, true),
    ('30000000-0000-0000-0000-000000000006'::uuid, '10000000-0000-0000-0000-000000000004'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, '20000000-0000-0000-0000-000000000016'::uuid, '20000000-0000-0000-0000-000000000013'::uuid, 2, 1, '2026-02-05 19:30:00', 11, 9, true);

-- ============================================================================
-- 8. VERIFY SEED DATA INSTALLATION
-- ============================================================================

SELECT 'Users' as entity, COUNT(*) as count FROM "user" UNION ALL
SELECT 'User Details', COUNT(*) FROM user_details UNION ALL
SELECT 'User Phones', COUNT(*) FROM user_phone UNION ALL
SELECT 'Tournaments', COUNT(*) FROM tournament UNION ALL
SELECT 'Teams', COUNT(*) FROM team UNION ALL
SELECT 'User-Team Memberships', COUNT(*) FROM user_team UNION ALL
SELECT 'Matches', COUNT(*) FROM match
ORDER BY entity;

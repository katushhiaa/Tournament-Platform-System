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

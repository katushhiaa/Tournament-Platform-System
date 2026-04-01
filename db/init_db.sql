-- ============================================================================
-- 1. CREATE ENUM TYPES
-- ============================================================================

CREATE TYPE tournament_status AS ENUM (
    'REGISTRATION_OPEN',
    'REGISTRATION_CLOSED',
    'IN_PROGRESS',
    'COMPLETED'
);

-- ============================================================================
-- 2. CREATE ACCOUNT_STATE TABLE (Lookup)
-- ============================================================================

CREATE TABLE account_state (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert default account states
INSERT INTO account_state (name, description) VALUES
    ('active', 'User account is active'),
    ('inactive', 'User account is inactive'),
    ('suspended', 'User account is suspended'),
    ('banned', 'User account is banned');

-- ============================================================================
-- 3. CREATE USER TABLE (with soft delete & audit trail)
-- ============================================================================

CREATE TABLE "user" (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    is_organizer BOOLEAN DEFAULT false,
    account_state_id INTEGER NOT NULL REFERENCES account_state(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

CREATE UNIQUE INDEX idx_user_email_not_deleted 
ON "user" (id) 
WHERE deleted_at IS NULL;

CREATE INDEX idx_user_account_state_id ON "user"(account_state_id);
CREATE INDEX idx_user_is_organizer ON "user"(is_organizer);
CREATE INDEX idx_user_deleted_at ON "user"(deleted_at);

-- ============================================================================
-- 4. CREATE USER_DETAILS TABLE (Optional, cascade hard-delete with user)
-- ============================================================================

CREATE TABLE user_details (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL UNIQUE REFERENCES "user"(id) ON DELETE CASCADE,
    email VARCHAR(255) UNIQUE,
    date_of_birth DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_user_details_user_id ON user_details(user_id);
CREATE INDEX idx_user_details_email ON user_details(email);

-- ============================================================================
-- 5. CREATE USER_PHONE TABLE (Multiple phones per user, cascade hard-delete)
-- ============================================================================

CREATE TABLE user_phone (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES "user"(id) ON DELETE CASCADE,
    phone_number VARCHAR(20) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);

CREATE INDEX idx_user_phone_user_id ON user_phone(user_id);
CREATE INDEX idx_user_phone_phone_number ON user_phone(phone_number);

-- ============================================================================
-- 6. CREATE TOURNAMENT_THEME TABLE (Lookup)
-- ============================================================================

CREATE TABLE tournament_theme (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================================================
-- 7. CREATE TOURNAMENT TABLE (with soft delete & audit trail)
-- ============================================================================

CREATE TABLE tournament (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    organizer_id INTEGER REFERENCES "user"(id) ON DELETE SET NULL,
    theme_id INTEGER NOT NULL REFERENCES tournament_theme(id),
    start_date TIMESTAMP NOT NULL,
    registration_deadline TIMESTAMP NOT NULL,
    end_date TIMESTAMP NOT NULL,
    description TEXT,
    conditions TEXT,
    status tournament_status DEFAULT 'REGISTRATION_OPEN',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT valid_registration_deadline CHECK (registration_deadline <= start_date),
    CONSTRAINT valid_end_date CHECK (end_date >= start_date)
);

CREATE INDEX idx_tournament_organizer_id ON tournament(organizer_id);
CREATE INDEX idx_tournament_theme_id ON tournament(theme_id);
CREATE INDEX idx_tournament_status ON tournament(status);
CREATE INDEX idx_tournament_start_date ON tournament(start_date);

-- ============================================================================
-- 8. CREATE TEAM TABLE (Tournament-scoped, immutable deletion)
-- ============================================================================

CREATE TABLE team (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    tournament_id INTEGER NOT NULL REFERENCES tournament(id) ON DELETE CASCADE,
    is_disqualified BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_team_name_per_tournament UNIQUE (name, tournament_id)
);

CREATE INDEX idx_team_tournament_id ON team(tournament_id);
CREATE INDEX idx_team_is_disqualified ON team(is_disqualified);

-- ============================================================================
-- 9. CREATE USER_TEAM TABLE (Junction, immutable membership)
-- ============================================================================

CREATE TABLE user_team (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES "user"(id),
    team_id INTEGER NOT NULL REFERENCES team(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_user_team UNIQUE (user_id, team_id)
);

CREATE INDEX idx_user_team_user_id ON user_team(user_id);
CREATE INDEX idx_user_team_team_id ON user_team(team_id);

-- ============================================================================
-- 10. CREATE MATCH TABLE (Immutable, single elimination)
-- ============================================================================

CREATE TABLE match (
    id SERIAL PRIMARY KEY,
    tournament_id INTEGER NOT NULL REFERENCES tournament(id) ON DELETE CASCADE,
    team_a_id INTEGER NOT NULL REFERENCES team(id),
    team_b_id INTEGER NOT NULL REFERENCES team(id),
    winner_id INTEGER REFERENCES team(id),
    level INTEGER NOT NULL,
    order_number INTEGER NOT NULL,
    start_date TIMESTAMP,
    team_a_score INTEGER DEFAULT 0,
    team_b_score INTEGER DEFAULT 0,
    is_valid BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_match_position UNIQUE (tournament_id, level, order_number),
    CONSTRAINT valid_different_teams CHECK (team_a_id != team_b_id),
    CONSTRAINT valid_winner CHECK (winner_id IS NULL OR winner_id IN (team_a_id, team_b_id))
);

CREATE INDEX idx_match_tournament_id ON match(tournament_id);
CREATE INDEX idx_match_team_a_id ON match(team_a_id);
CREATE INDEX idx_match_team_b_id ON match(team_b_id);
CREATE INDEX idx_match_winner_id ON match(winner_id);
CREATE INDEX idx_match_is_valid ON match(is_valid);

-- ============================================================================
-- 11. CREATE TRIGGERS (Optional, for immutability enforcement)
-- ============================================================================

-- Prevent team deletion
CREATE OR REPLACE FUNCTION prevent_team_deletion()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'Teams cannot be deleted. Mark as disqualified instead.';
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_prevent_team_deletion
BEFORE DELETE ON team
FOR EACH ROW
EXECUTE FUNCTION prevent_team_deletion();

-- Prevent match deletion
CREATE OR REPLACE FUNCTION prevent_match_deletion()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'Matches cannot be deleted. Mark as invalid instead.';
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_prevent_match_deletion
BEFORE DELETE ON match
FOR EACH ROW
EXECUTE FUNCTION prevent_match_deletion();

-- Prevent user_team deletion
CREATE OR REPLACE FUNCTION prevent_user_team_deletion()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'User team memberships cannot be deleted. Keep historical record.';
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_prevent_user_team_deletion
BEFORE DELETE ON user_team
FOR EACH ROW
EXECUTE FUNCTION prevent_user_team_deletion();

-- Hard-delete user_phone on user soft-delete (GDPR compliance)
CREATE OR REPLACE FUNCTION hard_delete_user_phones()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM user_phone WHERE user_id = NEW.id AND deleted_at IS NULL;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_hard_delete_user_phones
AFTER UPDATE ON "user"
FOR EACH ROW
WHEN (OLD.deleted_at IS DISTINCT FROM NEW.deleted_at AND NEW.deleted_at IS NOT NULL)
EXECUTE FUNCTION hard_delete_user_phones();

-- Hard-delete user_details on user soft-delete (GDPR compliance)
CREATE OR REPLACE FUNCTION hard_delete_user_details()
RETURNS TRIGGER AS $$
BEGIN
    DELETE FROM user_details WHERE user_id = NEW.id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_hard_delete_user_details
AFTER UPDATE ON "user"
FOR EACH ROW
WHEN (OLD.deleted_at IS DISTINCT FROM NEW.deleted_at AND NEW.deleted_at IS NOT NULL)
EXECUTE FUNCTION hard_delete_user_details();

-- Update updated_at timestamp
CREATE OR REPLACE FUNCTION update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_update_user_timestamp
BEFORE UPDATE ON "user"
FOR EACH ROW
EXECUTE FUNCTION update_timestamp();

CREATE TRIGGER trigger_update_user_details_timestamp
BEFORE UPDATE ON user_details
FOR EACH ROW
EXECUTE FUNCTION update_timestamp();

CREATE TRIGGER trigger_update_tournament_timestamp
BEFORE UPDATE ON tournament
FOR EACH ROW
EXECUTE FUNCTION update_timestamp();

CREATE TRIGGER trigger_update_team_timestamp
BEFORE UPDATE ON team
FOR EACH ROW
EXECUTE FUNCTION update_timestamp();

CREATE TRIGGER trigger_update_match_timestamp
BEFORE UPDATE ON match
FOR EACH ROW
EXECUTE FUNCTION update_timestamp();

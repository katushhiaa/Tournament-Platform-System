CREATE TABLE account_state (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
INSERT INTO account_state (name, description) VALUES
    ('active', 'User account is active'),
    ('inactive', 'User account is inactive'),
    ('suspended', 'User account is suspended'),
    ('banned', 'User account is banned');
CREATE TABLE "user" (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    is_organizer BOOLEAN DEFAULT false,
    account_state_id UUID NOT NULL REFERENCES account_state(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);
CREATE INDEX idx_user_account_state_id ON "user"(account_state_id);
CREATE INDEX idx_user_is_organizer ON "user"(is_organizer);
CREATE INDEX idx_user_deleted_at ON "user"(deleted_at);
CREATE TABLE user_details (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL UNIQUE REFERENCES "user"(id) ON DELETE CASCADE,
    email VARCHAR(255) UNIQUE NOT NULL,
    date_of_birth DATE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    preferences_setup_completed BOOLEAN DEFAULT false
);
CREATE INDEX idx_user_details_user_id ON user_details(user_id);
CREATE INDEX idx_user_details_email ON user_details(email);
CREATE TABLE user_phone (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES "user"(id) ON DELETE CASCADE,
    phone_number VARCHAR(20) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMP NULL
);
CREATE INDEX idx_user_phone_user_id ON user_phone(user_id);
CREATE INDEX idx_user_phone_phone_number ON user_phone(phone_number);
CREATE TABLE tournament_theme (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL UNIQUE,
    image_url TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
INSERT INTO tournament_theme (name, image_url) VALUES
    ('Armwrestling', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/armwrestling_sport_image.png'),
    ('Badminton', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/badminton_sport_image.png'),
    ('Billiards', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/billiards_sport_image.png'),
    ('Boxing', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/boxing_sport_image.png'),
    ('Chess', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/chess_sport_image.png'),
    ('Darts', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/darts_sport_image.png'),
    ('Fencing', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/fencing_sport_image.png'),
    ('Judo', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/judo_sport_image.png'),
    ('Karate', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/karate_sport_image.png'),
    ('MMA', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/mma_sport_image.png'),
    ('Muay Thai', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/muay_thai_sport_image.png'),
    ('Rocket League', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/rocket_league_sport_image.png'),
    ('Shooting', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/shooting_sport_image.png'),
    ('Table Tennis', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/table_tennis_sport_image.png'),
    ('Tennis', 'https://storage.googleapis.com/tournament-zvytiaga-images/themes/tennis_sport_image.png');
CREATE TABLE user_tournament_theme_preference (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES "user"(id) ON DELETE CASCADE,
    theme_id UUID NOT NULL REFERENCES tournament_theme(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_user_theme_preference UNIQUE (user_id, theme_id)
);
CREATE INDEX idx_user_theme_pref_user_id ON user_tournament_theme_preference(user_id);
CREATE INDEX idx_user_theme_pref_theme_id ON user_tournament_theme_preference(theme_id);
CREATE TABLE tournament (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    organizer_id UUID REFERENCES "user"(id) ON DELETE SET NULL,
    theme_id UUID NOT NULL REFERENCES tournament_theme(id),
    max_teams INTEGER NOT NULL,
    background_img TEXT,
    start_date TIMESTAMP NOT NULL,
    registration_deadline TIMESTAMP NOT NULL,
    end_date TIMESTAMP NOT NULL,
    description TEXT,
    conditions TEXT,
    status INTEGER DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT valid_registration_deadline CHECK (registration_deadline <= start_date),
    CONSTRAINT valid_end_date CHECK (end_date >= start_date),
    CONSTRAINT valid_max_teams CHECK (max_teams >= 2 AND max_teams <= 1024)
);
CREATE INDEX idx_tournament_organizer_id ON tournament(organizer_id);
CREATE INDEX idx_tournament_theme_id ON tournament(theme_id);
CREATE INDEX idx_tournament_status ON tournament(status);
CREATE INDEX idx_tournament_start_date ON tournament(start_date);
CREATE INDEX idx_tournament_registration_deadline ON tournament(registration_deadline);
CREATE TABLE team (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    tournament_id UUID NOT NULL REFERENCES tournament(id) ON DELETE CASCADE,
    is_disqualified BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_team_name_per_tournament UNIQUE (name, tournament_id)
);
CREATE INDEX idx_team_tournament_id ON team(tournament_id);
CREATE INDEX idx_team_is_disqualified ON team(is_disqualified);
CREATE TABLE user_team (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES "user"(id) ON DELETE RESTRICT,
    team_id UUID NOT NULL REFERENCES team(id) ON DELETE RESTRICT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_user_team UNIQUE (user_id, team_id)
);
CREATE INDEX idx_user_team_user_id ON user_team(user_id);
CREATE INDEX idx_user_team_team_id ON user_team(team_id);
CREATE TABLE match (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tournament_id UUID NOT NULL REFERENCES tournament(id) ON DELETE CASCADE,
    team_a_id UUID NOT NULL REFERENCES team(id) ON DELETE RESTRICT,
    team_b_id UUID REFERENCES team(id) ON DELETE SET NULL,
    winner_id UUID REFERENCES team(id) ON DELETE SET NULL,
    level INTEGER NOT NULL,
    order_number INTEGER NOT NULL,
    start_date TIMESTAMP,
    team_a_score INTEGER DEFAULT 0,
    team_b_score INTEGER DEFAULT 0,
    is_valid BOOLEAN DEFAULT true,
    is_bye BOOLEAN DEFAULT false,
    status VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT unique_match_position UNIQUE (tournament_id, level, order_number),
    CONSTRAINT valid_different_teams CHECK (team_a_id != team_b_id),
    CONSTRAINT valid_winner CHECK (winner_id IS NULL OR winner_id = team_a_id OR (team_b_id IS NOT NULL AND winner_id = team_b_id)),
    CONSTRAINT valid_scores CHECK (team_a_score >= 0 AND team_b_score >= 0)
);
CREATE INDEX idx_match_tournament_id ON match(tournament_id);
CREATE INDEX idx_match_team_a_id ON match(team_a_id);
CREATE INDEX idx_match_team_b_id ON match(team_b_id);
CREATE INDEX idx_match_winner_id ON match(winner_id);
CREATE INDEX idx_match_is_valid ON match(is_valid);
CREATE INDEX idx_match_level ON match(level);
CREATE INDEX idx_match_status ON match(status);
CREATE INDEX idx_match_is_bye ON match(is_bye);
CREATE INDEX idx_tournament_dates ON tournament(start_date, end_date);
CREATE INDEX idx_user_created_at ON "user"(created_at);
CREATE TABLE refresh_token (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES "user"(id) ON DELETE CASCADE,
    token VARCHAR(255) NOT NULL,
    jwt_id VARCHAR(255) NOT NULL,
    is_used BOOLEAN DEFAULT false,
    is_revoked BOOLEAN DEFAULT false,
    expires_at TIMESTAMP NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_refresh_token_user_id ON refresh_token(user_id);
CREATE INDEX idx_refresh_token_token ON refresh_token(token);
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
CREATE OR REPLACE FUNCTION prevent_active_tournament_team_deletion()
RETURNS TRIGGER AS $$
DECLARE
    v_status INTEGER;
BEGIN
    SELECT status INTO v_status
    FROM tournament
    WHERE id = OLD.tournament_id;
    IF v_status IN (2, 3) THEN
        RAISE EXCEPTION 'Cannot delete teams from active or completed tournaments';
    END IF;
    RETURN OLD;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER trigger_prevent_active_tournament_team_deletion
BEFORE DELETE ON team
FOR EACH ROW
EXECUTE FUNCTION prevent_active_tournament_team_deletion();
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
SELECT tablename FROM pg_tables 
WHERE schemaname = 'public' 
ORDER BY tablename;
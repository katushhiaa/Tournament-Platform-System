--
-- PostgreSQL database dump
--

\restrict e6k31RJzmvB0f0mnJ8nwjZJePCWkva9UOp8w7IRdI4Zi0ciW28J1DTjqocJuQRZ

-- Dumped from database version 15.17 (Debian 15.17-1.pgdg13+1)
-- Dumped by pg_dump version 15.17 (Debian 15.17-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE ONLY public.user_tournament_theme_preference DROP CONSTRAINT user_tournament_theme_preference_user_id_fkey;
ALTER TABLE ONLY public.user_tournament_theme_preference DROP CONSTRAINT user_tournament_theme_preference_theme_id_fkey;
ALTER TABLE ONLY public.user_team DROP CONSTRAINT user_team_user_id_fkey;
ALTER TABLE ONLY public.user_team DROP CONSTRAINT user_team_team_id_fkey;
ALTER TABLE ONLY public.user_phone DROP CONSTRAINT user_phone_user_id_fkey;
ALTER TABLE ONLY public.user_details DROP CONSTRAINT user_details_user_id_fkey;
ALTER TABLE ONLY public."user" DROP CONSTRAINT user_account_state_id_fkey;
ALTER TABLE ONLY public.tournament DROP CONSTRAINT tournament_theme_id_fkey;
ALTER TABLE ONLY public.tournament DROP CONSTRAINT tournament_organizer_id_fkey;
ALTER TABLE ONLY public.team DROP CONSTRAINT team_tournament_id_fkey;
ALTER TABLE ONLY public.refresh_token DROP CONSTRAINT refresh_token_user_id_fkey;
ALTER TABLE ONLY public.match DROP CONSTRAINT match_winner_id_fkey;
ALTER TABLE ONLY public.match DROP CONSTRAINT match_tournament_id_fkey;
ALTER TABLE ONLY public.match DROP CONSTRAINT match_team_b_id_fkey;
ALTER TABLE ONLY public.match DROP CONSTRAINT match_team_a_id_fkey;
DROP INDEX public.user_details_user_id_key;
DROP INDEX public.user_details_email_key;
DROP INDEX public.unique_user_theme_preference;
DROP INDEX public.unique_user_team;
DROP INDEX public.unique_team_name_per_tournament;
DROP INDEX public.unique_match_position;
DROP INDEX public.tournament_theme_name_key;
DROP INDEX public.idx_user_team_user_id;
DROP INDEX public.idx_user_team_team_id;
DROP INDEX public.idx_user_phone_user_id;
DROP INDEX public.idx_user_phone_phone_number;
DROP INDEX public.idx_user_is_organizer;
DROP INDEX public.idx_user_details_user_id;
DROP INDEX public.idx_user_details_email;
DROP INDEX public.idx_user_deleted_at;
DROP INDEX public.idx_user_account_state_id;
DROP INDEX public.idx_tournament_theme_id;
DROP INDEX public.idx_tournament_status;
DROP INDEX public.idx_tournament_start_date;
DROP INDEX public.idx_tournament_registration_deadline;
DROP INDEX public.idx_tournament_organizer_id;
DROP INDEX public.idx_team_tournament_id;
DROP INDEX public.idx_team_is_disqualified;
DROP INDEX public.idx_refresh_token_token;
DROP INDEX public.idx_match_winner_id;
DROP INDEX public.idx_match_tournament_id;
DROP INDEX public.idx_match_team_b_id;
DROP INDEX public.idx_match_team_a_id;
DROP INDEX public.idx_match_level;
DROP INDEX public.idx_match_is_valid;
DROP INDEX public.account_state_name_key;
DROP INDEX public."IX_user_tournament_theme_preference_theme_id";
DROP INDEX public."IX_refresh_token_user_id";
ALTER TABLE ONLY public.user_tournament_theme_preference DROP CONSTRAINT user_tournament_theme_preference_pkey;
ALTER TABLE ONLY public.user_team DROP CONSTRAINT user_team_pkey;
ALTER TABLE ONLY public."user" DROP CONSTRAINT user_pkey;
ALTER TABLE ONLY public.user_phone DROP CONSTRAINT user_phone_pkey;
ALTER TABLE ONLY public.user_details DROP CONSTRAINT user_details_pkey;
ALTER TABLE ONLY public.tournament_theme DROP CONSTRAINT tournament_theme_pkey;
ALTER TABLE ONLY public.tournament DROP CONSTRAINT tournament_pkey;
ALTER TABLE ONLY public.team DROP CONSTRAINT team_pkey;
ALTER TABLE ONLY public.refresh_token DROP CONSTRAINT refresh_token_pkey;
ALTER TABLE ONLY public.match DROP CONSTRAINT match_pkey;
ALTER TABLE ONLY public.account_state DROP CONSTRAINT account_state_pkey;
ALTER TABLE ONLY public."__EFMigrationsHistory" DROP CONSTRAINT "PK___EFMigrationsHistory";
DROP TABLE public.user_tournament_theme_preference;
DROP TABLE public.user_team;
DROP TABLE public.user_phone;
DROP TABLE public.user_details;
DROP TABLE public."user";
DROP TABLE public.tournament_theme;
DROP TABLE public.tournament;
DROP TABLE public.team;
DROP TABLE public.refresh_token;
DROP TABLE public.match;
DROP TABLE public.account_state;
DROP TABLE public."__EFMigrationsHistory";
SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- Name: account_state; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.account_state (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    name character varying(50) NOT NULL,
    description character varying(255),
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.account_state OWNER TO postgres;

--
-- Name: match; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.match (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    tournament_id uuid NOT NULL,
    team_a_id uuid,
    team_b_id uuid,
    winner_id uuid,
    level integer NOT NULL,
    order_number integer NOT NULL,
    start_date timestamp without time zone,
    team_a_score integer DEFAULT 0,
    team_b_score integer DEFAULT 0,
    is_valid boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    "IsBye" boolean,
    "Status" text
);


ALTER TABLE public.match OWNER TO postgres;

--
-- Name: refresh_token; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.refresh_token (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    token character varying(255) NOT NULL,
    jwt_id character varying(255) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    expires_at timestamp without time zone NOT NULL,
    is_used boolean DEFAULT false NOT NULL,
    is_revoked boolean DEFAULT false NOT NULL
);


ALTER TABLE public.refresh_token OWNER TO postgres;

--
-- Name: team; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.team (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    name character varying(255) NOT NULL,
    tournament_id uuid NOT NULL,
    is_disqualified boolean DEFAULT false,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.team OWNER TO postgres;

--
-- Name: tournament; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tournament (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    name character varying(255) NOT NULL,
    organizer_id uuid,
    theme_id uuid NOT NULL,
    max_teams integer NOT NULL,
    background_img text,
    start_date timestamp without time zone NOT NULL,
    registration_deadline timestamp without time zone NOT NULL,
    end_date timestamp without time zone NOT NULL,
    description text,
    conditions text,
    status integer NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.tournament OWNER TO postgres;

--
-- Name: tournament_theme; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tournament_theme (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    name character varying(100) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    image_url text
);


ALTER TABLE public.tournament_theme OWNER TO postgres;

--
-- Name: user; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."user" (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    full_name character varying(255) NOT NULL,
    password_hash character varying(255) NOT NULL,
    is_organizer boolean DEFAULT false,
    account_state_id uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    deleted_at timestamp without time zone
);


ALTER TABLE public."user" OWNER TO postgres;

--
-- Name: user_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_details (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    email character varying(255) NOT NULL,
    date_of_birth date NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    updated_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    preferences_setup_completed boolean DEFAULT false NOT NULL
);


ALTER TABLE public.user_details OWNER TO postgres;

--
-- Name: user_phone; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_phone (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    phone_number character varying(20) NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    deleted_at timestamp without time zone
);


ALTER TABLE public.user_phone OWNER TO postgres;

--
-- Name: user_team; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_team (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    team_id uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.user_team OWNER TO postgres;

--
-- Name: user_tournament_theme_preference; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.user_tournament_theme_preference (
    id uuid DEFAULT gen_random_uuid() NOT NULL,
    user_id uuid NOT NULL,
    theme_id uuid NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.user_tournament_theme_preference OWNER TO postgres;

--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260504215056_NewInitial	9.0.15
20260511221947_MatchIsByeAndStatusColumnsAdded	9.0.15
20260516100858_AddUserThemePreferences	9.0.15
20260521184755_TeamBInMatchTableNullable	9.0.15
20260601120000_UpdateTournamentThemes	9.0.15
20260610224739_AddIndexes	9.0.15
20260611101053_PendingModelChanges	9.0.15
\.


--
-- Data for Name: account_state; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.account_state (id, name, description, is_active, created_at) FROM stdin;
98bcc273-7c58-4a7f-9b0c-c261831f55c4	active	User account is active	t	2026-06-11 10:21:08.698829
0f8b08b6-6a5b-4a73-8f7e-a6491d37c871	inactive	User account is inactive	t	2026-06-11 10:21:08.701548
dbf8f3ce-8b8c-485b-825a-a5f9350ebf94	suspended	User account is suspended	t	2026-06-11 10:21:08.702321
b446490d-2f34-439b-a4d4-a7cc4d01ac53	banned	User account is banned	t	2026-06-11 10:21:08.703058
\.


--
-- Data for Name: match; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.match (id, tournament_id, team_a_id, team_b_id, winner_id, level, order_number, start_date, team_a_score, team_b_score, is_valid, created_at, updated_at, "IsBye", "Status") FROM stdin;
22222222-0000-0000-0000-000000000001	542347cd-b857-41f2-b1a5-b154a236d9ad	11111111-0000-0000-0000-000000000001	11111111-0000-0000-0000-000000000002	\N	1	1	2026-06-10 10:30:00	0	0	t	2026-06-11 10:21:08.783504	2026-06-11 10:21:08.783504	\N	\N
22222222-0000-0000-0000-000000000002	542347cd-b857-41f2-b1a5-b154a236d9ad	11111111-0000-0000-0000-000000000003	11111111-0000-0000-0000-000000000004	\N	1	2	2026-06-10 10:30:00	0	0	t	2026-06-11 10:21:08.787281	2026-06-11 10:21:08.787281	\N	\N
22222222-0000-0000-0000-000000000003	21d19299-bc58-47b9-98ec-12b70c7e46ed	11111111-0000-0000-0000-000000000005	11111111-0000-0000-0000-000000000006	\N	1	1	2026-07-03 10:00:00	0	0	t	2026-06-11 10:21:08.788684	2026-06-11 10:21:08.788684	\N	\N
22222222-0000-0000-0000-000000000004	7f0249e3-c1cb-4f04-86a5-b39adac5a26d	11111111-0000-0000-0000-000000000007	11111111-0000-0000-0000-000000000008	\N	1	1	2026-06-20 19:30:00	0	0	t	2026-06-11 10:21:08.790312	2026-06-11 10:21:08.790312	\N	\N
\.


--
-- Data for Name: refresh_token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.refresh_token (id, user_id, token, jwt_id, created_at, expires_at, is_used, is_revoked) FROM stdin;
\.


--
-- Data for Name: team; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.team (id, name, tournament_id, is_disqualified, created_at, updated_at) FROM stdin;
11111111-0000-0000-0000-000000000001	Knights	542347cd-b857-41f2-b1a5-b154a236d9ad	f	2026-06-11 10:21:08.758996	2026-06-11 10:21:08.758996
11111111-0000-0000-0000-000000000002	Bishops	542347cd-b857-41f2-b1a5-b154a236d9ad	f	2026-06-11 10:21:08.761046	2026-06-11 10:21:08.761046
11111111-0000-0000-0000-000000000003	Rooks	542347cd-b857-41f2-b1a5-b154a236d9ad	f	2026-06-11 10:21:08.762065	2026-06-11 10:21:08.762065
11111111-0000-0000-0000-000000000004	Pawns	542347cd-b857-41f2-b1a5-b154a236d9ad	f	2026-06-11 10:21:08.763241	2026-06-11 10:21:08.763241
11111111-0000-0000-0000-000000000005	Aces	21d19299-bc58-47b9-98ec-12b70c7e46ed	f	2026-06-11 10:21:08.764337	2026-06-11 10:21:08.764337
11111111-0000-0000-0000-000000000006	Smashers	21d19299-bc58-47b9-98ec-12b70c7e46ed	f	2026-06-11 10:21:08.765518	2026-06-11 10:21:08.765518
11111111-0000-0000-0000-000000000007	Boosters	7f0249e3-c1cb-4f04-86a5-b39adac5a26d	f	2026-06-11 10:21:08.766602	2026-06-11 10:21:08.766602
11111111-0000-0000-0000-000000000008	Aerials	7f0249e3-c1cb-4f04-86a5-b39adac5a26d	f	2026-06-11 10:21:08.767871	2026-06-11 10:21:08.767871
44444444-0000-0000-0000-000000000001	Tigers	33333333-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.794172	2026-06-11 10:21:08.794172
44444444-0000-0000-0000-000000000002	Lions	33333333-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.794172	2026-06-11 10:21:08.794172
44444444-0000-0000-0000-000000000003	Bears	33333333-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.794172	2026-06-11 10:21:08.794172
44444444-0000-0000-0000-000000000004	Wolves	33333333-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.794172	2026-06-11 10:21:08.794172
77777777-0000-0000-0000-000000000001	Sharks	66666666-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.798828	2026-06-11 10:21:08.798828
77777777-0000-0000-0000-000000000002	Eagles	66666666-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.798828	2026-06-11 10:21:08.798828
77777777-0000-0000-0000-000000000003	Falcons	66666666-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.798828	2026-06-11 10:21:08.798828
77777777-0000-0000-0000-000000000004	Hawks	66666666-0000-0000-0000-000000000001	f	2026-06-11 10:21:08.798828	2026-06-11 10:21:08.798828
\.


--
-- Data for Name: tournament; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tournament (id, name, organizer_id, theme_id, max_teams, background_img, start_date, registration_deadline, end_date, description, conditions, status, created_at, updated_at) FROM stdin;
542347cd-b857-41f2-b1a5-b154a236d9ad	Spring Chess Open	00000000-0000-0000-0000-000000000001	5849dfb2-adcd-45cc-83e2-a31d721c7884	16	https://storage.googleapis.com/tournament-zvytiaga-images/themes/chess_sport_image.png	2026-06-10 10:00:00	2026-06-05 23:59:00	2026-06-12 18:00:00	A fast-paced weekend Swiss-to-elimination showcase for intermediate players.	Open to ages 16+. Time control: 10+5. Fair play rules enforced; bring your own clock if possible.	0	2026-06-11 10:21:08.717331	2026-06-11 10:21:08.717331
21d19299-bc58-47b9-98ec-12b70c7e46ed	City Tennis Cup	00000000-0000-0000-0000-000000000002	501584c4-c435-4c3d-9446-979bdf532b5f	32	https://storage.googleapis.com/tournament-zvytiaga-images/themes/tennis_sport_image.png	2026-07-03 09:00:00	2026-06-25 20:00:00	2026-07-05 20:00:00	Outdoor summer cup with group play and a knockout final.	Bring your own racket. Match format: best of 3 short sets. Dress code: light sportwear.	0	2026-06-11 10:21:08.719745	2026-06-11 10:21:08.719745
7f0249e3-c1cb-4f04-86a5-b39adac5a26d	Rocket League Night League	00000000-0000-0000-0000-000000000001	8bab8e39-2e77-44af-8232-4fe31b7b1e45	8	https://storage.googleapis.com/tournament-zvytiaga-images/themes/rocket_league_sport_image.png	2026-06-20 19:00:00	2026-06-18 23:00:00	2026-06-20 23:00:00	Evening bracket for casual teams with quick rounds and highlight matches.	All players must join the event Discord. Best of 1 until semifinals, best of 3 for finals.	0	2026-06-11 10:21:08.721552	2026-06-11 10:21:08.721552
33333333-0000-0000-0000-000000000001	Winter Boxing Championship	00000000-0000-0000-0000-000000000001	ef77529f-e68b-4066-9244-32d451009e80	4	https://storage.googleapis.com/tournament-zvytiaga-images/themes/boxing_sport_image.png	2025-12-01 10:00:00	2025-11-25 23:59:00	2025-12-05 18:00:00	Historical completed tournament.	Pro rules.	3	2026-06-11 10:21:08.792366	2026-06-11 10:21:08.792366
66666666-0000-0000-0000-000000000001	Summer Billiards Masters	00000000-0000-0000-0000-000000000002	d3ce1780-08ca-4bf5-9d1e-bc51129b34ab	4	https://storage.googleapis.com/tournament-zvytiaga-images/themes/billiards_sport_image.png	2026-06-10 10:21:08.796231	2026-06-09 10:21:08.796231	2026-06-13 10:21:08.796231	Ongoing Billiards masterclass.	Bring your own cue.	2	2026-06-11 10:21:08.796231	2026-06-11 10:21:08.796231
\.


--
-- Data for Name: tournament_theme; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tournament_theme (id, name, created_at, image_url) FROM stdin;
3b7cf5d0-bc03-4092-a24a-7900441772c0	Armwrestling	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/armwrestling_sport_image.png
99f5317a-2613-41eb-987f-a45d7e2d7af2	Badminton	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/badminton_sport_image.png
d3ce1780-08ca-4bf5-9d1e-bc51129b34ab	Billiards	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/billiards_sport_image.png
974ab5fe-9077-45fc-aa5c-4b2340224f6f	Darts	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/darts_sport_image.png
933fbda4-4cc5-4255-99ae-167acfc9f917	Fencing	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/fencing_sport_image.png
af6d632c-d624-4ae2-9030-ef48d8c0c0c7	Judo	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/judo_sport_image.png
3034b596-e970-44cb-9f25-8fefeb2bc411	Karate	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/karate_sport_image.png
a66e4e55-ece4-4327-983c-44eabb3ce4ed	MMA	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/mma_sport_image.png
5c1caf30-c8dd-4dd1-a272-4d74b1b11844	Muay Thai	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/muay_thai_sport_image.png
c4ee62d2-73f8-4e28-ba18-f95f6aecc855	Table Tennis	2026-06-11 10:21:07.973176	https://storage.googleapis.com/tournament-zvytiaga-images/themes/table_tennis_sport_image.png
ef77529f-e68b-4066-9244-32d451009e80	Boxing	2026-06-11 10:21:08.704994	https://storage.googleapis.com/tournament-zvytiaga-images/themes/boxing_sport_image.png
5849dfb2-adcd-45cc-83e2-a31d721c7884	Chess	2026-06-11 10:21:08.705659	https://storage.googleapis.com/tournament-zvytiaga-images/themes/chess_sport_image.png
8bab8e39-2e77-44af-8232-4fe31b7b1e45	Rocket League	2026-06-11 10:21:08.708498	https://storage.googleapis.com/tournament-zvytiaga-images/themes/rocket_league_sport_image.png
61a3b9ac-2886-42c6-a6ed-818cadad8c13	Shooting	2026-06-11 10:21:08.709206	https://storage.googleapis.com/tournament-zvytiaga-images/themes/shooting_sport_image.png
501584c4-c435-4c3d-9446-979bdf532b5f	Tennis	2026-06-11 10:21:08.710222	https://storage.googleapis.com/tournament-zvytiaga-images/themes/tennis_sport_image.png
\.


--
-- Data for Name: user; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."user" (id, full_name, password_hash, is_organizer, account_state_id, created_at, updated_at, deleted_at) FROM stdin;
00000000-0000-0000-0000-000000000001	John Smith	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	t	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.71094	2026-06-11 10:21:08.71094	\N
00000000-0000-0000-0000-000000000002	Jane Williams	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	t	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.713428	2026-06-11 10:21:08.713428	\N
00000000-0000-0000-0000-000000000003	Alex Johnson	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000004	Michael Brown	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000005	Sarah Davis	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000006	David Miller	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000007	Emily Wilson	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000008	Chris Anderson	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000009	Lisa Thomas	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000010	Kevin Martinez	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000011	Jessica Garcia	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
00000000-0000-0000-0000-000000000012	Ryan Lee	$2a$10$dXJ3SW6G7P50eS3WQYshlOAG4VPT8X3xDVNKBN3ILWtY3lV0kF8wS	f	98bcc273-7c58-4a7f-9b0c-c261831f55c4	2026-06-11 10:21:08.714637	2026-06-11 10:21:08.714637	\N
\.


--
-- Data for Name: user_details; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_details (id, user_id, email, date_of_birth, created_at, updated_at, preferences_setup_completed) FROM stdin;
ca1c1404-a889-4f08-9927-30f911dcffde	00000000-0000-0000-0000-000000000001	john.smith@example.com	1985-03-15	2026-06-11 10:21:08.723316	2026-06-11 10:21:08.723316	f
0ecbded8-9e1a-4c3f-9bf9-6c8fa1588a66	00000000-0000-0000-0000-000000000002	jane.williams@example.com	1990-07-22	2026-06-11 10:21:08.725389	2026-06-11 10:21:08.725389	f
f6ac2df0-394b-4004-b7fe-2c0c959c0e2c	00000000-0000-0000-0000-000000000003	alex.johnson@example.com	1995-01-10	2026-06-11 10:21:08.726509	2026-06-11 10:21:08.726509	f
01e330a6-001e-4ce3-9a6b-d218266390d1	00000000-0000-0000-0000-000000000004	michael.brown@example.com	1992-05-18	2026-06-11 10:21:08.727679	2026-06-11 10:21:08.727679	f
9937c2ff-00ba-4d31-9047-c9b0b52ab300	00000000-0000-0000-0000-000000000005	sarah.davis@example.com	1998-11-30	2026-06-11 10:21:08.728699	2026-06-11 10:21:08.728699	f
d2bdc039-1fe2-4ebb-bee9-2056b3e3c1b0	00000000-0000-0000-0000-000000000006	david.miller@example.com	1988-09-25	2026-06-11 10:21:08.729708	2026-06-11 10:21:08.729708	f
afbdb79d-44c4-4121-b395-135e7848c071	00000000-0000-0000-0000-000000000007	emily.wilson@example.com	1996-02-14	2026-06-11 10:21:08.731026	2026-06-11 10:21:08.731026	f
ade763e2-eec4-422a-ad78-fb2878565892	00000000-0000-0000-0000-000000000008	chris.anderson@example.com	1991-04-08	2026-06-11 10:21:08.732271	2026-06-11 10:21:08.732271	f
e2ae4f31-0afd-4562-a88c-5a136f7d1232	00000000-0000-0000-0000-000000000009	lisa.thomas@example.com	1994-08-19	2026-06-11 10:21:08.733206	2026-06-11 10:21:08.733206	f
18bf0fcc-35eb-4a62-9331-d73b476cd9e4	00000000-0000-0000-0000-000000000010	kevin.martinez@example.com	1993-12-03	2026-06-11 10:21:08.734092	2026-06-11 10:21:08.734092	f
b8cc597b-adc9-4e08-bf91-5b5b87fb2bf8	00000000-0000-0000-0000-000000000011	jessica.garcia@example.com	1997-06-27	2026-06-11 10:21:08.734987	2026-06-11 10:21:08.734987	f
63bce8c4-ad81-4c97-a7ed-d554636f47fa	00000000-0000-0000-0000-000000000012	ryan.lee@example.com	1989-10-11	2026-06-11 10:21:08.735809	2026-06-11 10:21:08.735809	f
\.


--
-- Data for Name: user_phone; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_phone (id, user_id, phone_number, created_at, deleted_at) FROM stdin;
d1fb8ef8-9a3f-4f3c-b96c-475a5c231fea	00000000-0000-0000-0000-000000000001	+1-555-0101	2026-06-11 10:21:08.736753	\N
6c81dc0d-b9c5-461a-84dd-9dd9a8dd0149	00000000-0000-0000-0000-000000000001	+1-555-0102	2026-06-11 10:21:08.739238	\N
96fb5e5f-be57-410c-a6de-e2d01f592824	00000000-0000-0000-0000-000000000002	+1-555-0201	2026-06-11 10:21:08.740831	\N
7742e8c0-dcb9-48a9-8c3d-3f27c8330721	00000000-0000-0000-0000-000000000003	+1-555-0301	2026-06-11 10:21:08.742344	\N
5c2ce4e9-e44a-48c0-b738-788370a3c134	00000000-0000-0000-0000-000000000004	+1-555-0401	2026-06-11 10:21:08.743708	\N
2ca2e4ca-cd58-468e-9b13-e091ceb475c6	00000000-0000-0000-0000-000000000005	+1-555-0501	2026-06-11 10:21:08.744994	\N
0a3286e0-09ff-49a9-b308-99db4209b1e5	00000000-0000-0000-0000-000000000006	+1-555-0601	2026-06-11 10:21:08.74615	\N
57a3d9f2-5595-40ab-b6fe-aab8caba54cf	00000000-0000-0000-0000-000000000007	+1-555-0701	2026-06-11 10:21:08.747148	\N
11d530ec-6aec-46da-a0af-eee350fb064b	00000000-0000-0000-0000-000000000008	+1-555-0801	2026-06-11 10:21:08.748063	\N
bd752f18-d2fa-4433-a9c3-c7ec8b2fa030	00000000-0000-0000-0000-000000000009	+1-555-0901	2026-06-11 10:21:08.748923	\N
e683ccb0-7c7d-4215-9f43-25890cbbfc3b	00000000-0000-0000-0000-000000000010	+1-555-1001	2026-06-11 10:21:08.749875	\N
f7ab10ab-bd2b-4622-b41f-2da7ed6dd60f	00000000-0000-0000-0000-000000000011	+1-555-1101	2026-06-11 10:21:08.751246	\N
db808e10-0f35-4505-866f-c586fc7121fb	00000000-0000-0000-0000-000000000012	+1-555-1201	2026-06-11 10:21:08.752943	\N
\.


--
-- Data for Name: user_team; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_team (id, user_id, team_id, created_at) FROM stdin;
0932604c-9909-4623-81e4-4cd35402a568	00000000-0000-0000-0000-000000000003	11111111-0000-0000-0000-000000000001	2026-06-11 10:21:08.769259
dfff66cb-ed1c-4d7a-9fe3-52260c503528	00000000-0000-0000-0000-000000000004	11111111-0000-0000-0000-000000000002	2026-06-11 10:21:08.771939
e677303d-efac-46d9-8f03-99c6960a2d0a	00000000-0000-0000-0000-000000000005	11111111-0000-0000-0000-000000000003	2026-06-11 10:21:08.773083
f01da6e6-06ad-44c6-9b92-3b0a936efd3c	00000000-0000-0000-0000-000000000006	11111111-0000-0000-0000-000000000004	2026-06-11 10:21:08.774139
23519408-2594-4f7e-872f-065b530d4b4a	00000000-0000-0000-0000-000000000007	11111111-0000-0000-0000-000000000005	2026-06-11 10:21:08.775702
bb7ec193-8d19-4be8-8348-79e1c2ea954a	00000000-0000-0000-0000-000000000008	11111111-0000-0000-0000-000000000006	2026-06-11 10:21:08.777161
7696bcbb-c0a4-4e40-8800-89b2614da917	00000000-0000-0000-0000-000000000009	11111111-0000-0000-0000-000000000007	2026-06-11 10:21:08.778416
cafe25a1-bfe3-4c42-b4fd-399226a72651	00000000-0000-0000-0000-000000000010	11111111-0000-0000-0000-000000000007	2026-06-11 10:21:08.779502
abb44b4d-322f-4a24-8617-b38c29039100	00000000-0000-0000-0000-000000000011	11111111-0000-0000-0000-000000000008	2026-06-11 10:21:08.780825
72cccb64-846d-4d83-8c03-c87be3dccb00	00000000-0000-0000-0000-000000000012	11111111-0000-0000-0000-000000000008	2026-06-11 10:21:08.78214
\.


--
-- Data for Name: user_tournament_theme_preference; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.user_tournament_theme_preference (id, user_id, theme_id, created_at) FROM stdin;
30942663-9f79-443c-a20d-3a112f5e2c01	00000000-0000-0000-0000-000000000001	5849dfb2-adcd-45cc-83e2-a31d721c7884	2026-06-11 10:21:08.754517
0c9d3c5e-dd34-467d-8ab3-3a9a2d80dc9e	00000000-0000-0000-0000-000000000002	501584c4-c435-4c3d-9446-979bdf532b5f	2026-06-11 10:21:08.757496
\.


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: account_state account_state_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.account_state
    ADD CONSTRAINT account_state_pkey PRIMARY KEY (id);


--
-- Name: match match_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.match
    ADD CONSTRAINT match_pkey PRIMARY KEY (id);


--
-- Name: refresh_token refresh_token_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT refresh_token_pkey PRIMARY KEY (id);


--
-- Name: team team_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.team
    ADD CONSTRAINT team_pkey PRIMARY KEY (id);


--
-- Name: tournament tournament_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tournament
    ADD CONSTRAINT tournament_pkey PRIMARY KEY (id);


--
-- Name: tournament_theme tournament_theme_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tournament_theme
    ADD CONSTRAINT tournament_theme_pkey PRIMARY KEY (id);


--
-- Name: user_details user_details_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_details
    ADD CONSTRAINT user_details_pkey PRIMARY KEY (id);


--
-- Name: user_phone user_phone_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_phone
    ADD CONSTRAINT user_phone_pkey PRIMARY KEY (id);


--
-- Name: user user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_pkey PRIMARY KEY (id);


--
-- Name: user_team user_team_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_team
    ADD CONSTRAINT user_team_pkey PRIMARY KEY (id);


--
-- Name: user_tournament_theme_preference user_tournament_theme_preference_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_tournament_theme_preference
    ADD CONSTRAINT user_tournament_theme_preference_pkey PRIMARY KEY (id);


--
-- Name: IX_refresh_token_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_refresh_token_user_id" ON public.refresh_token USING btree (user_id);


--
-- Name: IX_user_tournament_theme_preference_theme_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_user_tournament_theme_preference_theme_id" ON public.user_tournament_theme_preference USING btree (theme_id);


--
-- Name: account_state_name_key; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX account_state_name_key ON public.account_state USING btree (name);


--
-- Name: idx_match_is_valid; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_is_valid ON public.match USING btree (is_valid);


--
-- Name: idx_match_level; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_level ON public.match USING btree (level);


--
-- Name: idx_match_team_a_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_team_a_id ON public.match USING btree (team_a_id);


--
-- Name: idx_match_team_b_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_team_b_id ON public.match USING btree (team_b_id);


--
-- Name: idx_match_tournament_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_tournament_id ON public.match USING btree (tournament_id);


--
-- Name: idx_match_winner_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_match_winner_id ON public.match USING btree (winner_id);


--
-- Name: idx_refresh_token_token; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_refresh_token_token ON public.refresh_token USING btree (token);


--
-- Name: idx_team_is_disqualified; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_team_is_disqualified ON public.team USING btree (is_disqualified);


--
-- Name: idx_team_tournament_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_team_tournament_id ON public.team USING btree (tournament_id);


--
-- Name: idx_tournament_organizer_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tournament_organizer_id ON public.tournament USING btree (organizer_id);


--
-- Name: idx_tournament_registration_deadline; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tournament_registration_deadline ON public.tournament USING btree (registration_deadline);


--
-- Name: idx_tournament_start_date; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tournament_start_date ON public.tournament USING btree (start_date);


--
-- Name: idx_tournament_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tournament_status ON public.tournament USING btree (status);


--
-- Name: idx_tournament_theme_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tournament_theme_id ON public.tournament USING btree (theme_id);


--
-- Name: idx_user_account_state_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_account_state_id ON public."user" USING btree (account_state_id);


--
-- Name: idx_user_deleted_at; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_deleted_at ON public."user" USING btree (deleted_at);


--
-- Name: idx_user_details_email; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_details_email ON public.user_details USING btree (email);


--
-- Name: idx_user_details_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_details_user_id ON public.user_details USING btree (user_id);


--
-- Name: idx_user_is_organizer; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_is_organizer ON public."user" USING btree (is_organizer);


--
-- Name: idx_user_phone_phone_number; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_phone_phone_number ON public.user_phone USING btree (phone_number);


--
-- Name: idx_user_phone_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_phone_user_id ON public.user_phone USING btree (user_id);


--
-- Name: idx_user_team_team_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_team_team_id ON public.user_team USING btree (team_id);


--
-- Name: idx_user_team_user_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_user_team_user_id ON public.user_team USING btree (user_id);


--
-- Name: tournament_theme_name_key; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX tournament_theme_name_key ON public.tournament_theme USING btree (name);


--
-- Name: unique_match_position; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX unique_match_position ON public.match USING btree (tournament_id, level, order_number);


--
-- Name: unique_team_name_per_tournament; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX unique_team_name_per_tournament ON public.team USING btree (name, tournament_id);


--
-- Name: unique_user_team; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX unique_user_team ON public.user_team USING btree (user_id, team_id);


--
-- Name: unique_user_theme_preference; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX unique_user_theme_preference ON public.user_tournament_theme_preference USING btree (user_id, theme_id);


--
-- Name: user_details_email_key; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX user_details_email_key ON public.user_details USING btree (email);


--
-- Name: user_details_user_id_key; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX user_details_user_id_key ON public.user_details USING btree (user_id);


--
-- Name: match match_team_a_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.match
    ADD CONSTRAINT match_team_a_id_fkey FOREIGN KEY (team_a_id) REFERENCES public.team(id) ON DELETE RESTRICT;


--
-- Name: match match_team_b_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.match
    ADD CONSTRAINT match_team_b_id_fkey FOREIGN KEY (team_b_id) REFERENCES public.team(id) ON DELETE SET NULL;


--
-- Name: match match_tournament_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.match
    ADD CONSTRAINT match_tournament_id_fkey FOREIGN KEY (tournament_id) REFERENCES public.tournament(id) ON DELETE CASCADE;


--
-- Name: match match_winner_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.match
    ADD CONSTRAINT match_winner_id_fkey FOREIGN KEY (winner_id) REFERENCES public.team(id) ON DELETE SET NULL;


--
-- Name: refresh_token refresh_token_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT refresh_token_user_id_fkey FOREIGN KEY (user_id) REFERENCES public."user"(id) ON DELETE CASCADE;


--
-- Name: team team_tournament_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.team
    ADD CONSTRAINT team_tournament_id_fkey FOREIGN KEY (tournament_id) REFERENCES public.tournament(id) ON DELETE CASCADE;


--
-- Name: tournament tournament_organizer_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tournament
    ADD CONSTRAINT tournament_organizer_id_fkey FOREIGN KEY (organizer_id) REFERENCES public."user"(id) ON DELETE SET NULL;


--
-- Name: tournament tournament_theme_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tournament
    ADD CONSTRAINT tournament_theme_id_fkey FOREIGN KEY (theme_id) REFERENCES public.tournament_theme(id);


--
-- Name: user user_account_state_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."user"
    ADD CONSTRAINT user_account_state_id_fkey FOREIGN KEY (account_state_id) REFERENCES public.account_state(id);


--
-- Name: user_details user_details_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_details
    ADD CONSTRAINT user_details_user_id_fkey FOREIGN KEY (user_id) REFERENCES public."user"(id) ON DELETE CASCADE;


--
-- Name: user_phone user_phone_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_phone
    ADD CONSTRAINT user_phone_user_id_fkey FOREIGN KEY (user_id) REFERENCES public."user"(id) ON DELETE CASCADE;


--
-- Name: user_team user_team_team_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_team
    ADD CONSTRAINT user_team_team_id_fkey FOREIGN KEY (team_id) REFERENCES public.team(id) ON DELETE RESTRICT;


--
-- Name: user_team user_team_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_team
    ADD CONSTRAINT user_team_user_id_fkey FOREIGN KEY (user_id) REFERENCES public."user"(id) ON DELETE RESTRICT;


--
-- Name: user_tournament_theme_preference user_tournament_theme_preference_theme_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_tournament_theme_preference
    ADD CONSTRAINT user_tournament_theme_preference_theme_id_fkey FOREIGN KEY (theme_id) REFERENCES public.tournament_theme(id) ON DELETE CASCADE;


--
-- Name: user_tournament_theme_preference user_tournament_theme_preference_user_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.user_tournament_theme_preference
    ADD CONSTRAINT user_tournament_theme_preference_user_id_fkey FOREIGN KEY (user_id) REFERENCES public."user"(id) ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict e6k31RJzmvB0f0mnJ8nwjZJePCWkva9UOp8w7IRdI4Zi0ciW28J1DTjqocJuQRZ


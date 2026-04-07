
## Data Dictionary - Tournament Platform System

#### Посилання на ER-діаграму https://github.com/katushhiaa/Tournament-Platform-System/wiki/ER%E2%80%90Diagram-and-Domain-Model-&-Schema-Specification

#### Посилання на DDL https://github.com/katushhiaa/Tournament-Platform-System/blob/main/db/init_db.sql

**Дата:** 2026-04-07 

---

## ACCOUNT_STATE

**Опис:** Довідник можливих станів акаунту користувача.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор стану |
| `name` | VARCHAR(50) | UNIQUE | Назва стану (active, inactive, suspended, banned) |
| `description` | TEXT | NULLABLE | Опис стану |
| `is_active` | BOOLEAN | NOT NULL | Чи стан активний |

**Зв'язки:**
- 1:N → USER (одне стану → багато користувачів)

---

## USER

**Опис:** Основна таблиця користувачів. Містить облікові дані та ролі (організатор/гравець).

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK, AUTO_INCREMENT | Унікальний ідентифікатор |
| `full_name` | VARCHAR(255) | NOT NULL | ПІБ користувача |
| `password_hash` | VARCHAR(255) | NOT NULL | Bcrypt хеш пароля |
| `is_organizer` | BOOLEAN | NOT NULL, DEFAULT FALSE | true = організатор, false = гравець |
| `account_state_id` | INT | NOT NULL, FK | Посилання на стан акаунту |
| `created_at` | TIMESTAMP | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `updated_at` | TIMESTAMP | NOT NULL, AUTO_UPDATE | Дата останньої зміни |
| `deleted_at` | TIMESTAMP | NULLABLE | Дата soft delete (GDPR) |

**Зв'язки:**
- N:1 ← ACCOUNT_STATE (account_state_id)
- 1:N → TOURNAMENT (organizer_id, ON DELETE SET NULL)
- 1:N → USER_TEAM (CASCADE soft-delete)
- 1:1 ↔ USER_DETAILS (CASCADE hard-delete)
- 1:N → USER_PHONE (CASCADE hard-delete)

**Бізнес-логіка:**
- `deleted_at IS NULL` → користувач активний
- `is_organizer = true` → може створювати турніри

---

## USER_DETAILS

**Опис:** Чутливі персональні дані користувача (email, дата народження). Зберігаються окремо для GDPR.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор |
| `user_id` | INT | FK UNIQUE | Посилання на користувача (one-to-one) |
| `email` | VARCHAR(255) | NOT NULL, UNIQUE | Email для входу |
| `date_of_birth` | DATE | NOT NULL | Дата народження |
| `created_at` | TIMESTAMP | NOT NULL | Дата створення |
| `updated_at` | TIMESTAMP | NOT NULL | Дата останньої зміни |

**Зв'язки:**
- 1:1 ← USER (user_id, CASCADE DELETE)

**Бізнес-логіка:**
- Email UNIQUE для унікальності облікових даних
- CASCADE DELETE з USER для GDPR compliance

---

## USER_PHONE

**Опис:** Телефонні номери користувача. Користувач може мати декілька номерів.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор |
| `user_id` | INT | FK | Посилання на користувача |
| `phone_number` | VARCHAR(20) | NOT NULL | Номер телефону (+38...) |
| `created_at` | TIMESTAMP | NOT NULL | Дата додавання |
| `deleted_at` | TIMESTAMP | NULLABLE | Дата soft delete |

**Зв'язки:**
- N:1 ← USER (user_id, CASCADE DELETE)

**Бізнес-логіка:**
- One-to-many (користувач може мати декілька номерів)
- `deleted_at IS NULL` → номер активний

---

## TOURNAMENT_THEME

**Опис:** Довідник видів спорту/тем турнірів.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор |
| `name` | VARCHAR(100) | UNIQUE | Назва теми (Football, Basketball, Volleyball, тощо) |

**Зв'язки:**
- 1:N → TOURNAMENT (theme_id)

---

## TOURNAMENT

**Опис:** Основ��а таблиця турнірів. Містить інформацію про турнір, дати та обмеження.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор турніру |
| `name` | VARCHAR(255) | NOT NULL | Назва турніру |
| `organizer_id` | INT | FK NULLABLE | Організатор (ON DELETE SET NULL) |
| `theme_id` | INT | FK | Вид спорту/тема |
| `max_teams` | INT | NOT NULL, CHECK >= 2 | Макс. кількість учасників |
| `start_date` | TIMESTAMP | NOT NULL | Дата/час початку |
| `registration_deadline` | TIMESTAMP | NOT NULL, CHECK <= start_date | Дата закриття реєстрації |
| `end_date` | TIMESTAMP | NOT NULL, CHECK >= start_date | Дата завершення |
| `description` | TEXT | NULLABLE | Опис турніру |
| `conditions` | TEXT | NULLABLE | Умови участі |
| `status` | ENUM | NOT NULL, DEFAULT 'REGISTRATION_OPEN' | Статус турніру |
| `created_at` | TIMESTAMP | NOT NULL | Дата створення |
| `updated_at` | TIMESTAMP | NOT NULL | Дата останньої зміни |

**Зв'язки:**
- N:1 ← USER (organizer_id)
- N:1 ← TOURNAMENT_THEME (theme_id)
- 1:N → TEAM (CASCADE DELETE)
- 1:N → MATCH (CASCADE DELETE)

**Статуси:**
- REGISTRATION_OPEN → гравці можуть реєструватися
- REGISTRATION_CLOSED → реєстрація закрита, сітка готова
- IN_PROGRESS → матчі проходять
- COMPLETED → турнір завершений

---

## TEAM

**Опис:** Команди в турнірах. У Single Elimination - один гравець = одна команда.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор команди |
| `name` | VARCHAR(255) | NOT NULL | Назва команди |
| `tournament_id` | INT | FK | Посилання на турнір |
| `is_disqualified` | BOOLEAN | NOT NULL, DEFAULT FALSE | Чи дискваліфікована |
| `created_at` | TIMESTAMP | NOT NULL | Дата створення |
| `updated_at` | TIMESTAMP | NOT NULL | Дата останньої зміни |
| | | UNIQUE (name, tournament_id) | Унікальна назва в турнірі |

**Зв'язки:**
- N:1 ← TOURNAMENT (tournament_id, CASCADE DELETE)
- 1:N → USER_TEAM (CASCADE DELETE)
- 1:N → MATCH (team_a_id, team_b_id, winner_id)

**Бізнес-логіка:**
- Команди НЕ видаляються, тільки marked as disqualified
- UNIQUE (name, tournament_id) → одна назва на турнір

---

## USER_TEAM

**Опис:** Junction table - участь користувача в командах. Immutable membership.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор |
| `user_id` | INT | FK | Посилання на користувача |
| `team_id` | INT | FK | Посилання на команду |
| `created_at` | TIMESTAMP | NOT NULL | Дата приєднання |
| | | UNIQUE (user_id, team_id) | Користувач не може бути у команді двічі |

**Зв'язки:**
- N:1 ← USER (user_id, CASCADE DELETE)
- N:1 ← TEAM (team_id, CASCADE DELETE)

**Бізнес-логіка:**
- Many-to-many (користувач ↔ команди)
- Immutable - записи НЕ можуть редагуватися
- CASCADE DELETE з TEAM при видаленні команди

---

## MATCH

**Опис:** Матчи турніру. Генеруються автоматично в Single Elimination форматі.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | INT | PK | Ідентифікатор матчу |
| `tournament_id` | INT | FK | Посилання на турнір |
| `team_a_id` | INT | FK | Перша команда |
| `team_b_id` | INT | FK NULLABLE | Друга команда (NULL для bye) |
| `winner_id` | INT | FK NULLABLE, CHECK IN (team_a_id, team_b_id) | Переможець |
| `level` | INT | NOT NULL, CHECK >= -1 | Рівень сітки (0=фінал, -1=3rd place, 1,2,3...) |
| `order_number` | INT | NOT NULL | Позиція на рівні |
| `start_date` | TIMESTAMP | NULLABLE | Дата/час матчу |
| `team_a_score` | INT | NOT NULL, DEFAULT 0 | Бали команди A |
| `team_b_score` | INT | NOT NULL, DEFAULT 0 | Бали команди B |
| `is_valid` | BOOLEAN | NOT NULL, DEFAULT TRUE | Чи матч рахується |
| `created_at` | TIMESTAMP | NOT NULL | Дата створення |
| `updated_at` | TIMESTAMP | NOT NULL | Дата останньої зміни |
| | | UNIQUE (tournament_id, level, order_number) | Унікальна позиція матчу |

**Зв'язки:**
- N:1 ← TOURNAMENT (tournament_id, CASCADE DELETE)
- N:1 ← TEAM (team_a_id)
- N:1 ← TEAM (team_b_id, NULLABLE для bye)
- N:1 ← TEAM (winner_id, NULLABLE)

**Бізнес-логіка:**
- `team_b_id = NULL` → bye матч (Team A автоматично проходить)
- `winner_id = NULL` → результат не внесено
- `is_valid = false` → матч не рахується (дискваліфікація)
- Матчи НЕ видаляються, тільки marked as invalid

---

## Сумарна таблиця зв'язків

| Таблиця 1 | Тип | Таблиця 2 | Каскадне видалення |
|-----------|-----|-----------|-------------------|
| ACCOUNT_STATE | 1:N | USER | - |
| USER | 1:N | TOURNAMENT | SET NULL |
| USER | 1:N | USER_TEAM | Soft delete |
| USER | 1:1 | USER_DETAILS | CASCADE |
| USER | 1:N | USER_PHONE | CASCADE |
| TOURNAMENT_THEME | 1:N | TOURNAMENT | - |
| TOURNAMENT | 1:N | TEAM | CASCADE |
| TOURNAMENT | 1:N | MATCH | CASCADE |
| TEAM | 1:N | USER_TEAM | CASCADE |
| TEAM | 1:N | MATCH (A/B/Winner) | - |
| USER_TEAM | N:1 | USER | CASCADE |
| USER_TEAM | N:1 | TEAM | CASCADE |


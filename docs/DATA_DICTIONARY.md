## Data Dictionary - Tournament Platform System

#### Посилання на ER-діаграму https://github.com/katushhiaa/Tournament-Platform-System/wiki/ER%E2%80%90Diagram-and-Domain-Model-&-Schema-Specification
#### Посилання на DDL https://github.com/katushhiaa/Tournament-Platform-System/blob/main/db/init_db.sql

**Дата оновлення:** 2026-06-11 (На основі файлу бекапу)

---

## ACCOUNT_STATE

**Опис:** Довідник можливих станів акаунту користувача.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор стану |
| `name` | VARCHAR(50) | NOT NULL, UNIQUE | Назва стану (active, inactive, suspended, banned) |
| `description` | VARCHAR(255) | NULLABLE | Опис стану |
| `is_active` | BOOLEAN | DEFAULT true | Чи стан активний |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата створення |

**Зв'язки:**
- 1:N → USER (одне стану → багато користувачів)

---

## USER

**Опис:** Основна таблиця користувачів. Містить облікові дані та ролі (організатор/гравець).

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Унікальний ідентифікатор |
| `full_name` | VARCHAR(255) | NOT NULL | ПІБ користувача |
| `password_hash` | VARCHAR(255) | NOT NULL | Bcrypt хеш пароля |
| `is_organizer` | BOOLEAN | DEFAULT false | true = організатор, false = гравець |
| `account_state_id` | UUID | NOT NULL, FK | Посилання на стан акаунту |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `updated_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата останньої зміни |
| `deleted_at` | TIMESTAMP | NULLABLE | Дата soft delete (GDPR) |

**Зв'язки:**
- N:1 ← ACCOUNT_STATE (account_state_id)
- 1:N → TOURNAMENT (organizer_id)
- 1:N → USER_TEAM (user_id)
- 1:1 ↔ USER_DETAILS (user_id)
- 1:N → USER_PHONE (user_id)
- 1:N → REFRESH_TOKEN (user_id)
- 1:N → USER_TOURNAMENT_THEME_PREFERENCE (user_id)

---

## USER_DETAILS

**Опис:** Чутливі персональні дані користувача (email, дата народження, статус налаштування преференцій).

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор |
| `user_id` | UUID | NOT NULL, FK, UNIQUE | Посилання на користувача |
| `email` | VARCHAR(255) | NOT NULL, UNIQUE | Email для входу |
| `date_of_birth` | DATE | NOT NULL | Дата народження |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `updated_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата останньої зміни |
| `preferences_setup_completed`| BOOLEAN | NOT NULL, DEFAULT false | Чи завершено налаштування преференцій |

**Зв'язки:**
- 1:1 ← USER (user_id, CASCADE DELETE)

---

## USER_PHONE

**Опис:** Телефонні номери користувача. Користувач може мати декілька номерів.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор |
| `user_id` | UUID | NOT NULL, FK | Посилання на користувача |
| `phone_number` | VARCHAR(20) | NOT NULL | Номер телефону |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата додавання |
| `deleted_at` | TIMESTAMP | NULLABLE | Дата soft delete |

**Зв'язки:**
- N:1 ← USER (user_id, CASCADE DELETE)

---

## TOURNAMENT_THEME

**Опис:** Довідник видів спорту/тем турнірів.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор |
| `name` | VARCHAR(100) | NOT NULL, UNIQUE | Назва теми (Chess, Tennis, тощо) |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `image_url` | TEXT | NULLABLE | Посилання на зображення теми |

**Зв'язки:**
- 1:N → TOURNAMENT (theme_id)
- 1:N → USER_TOURNAMENT_THEME_PREFERENCE (theme_id)

---

## TOURNAMENT

**Опис:** Основна таблиця турнірів. Містить інформацію про турнір, дати та обмеження.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор турніру |
| `name` | VARCHAR(255) | NOT NULL | Назва турніру |
| `organizer_id` | UUID | NULLABLE, FK | Організатор (ON DELETE SET NULL) |
| `theme_id` | UUID | NOT NULL, FK | Вид спорту/тема |
| `max_teams` | INT | NOT NULL | Макс. кількість учасників |
| `background_img` | TEXT | NULLABLE | Посилання на фонове зображення |
| `start_date` | TIMESTAMP | NOT NULL | Дата/час початку |
| `registration_deadline` | TIMESTAMP | NOT NULL | Дата закриття реєстрації |
| `end_date` | TIMESTAMP | NOT NULL | Дата завершення |
| `description` | TEXT | NULLABLE | Опис турніру |
| `conditions` | TEXT | NULLABLE | Умови участі |
| `status` | INT | NOT NULL | Статус турніру (Enum integer) |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `updated_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата останньої зміни |

**Зв'язки:**
- N:1 ← USER (organizer_id)
- N:1 ← TOURNAMENT_THEME (theme_id)
- 1:N → TEAM (CASCADE DELETE)
- 1:N → MATCH (CASCADE DELETE)

---

## TEAM

**Опис:** Команди в турнірах. У Single Elimination - один гравець = одна команда.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор команди |
| `name` | VARCHAR(255) | NOT NULL | Назва команди |
| `tournament_id` | UUID | NOT NULL, FK | Посилання на турнір |
| `is_disqualified` | BOOLEAN | DEFAULT false | Чи дискваліфікована |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `updated_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата останньої зміни |

*Обмеження:* Унікальна пара `name` + `tournament_id`.

**Зв'язки:**
- N:1 ← TOURNAMENT (tournament_id, CASCADE DELETE)
- 1:N → USER_TEAM (team_id)
- 1:N → MATCH (як team_a_id, team_b_id, winner_id)

---

## USER_TEAM

**Опис:** Junction table - участь користувача в командах.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор |
| `user_id` | UUID | NOT NULL, FK | Посилання на користувача |
| `team_id` | UUID | NOT NULL, FK | Посилання на команду |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата приєднання |

*Обмеження:* Унікальна пара `user_id` + `team_id`.

**Зв'язки:**
- N:1 ← USER (user_id)
- N:1 ← TEAM (team_id)

---

## USER_TOURNAMENT_THEME_PREFERENCE

**Опис:** Улюблені теми (види спорту) користувача.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор |
| `user_id` | UUID | NOT NULL, FK | Посилання на користувача |
| `theme_id` | UUID | NOT NULL, FK | Посилання на тему |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата додавання |

*Обмеження:* Унікальна пара `user_id` + `theme_id`.

**Зв'язки:**
- N:1 ← USER (user_id, CASCADE DELETE)
- N:1 ← TOURNAMENT_THEME (theme_id, CASCADE DELETE)

---

## MATCH

**Опис:** Матчи турніру. Генеруються автоматично в турнірній сітці.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор матчу |
| `tournament_id` | UUID | NOT NULL, FK | Посилання на турнір |
| `team_a_id` | UUID | NULLABLE, FK | Перша команда |
| `team_b_id` | UUID | NULLABLE, FK | Друга команда |
| `winner_id` | UUID | NULLABLE, FK | Переможець |
| `level` | INT | NOT NULL | Рівень сітки |
| `order_number` | INT | NOT NULL | Позиція на рівні |
| `start_date` | TIMESTAMP | NULLABLE | Дата/час матчу |
| `team_a_score` | INT | DEFAULT 0 | Бали команди A |
| `team_b_score` | INT | DEFAULT 0 | Бали команди B |
| `is_valid` | BOOLEAN | DEFAULT true | Чи матч рахується |
| `created_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `updated_at` | TIMESTAMP | DEFAULT CURRENT_TIMESTAMP | Дата останньої зміни |
| `IsBye` | BOOLEAN | NULLABLE | Чи є це матч-пропуск (Bye) |
| `Status` | TEXT | NULLABLE | Статус матчу |

*Обмеження:* Унікальна комбінація `tournament_id` + `level` + `order_number`.

**Зв'язки:**
- N:1 ← TOURNAMENT (tournament_id, CASCADE DELETE)
- N:1 ← TEAM (team_a_id)
- N:1 ← TEAM (team_b_id)
- N:1 ← TEAM (winner_id)

---

## REFRESH_TOKEN

**Опис:** Збережені токени оновлення для сесій користувачів.

| Поле | Тип | Обмеження | Опис |
|------|------|-----------|------|
| `id` | UUID | PK, DEFAULT gen_random_uuid() | Ідентифікатор токену |
| `user_id` | UUID | NOT NULL, FK | Користувач-власник токену |
| `token` | VARCHAR(255) | NOT NULL | Значення токену |
| `jwt_id` | VARCHAR(255) | NOT NULL | ID пов'язаного JWT |
| `created_at` | TIMESTAMP | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Дата створення |
| `expires_at` | TIMESTAMP | NOT NULL | Дата закінчення терміну дії |
| `is_used` | BOOLEAN | NOT NULL, DEFAULT false | Чи був використаний |
| `is_revoked` | BOOLEAN | NOT NULL, DEFAULT false | Чи був відкликаний |

**Зв'язки:**
- N:1 ← USER (user_id, CASCADE DELETE)

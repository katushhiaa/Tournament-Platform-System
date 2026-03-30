# Tournament-Platform-System

Система управління турнірами, яка дозволяє користувачам створювати турніри, керувати учасниками та відстежувати результати матчів

## Корисні посилання
* **[Project Hub (Wiki)](https://github.com/katushhiaa/Tournament-Platform-System/wiki)** — повна документація проєкту.
* **[Jira Board](https://tournamentsystem.atlassian.net/jira/software/projects/DEV/boards/1/backlog)** — таск-трекер та керування спринтами.
* **[Figma Design]()** — прототипи інтерфейсу користувача.

## Структура репозиторію
Згідно з обраною архітектурою, проєкт має наступну структуру папок:

* `📂 /docs` — Документація, ТЗ, діаграми, API-специфікації
* `📂 /client` — Frontend-частина застосунку.
* `📂 /server` — Backend-частина застосунку.
* `📂 /shared` — Спільні ресурси (типи даних, константи, переклади).
* `📂 /deploy` — Конфігурації для Docker, CI/CD, скрипти розгортання.

## Технологічний стек
* **Backend:** .NET / ASP.NET Core
* **Frontend:** Vue.js (або React)
* **Database:** PostgreSQL

## Інструкція із запуску
*На поточному етапі проєкт містить архітектурний скелет.*

1. **Клонування репозиторію:**
   ```bash
   git clone https://github.com/katushhiaa/Tournament-Platform-System
   ```
2. **Перехід у папку проєкту:**
   ```bash
   cd tournament-platform
   ```

## 👥 Команда проєкту
* **Анур'єва Катерина** — Project Manager
* **Скуртул Сергій** — Backend Developer
* **Дудко Володимир** — Database Engineer
* **Ярмолюк Людмила** — Frontend Developer
* **Загрбенюк Богдан** — QA Engineer

---

## Naming Conventions

### Backend (.NET)

#### Класи
- PascalCase
- Приклади: `User`, `Tournament`, `Match`, `TournamentService`

#### Методи
- PascalCase
- Приклади: `GetTournamentById()`, `CreateTournament()`, `JoinTournament()`

#### Параметри
- camelCase
- Приклади: `tournamentId`, `userId`, `matchResult`

#### Private-поля
- _camelCase
- Приклади: `_userRepository`, `_tournamentService`, `_logger`

#### Інтерфейси
- PascalCase з префіксом `I`
- Приклади: `IUserService`, `ITournamentRepository`

---

### Frontend (Vue 3 + TypeScript)

#### Папки
- kebab-case
- Приклади: `user-profile`, `match-bracket`, `tournaments`

#### Компоненти
- PascalCase
- Приклади: `TournamentCard.vue`, `MatchBracket.vue`, `UserProfile.vue`

#### Утиліти
- camelCase
- Приклади: `formatDate.ts`, `calculateBracket.ts`, `validateEmail.ts`

#### Hooks
- camelCase з префіксом `use`
- Приклади: `useAuth.ts`, `useTournament.ts`

---

### Database (PostgreSQL)

#### Таблиці
- snake_case, множина
- Приклади: `users`, `tournaments`, `matches`

#### Колонки
- snake_case, однина
- Приклади: `id`, `user_name`, `created_at`, `tournament_name`

#### Зовнішні ключі
- формат: `entity_id`
- Приклади: `user_id`, `tournament_id`, `match_id`

---

### Заборонені назви
Не дозволяється використовувати:
- `data`
- `info`
- `temp`
- `test`
- `item`
- `object`
- `helper`
- `utils`

---

### Formatting Rules

#### Backend
- відступ: 4 пробіли
- один клас на файл
- using-директиви впорядковані

#### Frontend
- відступ: 2 пробіли
- крапка з комою обов’язкова
- лапки: одинарні
- максимальна довжина рядка: 100 символів

#### Database
- SQL ключові слова у верхньому регістрі
- назви таблиць і колонок у snake_case

*Чернівці, 2026*

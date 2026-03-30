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
- Назва починається з дієслова  
- Приклади: `GetTournamentById()`, `CreateTournament()`, `JoinTournament()`

#### Параметри
- camelCase  
- Приклади: `tournamentId`, `userId`, `matchResult`

#### Private-поля
- _camelCase  
- Приклади: `_userRepository`, `_tournamentService`, `_logger`

#### Публічні властивості
- PascalCase  
- Приклади: `Id`, `UserName`, `Email`, `CreatedAt`

#### Інтерфейси
- PascalCase з префіксом `I`  
- Приклади: `IUserService`, `ITournamentRepository`

#### DTO / Request / Response
- PascalCase  
- Приклади: `RegisterUserRequest`, `LoginResponse`, `TournamentDetailsDto`

---

### Frontend (Vue 3 + TypeScript)

#### Папки
- kebab-case  
- Приклади: `user-profile`, `match-bracket`, `tournaments`

#### Компоненти
- PascalCase  
- Файл = назва компонента  
- Приклади: `TournamentCard.vue`, `MatchBracket.vue`, `UserProfile.vue`

Заборонено використовувати абстрактні назви:
- `Data.vue`, `Item.vue`, `Info.vue`

#### Сторінки
- PascalCase + суфікс Page  
- Приклади: `HomePage.vue`, `TournamentDetailsPage.vue`

#### Форми
- PascalCase + суфікс Form  
- Приклади: `LoginForm.vue`, `CreateTournamentForm.vue`

#### Сервіси (API)
- camelCase  
- Приклади: `authService.ts`, `tournamentService.ts`

#### Утиліти
- camelCase  
- Приклади: `formatDate.ts`, `validateEmail.ts`

#### Hooks
- camelCase з префіксом use  
- Приклади: `useAuth.ts`, `useTournament.ts`

#### Змінні
- camelCase  
- Приклади: `tournamentList`, `currentUser`

#### Boolean-змінні
- префікси: is, has, can  
- Приклади: `isLoading`, `hasError`, `canJoinTournament`

#### Функції
- camelCase  
- Приклади: `fetchTournaments()`, `createTournament()`

#### Константи
- UPPER_SNAKE_CASE  
- Приклади: `API_URL`, `MAX_PLAYERS`

#### Props
- camelCase у script  
- kebab-case у template  

#### Emits
- kebab-case  
- Приклади: `join-tournament`, `submit-result`

---

### Database (PostgreSQL)

#### Таблиці
- snake_case, множина  
- Приклади: `users`, `tournaments`, `matches`

#### Колонки
- snake_case, однина  
- Приклади: `id`, `user_name`, `created_at`

#### Первинний ключ
- `id`

#### Зовнішні ключі
- формат: entity_id  
- Приклади: `user_id`, `tournament_id`, `match_id`

#### Таблиці зв’язку
- snake_case  
- Приклади: `tournament_participants`, `user_roles`

#### Boolean-поля
- префікси: is_, has_, can_  
- Приклади: `is_active`, `has_paid_fee`

#### Дата та час
- суфікс _at  
- Приклади: `created_at`, `updated_at`

---

### Заборонені назви

Заборонено використовувати:

- data  
- info  
- temp  
- test  
- item  
- object  
- helper  
- utils  
- value  
- entity  

Bad:
- DataService  
- InfoComponent  
- TempTable  

Good:
- TournamentService  
- MatchResultCard  
- UserRepository  

---

### Formatting Rules

#### Backend (.NET)
- відступ: 4 пробіли  
- один клас на файл  
- using-директиви впорядковані  

#### Frontend (Vue + TypeScript)
- відступ: 2 пробіли  
- крапка з комою обов’язкова  
- лапки: одинарні (' ')  
- max довжина рядка: 100 символів  

#### Database
- SQL ключові слова у верхньому регістрі  
- назви у snake_case  


*Чернівці, 2026*

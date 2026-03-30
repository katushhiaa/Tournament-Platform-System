Code Style Guide
Базові стандарти
У проєкті використовуються такі базові галузеві стандарти:
Backend
Microsoft C# Coding Conventions
.NET Coding Conventions
Frontend
Vue 3 Style Guide (Official)
Airbnb JavaScript Style Guide (адаптовано під Vue + TypeScript)
Database
PostgreSQL naming best practices, адаптовані під структуру проєкту та узгоджені з Backend-моделями через ORM
Naming Conventions
2.1. Backend (.NET)
Класи
формат: PascalCase
назва класу повинна відображати конкретну сутність або відповідальність
Приклади:
User
Tournament
Match
TournamentService
AuthController
CreateTournamentRequest
Методи
формат: PascalCase
назва методу повинна починатися з дієслова та описувати дію
Приклади:
GetTournamentById()
CreateTournament()
JoinTournament()
UpdateMatchResult()
ValidateUserCredentials()
Параметри методів
формат: camelCase
назва параметра повинна бути короткою та змістовною
Приклади:
tournamentId
userId
matchResult
createTournamentRequest
Private-поля
формат: _camelCase
обов’язковий префікс нижнього підкреслення
Приклади:
_userRepository
_tournamentService
_logger
_jwtSettings
Публічні властивості
формат: PascalCase
Приклади:
Id
UserName
Email
CreatedAt
TournamentName
Локальні змінні
формат: camelCase
Приклади:
currentUser
tournamentList
matchParticipants
Boolean-змінні
мають починатися з is, has, can
Приклади:
isActive
hasAccess
canJoinTournament
Інтерфейси
формат: PascalCase
обов’язковий префікс I
Приклади:
IUserService
ITournamentRepository
IMatchGenerator
DTO / Request / Response моделі
формат: PascalCase
назва повинна містити суфікс, що пояснює призначення
Приклади:
RegisterUserRequest
LoginResponse
TournamentDetailsDto
UpdateMatchResultRequest
Файли
назва файлу повинна збігатися з назвою головної сутності всередині
Приклади:
TournamentService.cs
UserController.cs
RegisterUserRequest.cs
2.2. Frontend (Vue 3 + TypeScript)
Папки
формат: kebab-case
структура побудована по feature-based підходу
Приклади:
components/
tournaments/
user-profile/
auth/
match-bracket/
Компоненти
формат: PascalCase
файл має називатися так само, як і головний компонент усередині
Приклади:
TournamentCard.vue
TournamentList.vue
TournamentDetails.vue
MatchBracket.vue
UserProfile.vue
Назва компонента повинна відображати конкретну сутність, а не абстракцію.
Bad:
DataCard.vue
Item.vue
InfoBlock.vue
Good:
TournamentCard.vue
MatchResultCard.vue
PlayerStatsCard.vue
Сторінки (Pages / Views)
формат: PascalCase
для сторінок використовується суфікс Page
Приклади:
HomePage.vue
LoginPage.vue
RegisterPage.vue
TournamentListPage.vue
TournamentDetailsPage.vue
CreateTournamentPage.vue
UserProfilePage.vue
Форми
формат: PascalCase
для форм використовується суфікс Form
Приклади:
LoginForm.vue
RegisterForm.vue
CreateTournamentForm.vue
MatchResultForm.vue
Сервіси (API)
формат: camelCase
Приклади:
authService.ts
tournamentService.ts
userService.ts
matchService.ts
Утиліти (Utilities)
формат: camelCase
Приклади:
formatDate.ts
calculateBracket.ts
validateEmail.ts
Hooks / Composition API
формат: camelCase
обов’язковий префікс use
Приклади:
useAuth.ts
useTournament.ts
useBracket.ts
useFetch.ts
Стилі
для локальних стилів рекомендується використовувати scoped CSS у .vue файлах
для окремих CSS-файлів використовується kebab-case або назва, пов’язана з компонентом
Приклади:
tournament-card.css
match-bracket.css
Глобальні стилі:
main.css
variables.css
reset.css
Змінні
формат: camelCase
Приклади:
const tournamentList = []
const currentUser = {}
const matchResults = []
Boolean-змінні
мають починатися з is, has, can
Приклади:
const isLoading = false
const hasError = false
const canJoinTournament = true
const isAuthenticated = true
Функції
формат: camelCase
Приклади:
fetchTournaments()
createTournament()
joinTournament()
submitMatchResult()
Константи
формат: UPPER_SNAKE_CASE
Приклади:
API_URL
MAX_PLAYERS
DEFAULT_TOURNAMENT_TYPE
Props
назви props у script мають бути у camelCase
при передачі в template використовується kebab-case
Emits
назви подій мають бути у kebab-case
Приклади:
join-tournament
submit-result
open-modal
2.3. Database (PostgreSQL)
Таблиці
формат: snake_case
використовуються іменники у множині
Приклади:
users
tournaments
matches
tournament_participants
match_results
Колонки
формат: snake_case
використовуються іменники в однині
Приклади:
id
user_name
email
created_at
updated_at
tournament_name
start_date
match_status
Первинні ключі
назва: id
Приклад:
id
Зовнішні ключі
формат: сутність_id
Приклади:
user_id
tournament_id
match_id
winner_id
Таблиці зв’язку many-to-many
формат: snake_case
назва має відображати обидві сутності
Приклади:
tournament_participants
user_roles
Булеві поля
мають починатися з is_, has_, can_
Приклади:
is_active
has_paid_fee
can_edit
Поля дати та часу
використовуються зрозумілі суфікси
Приклади:
created_at
updated_at
started_at
finished_at
Enum-подібні поля
назва повинна відображати статус або тип
Приклади:
status
role
tournament_type
match_stage
Структура проєкту
Backend
src/
├── Controllers/
├── Services/
├── Repositories/
├── Models/
├── DTOs/
├── Validators/
└── Data/
Frontend
src/
├── components/
│ ├── tournament/
│ ├── match/
│ ├── user/
├── pages/
├── services/
├── utilities/
├── hooks/
├── router/
├── store/
└── assets/
Заборонені назви
Офіційно заборонено використовувати такі назви для файлів, класів, компонентів, таблиць, змінних або інших сутностей:
data
info
temp
test
item
object
manager
helper
utils
newData
someValue
value
entity
Забороняється використовувати абстрактні або сміттєві назви, які не описують призначення сутності.
Bad:
DataService
InfoComponent
TempTable
Helper
Utils
Item
Good:
TournamentService
MatchResultCard
TournamentParticipants
ValidateEmail
UserRepository
Formatting Rules
Загальні правила
відступ: 2 пробіли для Frontend, 4 пробіли для Backend
крапка з комою: обов’язково там, де це вимагає мова або стиль проєкту
максимальна довжина рядка: 100 символів
між логічними блоками коду має бути порожній рядок
не допускаються зайві порожні рядки в кінці файлу
кожен файл повинен містити лише ті сутності, які логічно з ним пов’язані
Backend (.NET)
фігурні дужки відкриваються з нового рядка
використовувати var лише там, де тип змінної очевидний
один клас або одна основна сутність на файл
using-директиви мають бути впорядковані
Frontend (Vue + TypeScript)
відступ: 2 пробіли
лапки: одинарні
один компонент = один файл
не розміщувати складну бізнес-логіку в template
уникати великих компонентів понад 300 рядків
Database
SQL-ключові слова рекомендується писати у верхньому регістрі
назви таблиць і колонок завжди у snake_case
скорочення використовуються лише якщо вони є загальноприйнятими
Коментарі
Backend
XML-коментарі обов’язкові для публічних методів сервісів і контролерів, якщо логіка не є очевидною
складна бізнес-логіка повинна супроводжуватися коротким поясненням
Frontend
коментарі є обов’язковими для API-викликів і складної логіки
не використовувати коментарі для очевидних речей
Database
для складних SQL-запитів або міграцій слід додавати пояснювальні коментарі
Приклад:
// Fetch tournaments list from backend
const fetchTournaments = async () => {}
Архітектурні правила
Backend
контролери відповідають лише за прийом і передачу даних
бізнес-логіка розміщується у services
робота з базою даних розміщується у repositories
DTO використовуються для передачі даних між шарами
Frontend
один компонент = одна відповідальність
бізнес-логіка розміщується у hooks
API-виклики розміщуються у services
утиліти розміщуються у utilities
UI-компоненти розміщуються у components
сторінки розміщуються у pages
Database
схема бази даних повинна бути консистентною з Backend-моделями через ORM
таблиці повинні мати зрозумілі зв’язки та передбачувану структуру ключів
Best Practices
Backend
використовувати dependency injection
не змішувати бізнес-логіку з логікою доступу до даних
уникати довгих методів понад 40–50 рядків
назви методів повинні відображати одну чітку дію
Frontend
використовувати Composition API
props повинні бути readonly
emit використовується для взаємодії між компонентами
не розміщувати складну логіку в template
уникати великих компонентів понад 300 рядків
Database
уникати дублювання даних
використовувати зовнішні ключі для підтримки цілісності
назви таблиць і колонок повинні бути передбачуваними та стабільними
Правила під MVP проєкту
Backend
UserController.cs
AuthController.cs
TournamentController.cs
MatchController.cs
TournamentService.cs
MatchService.cs
UserRepository.cs
TournamentRepository.cs
Frontend
Tournament:
TournamentCard.vue
TournamentList.vue
TournamentDetails.vue
CreateTournamentForm.vue
Match / Bracket:
MatchCard.vue
MatchResultForm.vue
BracketView.vue
User:
UserProfile.vue
UserStats.vue
Auth:
LoginForm.vue
RegisterForm.vue
Database
users
tournaments
matches
tournament_participants
match_results

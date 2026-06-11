# Tournament-Platform-System

Система управління турнірами, яка дозволяє користувачам створювати турніри, керувати учасниками та відстежувати результати матчів

## 👥 Команда проєкту
* **Анур'єва Катерина** — Project Manager
* **Скуртул Сергій** — Backend Developer
* **Дудко Володимир** — Database Engineer
* **Ярмолюк Людмила** — Frontend Developer
* **Загрбенюк Богдан** — QA Engineer

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

## 📁 Структура клієнтської частини
client/src/
├── api/              # Axios-інстанція, базові налаштування HTTP-запитів

├── assets/           # Статичні ресурси (зображення, шрифти)

│   └── icons/        # SVG-іконки

├── components/       # Перевикористовувані Vue-компоненти

│   ├── dashboard/    # Компоненти дашборду (картки турнірів, секції)

│   ├── forms/        # Форми (логін, реєстрація, створення турніру)

│   ├── modals/       # Модальні вікна

│   ├── tournament/   # Компоненти сторінки турніру (деталі, учасники, сітка)

│   └── ui/           # Базові UI-елементи (кнопки, інпути, спінери)

├── hooks/            # Композабли (useAuth, useTournament тощо)

├── router/           # Vue Router — визначення маршрутів

├── services/         # Сервіси для роботи з API (authService, tournamentService)

├── stores/           # Pinia-стори — глобальний стан застосунку

├── types/            # TypeScript-типи та інтерфейси

├── utils/            # Утиліти (форматування дат, валідація тощо)

└── views/            # Сторінки застосунку (HomePage, LoginPage тощо)


---
## 📸 Screenshots

### 🏠 Головна сторінка

![Головна сторінка](docs/screenshots/home_page.png)

Лендінг з hero-секцією, переліком активних турнірів та описом платформи для організаторів і гравців.

---

### 🔐 Авторизація

| Вхід | Реєстрація |
|------|------------|
| ![Вхід](docs/screenshots/log_in_page.png) | ![Реєстрація](docs/screenshots/sign_up_page.png) |

---

### 🎮 Гравець

**Дашборд** — персоналізований огляд активних та приєднаних турнірів.

![Дашборд гравця](docs/screenshots/player_dashboard.png)

**Перегляд турнірів** — повний список з пошуком і пагінацією.

![Сторінка турнірів](docs/screenshots/player_tournaments_page.png)

**Мої турніри** — турніри, до яких гравець приєднався.

![Мої турніри гравця](docs/screenshots/player_my_tournaments_page.png)

**Деталі турніру** — огляд, умови, учасники, сітка гравців.

| Подати заявку | Скасувати участь |
|---------------|-----------------|
| ![Подати заявку](docs/screenshots/player_detail_tournament_submit.png) | ![Скасувати участь](docs/screenshots/player_detail_tournament_cancel_participation.png) |

---

### 🛠️ Організатор

**Дашборд** — огляд створених турнірів, розділених на активні та керовані.

![Дашборд організатора](docs/screenshots/organizer_dashboard.png)

**Мої турніри** — повний список створених турнірів з пошуком.

![Мої турніри організатора](docs/screenshots/organizer_my_tournaments_page.png)

**Деталі турніру** — вигляд організатора з кнопкою редагування.

![Деталі турніру організатора](docs/screenshots/organizer_detail_tournament.png)

**Створення турніру** — форма з завантаженням банера, типом спорту, датами, описом та умовами.

![Створення турніру](docs/screenshots/create_tournament.png)

**Редагування турніру** — попередньо заповнена форма з керуванням учасниками та можливістю дискваліфікації.

![Редагування турніру](docs/screenshots/edit_tournament.png)
---


*Чернівці, 2026*

<div align="center">
  <img src="docs/screenshots/banner.png" alt="ZVYTIAHA Banner" width="100%"/>
</div>

# Tournament-Platform-System

<div align="center">

![Vue 3](https://img.shields.io/badge/Vue-3-4FC08D?style=flat&logo=vue.js)
![TypeScript](https://img.shields.io/badge/TypeScript-Vite-3178C6?style=flat&logo=typescript)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-.NET_9-512BD4?style=flat&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-4169E1?style=flat&logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat&logo=docker)
![Status](https://img.shields.io/badge/Status-Stable-brightgreen?style=flat)
![Version](https://img.shields.io/badge/Version-1.0.0-orange?style=flat)

</div>

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
* **[Swagger API](http://localhost:5050/swagger)** — інтерактивна API-документація (доступна локально після запуску стеку через `docker compose up`).

## Структура репозиторію
Згідно з обраною архітектурою, проєкт має наступну структуру папок:

* `📂 /docs` — Документація, ТЗ, діаграми, API-специфікації
* `📂 /client` — Frontend-частина застосунку.
* `📂 /server` — Backend-частина застосунку.
* `📂 /shared` — Спільні ресурси (типи даних, константи, переклади).
* `📂 /deploy` — Конфігурації для Docker, CI/CD, скрипти розгортання.

## Технологічний стек
* **Backend:** ASP.NET Core (.NET 9)
* **Frontend:** Vue 3 + TypeScript (Vite)
* **Database:** PostgreSQL 15
* **Інфраструктура:** Docker / Docker Compose

## 🚀 Інструкція з розгортання (Deployment Guide)

Проєкт повністю контейнеризований — для запуску достатньо Docker. Жодних локально встановлених .NET, Node чи PostgreSQL не потрібно.

### Системні вимоги

| Компонент | Мінімум | Протестовано на |
|-----------|---------|-----------------|
| Docker Engine | 20.10+ | 28.5.1 |
| Docker Compose | v2.0+ | v2.40.3 |
| RAM (виділено Docker) | 4 GB | 8 GB |
| CPU | 2 ядра | 4 ядра |
| Вільне місце на диску | ~3 GB | — |
| ОС | Windows 10/11, macOS, Linux | Windows 11 Pro |

> Достатньо встановленого **Docker Desktop** (включає Engine + Compose).

### Кроки запуску

```bash
# 1. Клонувати репозиторій
git clone https://github.com/katushhiaa/Tournament-Platform-System
cd Tournament-Platform-System

# 2. Створити файл змінних оточення з шаблону
cp .env.example .env        # Windows (cmd): copy .env.example .env
#   за потреби відредагувати паролі та JWT__KEY у .env

# 3. Підняти весь стек (db, backend, frontend, pgAdmin)
docker compose up --build
```

Перший запуск збирає образи та виконує міграції + наповнення БД автоматично (контейнер `init`) — це може зайняти кілька хвилин.

### Доступ до сервісів

| Сервіс | URL |
|--------|-----|
| Frontend (застосунок) | http://localhost:5173 |
| Backend API | http://localhost:5050/api/v1 |
| Swagger (API-документація) | http://localhost:5050/swagger |
| pgAdmin (керування БД) | http://localhost:8080 |

### Корисні команди

```bash
docker compose down          # зупинити стек
docker compose down -v       # зупинити + очистити БД (чистий перезапуск)
docker compose up --build    # перезібрати й підняти заново
```

> ⚠️ Перед перемиканням між гілками робіть `docker compose down -v`, щоб уникнути конфлікту міграцій.

## 🔑 Тестові доступи (Credentials)

- **Основний спосіб** — зареєструвати власний акаунт через кнопку **Sign Up** на `http://localhost:5173/register` (доступні ролі Organizer та Player).
- **Seed-користувачі** — при першому запуску БД наповнюється тестовими користувачами з `server/seeds/seed.sql`:
  - Організатори: `john.smith@example.com`, `jane.williams@example.com`
  - Гравці: `alex.johnson@example.com`, `michael.brown@example.com` та ін.
  - Пароль seed-користувачів зберігається у вигляді хешу; актуальний plaintext-пароль уточнюйте у Backend-розробника (Сергій Скуртул).

## 📂 Структура проєкту (де що знаходиться)

| Розташування | Призначення |
|--------------|-------------|
| `/client` | **Frontend** — Vue 3 + TypeScript (Vite) |
| `/server` | **Backend** — ASP.NET Core (.NET 9) |
| `/server/.../Infrastructure/Migrations` | **Міграції БД** (Entity Framework Core) |
| `/server/seeds/seed.sql` | Початкові тестові дані (seed) |
| `docker-compose.yml` | Оркестрація всіх сервісів |
| `.env.example` | Шаблон змінних оточення |

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

## 🎬 Demo
### Реєстрація
![Реєстрація](docs/gif/registration.gif)

### Пошук турнірів
![Пошук турнірів](docs/gif/search.gif)

### Взяти участь
![Взяти участь](docs/gif/to%20take%20part.gif)

### Скасувати участь
![Скасувати участь](docs/gif/cancel%20participation.gif)

### Генерація сітки
![Генерація сітки](docs/gif/grid%20generation.gif)

### Результати матчу
![Результати матчу](docs/gif/match%20results.gif)

### Дискваліфікація
![Дискваліфікація](docs/gif/disqualification.gif)

### Додати гравця 
![Додати гравця](docs/gif/add%20member.gif)

## 📱 Mobile View
### Головна
![Головна](docs/screenshots/home_dashboard(iPhone%2014%20Pro%20Max)%20(1).png)

### Дашборд гравця 
![Дашборд гравця ](docs/screenshots/player_dashboard(iPhone%2014%20Pro%20Max).png)

### Дашборд організатора 
![Дашборд організатора](docs/screenshots/organizer_dashboard(iPhone%2014%20Pro%20Max).png)

*Чернівці, 2026*

# LAB_TRIS
**Тема:** Система обработки обращений жильцов общежития
**Студент:** Болтенко Мария Васильевна
**Группа:** ТРИС-2-23
**Стек:** Vue 3 + ASP.NET Core + PostgreSQL
**Курс:** Б1.В.ДЭ.01.02.02 · РИС и Web · семестр 1 · 2026/2027

---

## Структура репозитория

```text
LAB_TRIS/
├── README.md            ← этот файл (поставка)
├── .gitignore
├── docs/                ← ЛР1–2 (ТЗ, архитектура, модель, API-контракт)
│   ├── api-collection/  ← коллекция запросов ЛР8
│   ├── reports/         ← отчёты lab-03 … lab-13
│   ├── role-matrix.md   ← матрица ролей ЛР11
│   ├── acceptance-check.md, known-defects.md, predefense-check.md
│   └── kursovaya-zapiska.md
├── frontend/            ← Vue 3 + Vite (ЛР3–6, 11)
├── backend/             ← ASP.NET Core Web API (ЛР7–8, 10–11)
└── db/                  ← schema.sql, seed.sql, queries.sql (ЛР9)
```

## Стек и версии

- Node.js LTS, npm; Vue 3, vue-router, Vite (клиент, порт 5173);
- .NET 8 SDK (сервер ASP.NET Core, порт 5000), EF Core + Npgsql;
- PostgreSQL 14+ (порт 5432).

## Подготовка (один раз)

1. Установить Node.js LTS, .NET 8 SDK, PostgreSQL.
2. Создать учебную базу:

   ```bash
   createdb dorm_tickets
   ```

3. Настроить строку подключения через user-secrets (не коммитится):

   ```bash
   cd backend
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=dorm_tickets;Username=postgres;Password=ВАШ_ПАРОЛЬ"
   ```

   В `appsettings.json` настоящего пароля нет — только ключ с пустым
   значением. Пароль PostgreSQL в Git не пишем.

4. Применить миграции (создаёт таблицы; учебные данные — в `db/seed.sql`):

   ```bash
   cd backend
   dotnet ef database update
   ```

5. Учебные данные для SQL-запросов (необязательно для работы API):

   ```bash
   psql -d dorm_tickets -f db/schema.sql   # пересоздаёт таблицы
   psql -d dorm_tickets -f db/seed.sql     # учебные данные, все 4 статуса
   ```

## Запуск

Терминал 1 — сервер:

```bash
cd backend
dotnet restore
dotnet run          # http://localhost:5000  Swagger: http://localhost:5000/swagger
```

Терминал 2 — клиент:

```bash
cd frontend
npm install
cp .env.example .env
npm run dev         # http://localhost:5173
```

В `.env`: `VITE_API_BASE_URL=http://localhost:5000` и `VITE_USE_MOCK=false`
(настоящий API). Значение `VITE_USE_MOCK=true` включает учебный mock
из ЛР6 — без сервера.

## Учебные учётки (ЛР11)

Способ входа — один: учебный JWT в заголовке `Authorization` (`POST /api/auth/login`).

| Логин | Роль | Пароль (учебный) |
|-------|------|------------------|
| `komendant` | Комендант — создаёт, назначает, отменяет | `Passw0rd!` |
| `santehnik` | Сантехник — меняет статус своих | `Passw0rd!` |
| `elektrik` | Электрик — меняет статус своих | `Passw0rd!` |

В базе пароли лежат хэшем (PBKDF2, ASP.NET Identity), это учебные учётки
только для лабораторного контура.

## Проверочный сценарий (ЛР12)

1. Войти комендантом на `http://localhost:5173/login`.
2. Создать обращение на `/new` — в Network `POST /api/tickets` → 201,
   номер `ОБР-2026-…`, статус `New`.
3. Назначить исполнителя (сантехник, id 2).
4. Войти исполнителем (`santehnik`) и перевести обращение в `InProgress`.
5. Один и тот же `id` виден в UI, в Network и в SQL:

   ```sql
   SELECT id, number, title, status, assignee_user_id
   FROM tickets
   WHERE id = 1;
   ```

6. Перезапустить API — запись на месте.

Полная таблица: `docs/acceptance-check.md`; сценарий демо на 7 минут —
`docs/predefense-check.md`.

## Служебные адреса сервера (ЛР7)

- `GET /health` — только доступность процесса (liveness); создание
  обращений он **не** проверяет, настоящее создание появилось в ЛР8.
- `GET /info` — имя приложения и среда.

## Проверка API без Vue

Импортировать `docs/api-collection/tickets.postman_collection.json`
в Postman, базовый URL — переменная `baseUrl` (`http://localhost:5000`).
В коллекции: вход (200), создание (201), короткая тема (400), список (200),
несуществующий id (404), статус без исполнителя (409), роль без права (403).

## Известные ограничения

- После перезапуска **backend в ЛР7–8** список пуст — данные были в памяти.
  С ЛР10 данные хранятся в PostgreSQL.
- Генерация номера `ОБР-2026-NNN` не защищена от гонки при одновременном
  создании (учебное ограничение).
- JWT-ключ по умолчанию используется только в Development; настоящий
  задаётся через `dotnet user-secrets set "Jwt:Key" "…"`.
- Полный список — `docs/known-defects.md`.

## Лабораторные работы и коммиты

| ЛР | Тема | Где смотреть | Коммит |
|----|------|--------------|--------|
| 1 | Анализ и требования | `docs/lab-01-requirements.md`, `docs/project-proposal.md`, `docs/requirements-matrix.md` | «ЛР1» |
| 2 | Архитектура и модель данных | `docs/architecture.md`, `docs/data-model.md`, `docs/api-contract.md` | «Лаб2» |
| 3 | Vue: каркас и экраны | `frontend/` | «ЛР3: каркас Vue и три экрана» |
| 4 | Vue: компоненты и маршруты | `frontend/src/router.js`, `pages/`, `components/` | «ЛР4: компоненты и маршруты Vue» |
| 5 | Vue: форма и список в памяти | `frontend/src/pages/TicketNew.vue`, `ticketsStore.js` | «ЛР5: форма и список в памяти» |
| 6 | Vue: клиент API | `frontend/src/api/`, отчёт `docs/reports/lab-06.md` | «ЛР6: клиент API, mock и состояния UI» |
| 7 | Каркас Web API | `backend/`, `/health`, `/info`, Swagger | «ЛР7: каркас ASP.NET Core Web API» |
| 8 | REST и валидация | `backend/Controllers/TicketsController.cs`, коллекция | «ЛР8: REST и валидация» |
| 9 | PostgreSQL и SQL | `db/` | «ЛР9: PostgreSQL и SQL» |
| 10 | EF Core и PostgreSQL | `backend/Data/`, `backend/Migrations/` | «ЛР10: EF Core и PostgreSQL» |
| 11 | Вход и доступ | `backend/Auth/`, `backend/Controllers/AuthController.cs`, `docs/role-matrix.md` | «ЛР11: вход и доступ» |
| 12 | Сквозная сборка | `docs/acceptance-check.md`, `docs/known-defects.md` | «ЛР12: сквозная сборка» |
| 13 | Предзащита | `docs/predefense-check.md`, `docs/kursovaya-zapiska.md`, `docs/reports/lab-13.md` | «ЛР13: предзащита» |

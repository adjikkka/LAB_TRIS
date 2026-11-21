# Отчёт по лабораторной работе № 10

| Поле | Значение |
|------|----------|
| Дисциплина | Б1.В.ДЭ.01.02.02 · РИС и Web |
| Студент | Болтенко Мария Васильевна |
| Группа | ТРИС-2-23 |
| Дата | 21.11.2026 |
| № ЛР | 10 |
| Тема работы | EF Core: сервер пишет в базу |
| Тема проекта | Система обработки обращений жильцов общежития |

## 1. Цель

Сервер через EF Core пишет в PostgreSQL: создали запись → перезапустили
API → запись на месте. JSON и коды ЛР8 не меняются; строка подключения
с паролем не в Git.

## 2. Что сделали

1. Установили пакеты в `backend`:
   `Microsoft.EntityFrameworkCore.Design`, `Npgsql.EntityFrameworkCore.PostgreSQL`,
   глобальный инструмент `dotnet-ef` (`dotnet ef --version` отвечает).
2. Настроили секрет локально: `dotnet user-secrets init`, затем
   `dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=dorm_tickets;Username=postgres;Password=…"`.
   В `appsettings.json` настоящего пароля нет; `git grep -i password`
   находит только пустой пример.
3. Создали `Data/AppDbContext.cs` (`DbSet` для комнат, пользователей,
   обращений). Имена таблиц и столбцов заданы явно и совпадают со схемой
   ЛР9 (`rooms`, `users`, `tickets`, snake_case, CHECK статусов — см.
   таблицу соответствия ниже).
4. Миграция: `dotnet ef migrations add InitialCreate`,
   `dotnet ef database update`. Папка `Migrations` добавлена в Git —
   у соседа есть что применять.
5. Зарегистрировали контекст в `Program.cs` через Npgsql
   (`UseNpgsql(builder.Configuration.GetConnectionString("Default"))`).
6. Перевели `TicketService` с списка в памяти на `AppDbContext`:
   POST, два GET и оба PATCH выполняют те же проверки ЛР8, но данные
   читаются и сохраняются в PostgreSQL. Контроллер почти не изменился.
7. Выполнили обязательный рестарт-тест (см. раздел 3).

## 3. Обязательный рестарт-тест

1. Запустили PostgreSQL и API (`http://localhost:5000`).
2. Создали обращение через `POST /api/tickets`, записали
   `id = ___`, `number = ОБР-2026-___`, `title = «___»`.
3. Остановили API `Ctrl+C`, запустили `dotnet run` снова.
4. `GET /api/tickets/{id}` — та же тема, статус и номер.

```sql
SELECT id, number, title, status
FROM tickets
WHERE id = ___;
```

Ожидание и факт: один и тот же `id` до и после рестарта — данные больше
не живут в памяти процесса.

*(место для скрина: GET после рестарта + результат SELECT)*

## 4. Соответствие имён C# ↔ PostgreSQL

| C# | PostgreSQL (столбец) |
|---|---|
| `Id` | `id` |
| `Number` | `number` |
| `Title` | `title` |
| `Description` | `description` |
| `Status` | `status` |
| `RoomId` | `room_id` |
| `CreatedAt` | `created_at` |
| `CreatedByUserId` | `created_by_user_id` |
| `AssigneeUserId` | `assignee_user_id` |
| `Room` / `User` | `rooms` / `users` |

## 5. Пара «LINQ ↔ SQL» для отчёта

```csharp
// строка C# (TicketService.GetTicketsAsync)
query.Where(t => t.AssigneeUserId == userId);
```

Тот же смысл в SQL из ЛР9:

```sql
SELECT * FROM tickets WHERE assignee_user_id = <userId>;
```

## 6. Доказательство отказа (404 и 409 сохраняются)

- `GET /api/tickets/999999` после перевода на EF — 404, а не 500
  (проверка результата поиска осталась).
- Если человеческий номер повторился бы — сервис вернул бы 409;
  нумерация считается по году (`ОБР-2026-…`) внутри одной транзакции
  сервиса, известное учебное ограничение — см. `docs/known-defects.md`.

## 7. Что осталось на следующую работу

1. Вход и права: 401/403, хэш пароля, матрица ролей (ЛР11).
2. Сквозная сборка Vue + API + PostgreSQL (ЛР12).

## 8. Запуск

```bash
# одноразовая настройка секрета (не коммитится)
cd backend
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=dorm_tickets;Username=postgres;Password=ВАШ_ПАРОЛЬ"

dotnet ef database update
dotnet run
```

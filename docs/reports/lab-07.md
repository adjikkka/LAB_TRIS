# Отчёт по лабораторной работе № 7

| Поле | Значение |
|------|----------|
| Дисциплина | Б1.В.ДЭ.01.02.02 · РИС и Web |
| Студент | Болтенко Мария Васильевна |
| Группа | ТРИС-2-23 |
| Дата | 21.11.2026 |
| № ЛР | 7 |
| Тема работы | ASP.NET Core: каркас Web API |
| Тема проекта | Система обработки обращений жильцов общежития |

## 1. Цель

Поднять в репозитории каркас ASP.NET Core Web API на порту 5000:
Swagger в Development, служебные адреса `/health` и `/info`, папки
`Controllers / Services / Models / Data` и сервис, зарегистрированный
через DI. Сосед поднимает сервер по README.

## 2. Что сделали

1. Проверили SDK: `dotnet --version` (.NET 8 или новее).
2. Создали проект в корне репозитория, рядом с `/frontend` и `/docs`:
   `dotnet new webapi -o backend --use-controllers`.
3. `dotnet restore` и `dotnet run` — сервер стартует без ошибок.
4. В `Properties/launchSettings.json` зафиксировали HTTP-порт:
   `http://localhost:5000` — терминал пишет `Now listening on: http://localhost:5000`.
5. Включили Swagger в режиме Development (`UseSwagger` / `UseSwaggerUI`):
   `http://localhost:5000/swagger` открывает список endpoint.
6. Добавили служебные адреса (Minimal API — допустимо только для них):
   - `GET /health` → 200 и текст `ok`;
   - `GET /info` → имя приложения, проект, среда `Development`, пометка ЛР7.
7. Создали папки `Controllers/`, `Services/`, `Models/`, `Data/`
   (в Git пустых папок не бывает — внутри реальные файлы-заготовки).
8. Сервис зарегистрирован через DI в `Program.cs`
   (`AddScoped<ITicketService, TicketService>()`), контроллер получает его
   через конструктор и не создаёт через `new`.
9. В корневой `.gitignore` добавили `bin/` и `obj/`.
10. Обновили README: команды запуска, порт, адрес Swagger, ограничения.

## 3. Доказательство успеха

- (место для скрина: страница Swagger `http://localhost:5000/swagger`)
- (место для скрина: ответ `GET /health` — `ok`)
- (место для скрина: ответ `GET /info`)

Запуск как сосед: остановили сервер `Ctrl+C`, выполнили команды README
с начала, открыли Swagger, отдельно вызвали `/health`.

## 4. Чеклист «/health ≠ создание обращения»

| Утверждение | да/нет |
|-------------|--------|
| `/health` возвращает `200` без тела заявки | да |
| `/health` не принимает `title`, `roomId`, `description` | да |
| Успешный `/health` не доказывает работу `POST /api/tickets` | да |
| Создание главного объекта относится к ЛР8 | да |
| README прямо объясняет различие (liveness ≠ создание) | да |
| Swagger и `/health` проверены отдельно | да |

## 5. Доказательство отказа

При первом запуске `/swagger` давал 404 — middleware Swagger не был
включён до `Run`. После переноса `UseSwagger` / `UseSwaggerUI` в блок
Development страница открывается. (Укажите свой случай, если отличался.)

## 6. Что осталось на следующую работу

1. Реализовать REST главного ресурса: POST, два GET, два PATCH (ЛР8).
2. Подключать Vue не сегодня — стыковка в ЛР12.

## 7. Запуск

```bash
cd backend
dotnet restore
dotnet run
```

- API: `http://localhost:5000`
- Swagger: `http://localhost:5000/swagger`
- `/health` — только доступность процесса (liveness), создание обращений
  он не проверяет.

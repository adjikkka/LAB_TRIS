-- ЛР9 · Учебные данные
-- Представлены все четыре статуса: New, InProgress, Closed, Cancelled.
-- Паролей в файле нет: в столбце password_hash лежит хэш учебного пароля
-- (PBKDF2, формат ASP.NET Identity v3, 10 000 итераций).
-- Учебные логины и пароль указаны в README (учебный контур, ЛР11).

-- Повторный запуск: сначала очищаем учебные данные
DELETE FROM tickets;
DELETE FROM rooms;
DELETE FROM users;

INSERT INTO rooms (id, name) VALUES
  (1, 'Комната 305'),
  (2, 'Комната 203'),
  (3, 'Комната 410'),
  (4, 'Комната 111');

-- Учебный пароль для всех учёток: Passw0rd! (см. README, ЛР11)
INSERT INTO users (id, name, login, role, password_hash) VALUES
  (1, 'Анна Петровна Смирнова', 'komendant', 'Комендант',
   'AQAQJwAA7eqU2MvXZLr504c4/2oA83mokBJp1j+ue3VHhz2yuLYSVEtbdI4haI9LzvNARVbl'),
  (2, 'Игорь Волков', 'santehnik', 'Сантехник',
   'AQAQJwAAQDhVIySR9wTi/hg5KwlHsmkMVLzj812X65wjMnhIavUCgTa/4pzD4RC9PQhfi82R'),
  (3, 'Павел Семёнов', 'elektrik', 'Электрик',
   'AQAQJwAAY1YYweBKmGjUQC8BWHUX4nzel6rUAP3RwmtqE1kKETavD1zOkQD2ij294O4SfNel');

-- Обращения: New (2), InProgress (2), Closed (2), Cancelled (1)
INSERT INTO tickets
  (id, number, title, description, status, room_id, created_at, created_by_user_id, assignee_user_id) VALUES
  (1, 'ОБР-2026-001', 'Протекает кран в ванной',
   'Капает кран в ванной, под раковиной собралась лужа',
   'New', 1, '2026-11-02 10:15:00+03', 1, NULL),
  (2, 'ОБР-2026-002', 'Не работает розетка',
   'Розетка у письменного стола не даёт питание',
   'InProgress', 2, '2026-11-03 12:40:00+03', 1, 3),
  (3, 'ОБР-2026-003', 'Скрипит дверь в душевой',
   'Дверь душевой на четвёртом этаже скрипит и заедает',
   'Closed', 3, '2026-10-28 09:00:00+03', 1, 2),
  (4, 'ОБР-2026-004', 'Сломан стул в комнате',
   'Отломалась спинка стула, сидеть опасно',
   'Cancelled', 4, '2026-10-20 18:30:00+03', 1, NULL),
  (5, 'ОБР-2026-005', 'Перегорели лампы на этаже',
   'На третьем этаже не горят две лампы в коридоре',
   'New', 2, '2026-11-05 08:05:00+03', 1, NULL),
  (6, 'ОБР-2026-006', 'Засор в раковине',
   'Вода в раковине уходит медленно, есть запах',
   'InProgress', 1, '2026-11-06 14:20:00+03', 1, 2),
  (7, 'ОБР-2026-007', 'Не закрывается окно',
   'Окно комнаты не закрывается до конца, дует',
   'Closed', 4, '2026-11-07 11:00:00+03', 1, 3);

-- Сдвигаем счётчики идентификаторов: дальше id создаёт PostgreSQL,
-- а API продолжает нумерацию после вставленных строк (ЛР10, рестарт-тест)
SELECT setval(pg_get_serial_sequence('rooms', 'id'),   (SELECT MAX(id) FROM rooms));
SELECT setval(pg_get_serial_sequence('users', 'id'),   (SELECT MAX(id) FROM users));
SELECT setval(pg_get_serial_sequence('tickets', 'id'), (SELECT MAX(id) FROM tickets));

-- Проверка ограничения CHECK (ЛР9, раздел 6.4).
-- Раскомментируйте, чтобы убедиться: PostgreSQL отклонит статус Done.
-- INSERT INTO tickets (number, title, description, status, room_id, created_at, created_by_user_id)
-- VALUES ('ОБР-2026-999', 'Проверка CHECK', 'Строка для проверки ограничения статуса', 'Done', 1, now(), 1);

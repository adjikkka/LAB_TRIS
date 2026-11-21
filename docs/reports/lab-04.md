# Отчёт по лабораторной работе № 4

| Поле | Значение |
|------|----------|
| Дисциплина | Б1.В.ДЭ.01.02.02 · РИС и Web |
| Студент | Болтенко Мария Васильевна |
| Группа | ТРИС-2-23 |
| Дата | 21.11.2026 |
| № ЛР | 4 |
| Тема работы | Vue 3: компоненты и маршруты |
| Тема проекта | Система обработки обращений жильцов общежития |

## 1. Цель

Декомпозировать каркас ЛР3 на компоненты и страницы, подключить
`vue-router`: шапка остаётся на месте при переходах, адреса `/`, `/new`,
`/login` открываются ручным вводом, таблица получает данные через props,
а общий массив лежит в одном модуле `ticketsStore.js`.

## 2. Что сделали

1. Установили `vue-router` (`npm install vue-router`).
2. Подключили роутер в `src/main.js`: `app.use(router)` до `mount`.
3. Создали `src/router.js` с тремя маршрутами (`createRouter` + `createWebHistory`):
   `/` — `TicketList.vue`, `/new` — `TicketNew.vue`, `/login` — `Login.vue`.
4. Разметку трёх экранов из ЛР3 перенесли в `src/pages/`.
5. Вынесли шапку в `src/components/AppHeader.vue`: меню из `router-link`,
   а не из кнопок с `v-if`. В `App.vue` остался только каркас
   `<AppHeader />` + `<router-view />` — шапка снаружи `router-view`.
6. Вынесли таблицу в `src/components/TicketTable.vue`: обязательный prop
   `items: { type: Array, required: true }`, собственный массив и копия
   props (`ref([...props.items])`) отсутствуют.
7. Создали `src/ticketsStore.js` — единственный общий массив обращений.
8. Проверили все три адреса ручным вводом в адресной строке, не только
   кликами по меню; шапка не пропадает при смене маршрута.

## 3. Доказательство успеха

- (место для скрина: `http://localhost:5173/new` с адресной строкой)
- (место для скрина: дерево `frontend/src/` в проводнике)

Дерево `src/`:

```text
frontend/src/
├── main.js               ← app.use(router)
├── App.vue               ← AppHeader + router-view
├── router.js             ← три маршрута
├── ticketsStore.js       ← единственный общий массив
├── assets/main.css
├── api/                  ← появится в ЛР6
├── components/
│   ├── AppHeader.vue
│   └── TicketTable.vue
└── pages/
    ├── TicketList.vue
    ├── TicketNew.vue
    └── Login.vue
```

Схема «шапка снаружи, страница внутри»:

```text
App.vue
├── AppHeader   (router-link: /  /new  /login)   ← всегда виден
└── <router-view />
    ├── TicketList   путь /
    ├── TicketNew    путь /new
    └── Login        путь /login
```

Таблица получает массив через props, источник — `ticketsStore.js`.

## 4. Доказательство отказа (как исправляли)

Первая попытка ввода `/new` вручную давала белую страницу: роутер был
установлен, но не подключён в `main.js`. После добавления `app.use(router)`
до `mount` все три адреса открываются и после обновления страницы.

## 5. Что осталось на следующую работу

1. Связать форму с общим массивом и добавить валидацию (ЛР5).
2. Перевести список и форму на API-модуль с mock (ЛР6).

## 6. Запуск

```bash
cd frontend
npm install
npm run dev
```

Адреса: `http://localhost:5173/`, `/new`, `/login`.

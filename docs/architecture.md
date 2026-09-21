# Архитектура системы

## 1. Общая схема

Система состоит из трёх основных узлов:

1. клиентское приложение Vue 3 + Vite;
2. серверное приложение ASP.NET Core;
3. база данных PostgreSQL.

```mermaid
flowchart LR
    A["Vue 3 + Vite<br/>localhost:5173"]
    B["ASP.NET Core API<br/>localhost:5000"]
    C["PostgreSQL<br/>localhost:5432"]

    A -->|"HTTP<br/>POST /api/tickets"| B
    B -->|"SQL-запросы"| C
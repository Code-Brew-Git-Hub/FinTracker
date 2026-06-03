# Запуск через Docker Compose

## Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (или Docker Engine + Compose v2)

## Запуск из готовых образов (GHCR)

Без локальной сборки (быстрее, рекомендуется для релизов):

```bash
cp .env.example .env
# FINTRACKER_VERSION=latest  или v1.0.0 из GitHub Releases
docker compose -f docker-compose.images.yml pull
docker compose -f docker-compose.images.yml up -d
```

Образы: `ghcr.io/code-brew-git-hub/fintracker-api` и `fintracker-frontend`. Подробнее: [releases.md](releases.md).

## Быстрый старт (сборка из исходников)

```bash
# из корня монорепозитория
cp .env.example .env   # опционально
docker compose up --build
```

| Сервис   | URL |
|----------|-----|
| Frontend | http://localhost:8080 |
| API      | http://localhost:5009 |
| Swagger  | http://localhost:5009/swagger |

Миграции БД применяются автоматически при старте API.

## Остановка

```bash
docker compose down
```

Данные PostgreSQL сохраняются в volume `postgres_data`. Чтобы удалить и их:

```bash
docker compose down -v
```

## Переменные окружения

См. [.env.example](../.env.example): пароль БД, порты `API_PORT` и `FRONTEND_PORT`.

Фронтенд обращается к API по `http://localhost:5009/api` (как при локальной разработке).

## Только пересборка одного сервиса

```bash
docker compose up --build api
docker compose up --build frontend
```

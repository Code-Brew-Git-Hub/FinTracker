# Справка по запуску FinTracker

Краткая техническая справка. **Если вы не программист** — начните с [docker-for-beginners.md](docker-for-beginners.md) или [README](../README.md).

---

## Что нужно

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows / macOS) или Docker Engine + Compose (Linux)

---

## Windows: самый простой способ

```powershell
.\start.bat    # запуск
.\stop.bat     # остановка
```

`start.bat` создаёт `.env`, скачивает или собирает компоненты и открывает http://localhost:8080.

Если скачивание с GitHub не работает (`unauthorized`):

```powershell
.\start.ps1 -Build
```

---

## Адреса после запуска

| Что | Адрес |
|-----|-------|
| Сайт (интерфейс) | http://localhost:8080 |
| API | http://localhost:5009 |
| Swagger (документация API) | http://localhost:5009/swagger |

Порт сайта можно изменить в `.env` (`FRONTEND_PORT`).

---

## Два способа запуска

### 1. Готовые образы (быстрее)

Скачиваются собранные версии программы с GitHub. Нужен файл `docker-compose.images.yml`.

```bash
cp .env.example .env
docker compose -f docker-compose.images.yml pull
docker compose -f docker-compose.images.yml up -d
```

В `.env` укажите версию: `FINTRACKER_VERSION=v0.0.3` (см. [releases.md](releases.md)).

### 2. Сборка из исходников (надёжнее, если образы недоступны)

```bash
cp .env.example .env
docker compose up --build -d
```

Первый запуск: 5–15 минут.

---

## Остановка

```bash
# если запускали через образы:
docker compose -f docker-compose.images.yml down

# если собирали из исходников:
docker compose down
```

**Данные сохраняются.** Чтобы удалить базу полностью:

```bash
docker compose -f docker-compose.images.yml down -v
docker compose down -v
```

---

## Настройки

Файл [.env.example](../.env.example) → скопируйте в `.env`:

| Переменная | Назначение |
|------------|------------|
| `DB_PASSWORD` | Пароль локальной PostgreSQL |
| `FRONTEND_PORT` | Порт сайта (по умолчанию 8080) |
| `API_PORT` | Порт API (по умолчанию 5009) |
| `FINTRACKER_VERSION` | Версия образов (`v0.0.3`, `latest`) |
| `GHCR_OWNER` | Организация на GitHub (`code-brew-git-hub`) |

---

## Как устроен запуск

```
Браузер → frontend (nginx) → api (.NET) → postgres (база данных)
```

- **frontend** и **api** — образы FinTracker (GHCR или локальная сборка)
- **postgres** — стандартный образ `postgres:17-alpine` с Docker Hub
- Данные БД — в Docker-volume `postgres_data` на вашем диске
- Схема базы создаётся автоматически при старте API

---

## Пересборка одного компонента (для разработчиков)

```bash
docker compose up --build api
docker compose up --build frontend
```

---

## См. также

- [Запуск с нуля для начинающих](docker-for-beginners.md)
- [Релизы и версии](releases.md)

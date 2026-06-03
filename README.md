# FinTracker

Сервис для сбора и агрегирования финансовых трат 

## Что внутри

- `FinTracker.Backend` — ASP.NET Core API + PostgreSQL
- `FinTracker.Frontend` — клиентская часть (HTML/CSS/JavaScript)
- `docs` — документация по запуску и аналитике

## Быстрый старт (Docker Compose)

Нужна подробная инструкция с нуля: [docs/docker-for-beginners.md](docs/docker-for-beginners.md)

### Windows: двойной клик или скрипт

```powershell
.\start.bat          # готовые образы GHCR + открытие браузера
.\start.ps1 -Build   # сборка из исходников
.\stop.bat           # остановка
```

### Готовые образы (без сборки, рекомендуется)

```bash
cp .env.example .env
docker compose -f docker-compose.images.yml pull
docker compose -f docker-compose.images.yml up -d
```

Версию можно зафиксировать в `.env`: `FINTRACKER_VERSION=v1.0.0` (см. [GitHub Releases](https://github.com/Code-Brew-Git-Hub/FinTracker/releases)).

### Сборка из исходников

```bash
docker compose up --build
```

После запуска:

- Frontend: [http://localhost:8080](http://localhost:8080)
- API: [http://localhost:5009](http://localhost:5009)
- Swagger: [http://localhost:5009/swagger](http://localhost:5009/swagger)

Остановка:

```bash
docker compose down
# для образов из GHCR:
docker compose -f docker-compose.images.yml down
```

## Переменные окружения

Можно создать локальный `.env` на основе шаблона:

```bash
cp .env.example .env
```

Основные переменные:

- `DB_NAME`, `DB_USER`, `DB_PASSWORD`
- `API_PORT`, `FRONTEND_PORT`
- `GHCR_OWNER`, `FINTRACKER_VERSION` — для `docker-compose.images.yml`

## Релизы

- Готовые Docker-образы на GHCR и выпуск версий: [docs/releases.md](docs/releases.md)
- Список релизов: [GitHub Releases](https://github.com/Code-Brew-Git-Hub/FinTracker/releases)

## Документация

- Подробный запуск для начинающих: [docs/docker-for-beginners.md](docs/docker-for-beginners.md)
- Запуск в Docker: [docs/docker.md](docs/docker.md)
- Синхронизация субрепозиториев: [docs/sync-subrepos.md](docs/sync-subrepos.md)
- Аналитика (папка): [docs/analitics](docs/analitics)
- Требования заказчика: [docs/analitics/Требования заказчика.md](docs/analitics/Требования%20заказчика.md)
- Общая аналитика: [docs/analitics/analytic.md](docs/analitics/analytic.md)
- Модуль агрегации: [docs/analitics/agregation_module.md](docs/analitics/agregation_module.md)
- Валидация транзакций: [docs/analitics/analytic_transaction_validation.md](docs/analitics/analytic_transaction_validation.md)
- Переводы между счетами: [docs/analitics/analytic_transfers_between_accounts.md](docs/analitics/analytic_transfers_between_accounts.md)
- Компенсации: [docs/analitics/analytic_compensations.md](docs/analitics/analytic_compensations.md)
- Позиции (массив): [docs/analitics/analytic_positions_array.md](docs/analitics/analytic_positions_array.md)

## Команда

- [Куратор](https://github.com/RefGnom)
- [Backend-разработчик](https://github.com/trydov1k)
- [Frontend-разработчик](https://github.com/Omnifisans)
- [UX/UI-проектировщик](https://github.com/MATFIX-coder)
- [Системный аналитик](https://github.com/Alexandr2807)
- [DevSecOps-инженер](https://github.com/Alexey-Eremin-lalala)

# FinTracker

Сервис для сбора и агрегирования финансовых трат.

## Структура репозитория

```
FinTracker/
├── FinTracker.Backend/   # ASP.NET Core API
├── FinTracker.Frontend/  # React + Vite
└── docs/                 # Требования и аналитика
```

## Локальный запуск

**Backend** — `FinTracker.Backend/FinTracker.API`, порт `5009`  
**Frontend** — `FinTracker.Frontend`, порт `5173` (`VITE_API_URL=http://localhost:5009`)

## Синхронизация с отдельными репозиториями

Backend и Frontend могут жить в отдельных репозиториях на GitHub. При push в `main` этих репозиториев изменения можно автоматически подтягивать сюда.

### 1. Секрет в монорепозитории

В **Settings → Secrets → Actions** репозитория `FinTracker` добавьте:

| Secret | Описание |
|--------|----------|
| `MONOREPO_SYNC_TOKEN` | [Fine-grained PAT](https://github.com/settings/tokens) с доступом на чтение `FinTracker.Backend`, `FinTracker.Frontend` и запись в `FinTracker` |

### 2. Триггеры в отдельных репозиториях

Скопируйте шаблоны в корень каждого репозитория как `.github/workflows/sync-to-monorepo.yml`:

- Backend: [.github/templates/sync-to-monorepo.backend.yml](.github/templates/sync-to-monorepo.backend.yml)
- Frontend: [.github/templates/sync-to-monorepo.frontend.yml](.github/templates/sync-to-monorepo.frontend.yml)

В **Backend** и **Frontend** добавьте тот же секрет `MONOREPO_SYNC_TOKEN` (PAT с правом `repository_dispatch` на монорепозиторий).

После push в `main` backend/frontend → workflow шлёт сигнал → монорепозиторий копирует актуальный код.

### 3. Ручной и фоновый sync

Workflow [sync-from-subrepos.yml](.github/workflows/sync-from-subrepos.yml) также запускается:

- вручную: **Actions → Sync from Backend / Frontend repos → Run workflow**
- по расписанию: раз в час (на случай, если триггер не настроен)

Целевая ветка монорепозитория задаётся переменной `MONOREPO_BRANCH` в workflow (по умолчанию `main`).

## Документация

- [Требования](docs/Требования.md)
- [Аналитика проекта](docs/analytic.md)
- [Модуль агрегации](docs/agregation_module.md)

## Участники

- [Куратор](https://github.com/RefGnom)
- [Backend-разработчик](https://github.com/trydov1k)
- [Frontend-разработчик](https://github.com/Omnifisans)
- [UX/UI-проектировщик](https://github.com/MATFIX-coder)
- [Системный аналитик](https://github.com/Alexandr2807)
- [DevSecOps-инженер](https://github.com/Alexey-Eremin-lalala)

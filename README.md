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

Backend и Frontend также живут в отдельных репозиториях на GitHub. При push в `main` изменения автоматически подтягиваются в этот монорепозиторий (ветка `Dima's-final-version`).

Workflow [sync-from-subrepos.yml](.github/workflows/sync-from-subrepos.yml) также можно запустить вручную (**Actions → Sync from Backend / Frontend repos**) или дождаться hourly sync.

Источники: `Code-Brew-Git-Hub/FinTracker.Backend`, `trydov1k/FinTracker.Frontend`.

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

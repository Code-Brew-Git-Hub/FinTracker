# Публикация FinTracker на сервере (только через GitLab)

Деплой **полностью автоматический**: вы пушите код в GitLab → runner на docker-host собирает образы и публикует сайт. **SSH на сервер не нужен.**

Локальный запуск на ПК (`start.bat`) не меняется — в `.env` на ПК оставьте `DEPLOY_MODE=local`.

---

## Схема

```
git push (main)
      │
      ▼
GitLab (192.168.0.21) — pipeline
      │
      ▼
Runner на docker-host (192.168.0.20)
  · checkout кода
  · создание .env из CI Variables
  · docker compose build && up
      │
      ▼
https://fintracker.trydov1k.online
```

---

## Что нужно сделать один раз

### 1. DNS (панель домена trydov1k.online)

| Тип | Имя | Значение |
|-----|-----|----------|
| A | `fintracker` | IP docker-host (`192.168.0.20` или внешний IP с пробросом 80/443) |

### 2. Переменные в GitLab

Проект → **Settings → CI/CD → Variables**:

| Переменная | Обязательна | Masked | Пример |
|------------|-------------|--------|--------|
| `DB_PASSWORD` | да | да | надёжный пароль |
| `LETSENCRYPT_EMAIL` | да | нет | you@mail.com |
| `DOMAIN` | нет | нет | `fintracker.trydov1k.online` |
| `PUBLIC_URL` | нет | нет | `https://fintracker.trydov1k.online` |

Значения по умолчанию для `DOMAIN` и `PUBLIC_URL` уже заданы в [`.gitlab-ci.yml`](../.gitlab-ci.yml).

### 3. Runner

Runner должен быть на **docker-host** и уметь вызывать `docker compose` (обычно executor **shell**, пользователь `gitlab-runner` в группе `docker`).

Если у runner задан **tag**, раскомментируйте `tags` в `.gitlab-ci.yml` и укажите свой tag.

---

## Ежедневная работа

```bash
git add .
git commit -m "..."
git push origin main
```

Pipeline сам:

1. Создаст `.env` из переменных GitLab
2. Соберёт образы api и frontend из исходников
3. Поднимет postgres + api + frontend + caddy (HTTPS)

Сайт: **https://fintracker.trydov1k.online**

---

## Локальный ПК vs сервер

| | ПК (Windows) | Сервер |
|---|--------------|--------|
| Запуск | `start.bat` | `git push` в `main` |
| Настройки | `.env` в папке проекта | GitLab CI/CD Variables |
| URL | http://localhost:8080 | https://fintracker.trydov1k.online |

---

## Устранение неполадок

| Проблема | Решение |
|----------|---------|
| Job не стартует | Проверьте, что runner online и tag в `.gitlab-ci.yml` совпадает |
| `permission denied` docker | На docker-host: `sudo usermod -aG docker gitlab-runner` (один раз при настройке runner) |
| Сертификат не выдаётся | DNS, проброс портов 80/443, верный `DOMAIN` |
| 502 Bad Gateway | Дождитесь healthy у postgres; смотрите логи job в GitLab |
| CORS | `PUBLIC_URL` должен совпадать с URL в браузере |

Логи на docker-host (если понадобится):

```bash
docker compose -p fintracker -f docker-compose.yml -f docker-compose.server.yml logs -f
```

---

## Безопасность

- Не коммитьте `.env` и пароли в git
- `DB_PASSWORD` храните только в GitLab Variables (masked)
- API снаружи не публикуется — только `/api` через frontend

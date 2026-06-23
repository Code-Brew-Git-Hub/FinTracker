# Релизы FinTracker

FinTracker выпускается **версиями** (например v0.0.3). Каждая версия — это готовая сборка программы для установки у себя на компьютере.

---

## Для пользователей

### Где скачать

[github.com/Code-Brew-Git-Hub/FinTracker/releases](https://github.com/Code-Brew-Git-Hub/FinTracker/releases)

На странице релиза:

1. Прочитайте описание версии
2. В блоке **Assets** скачайте **Source code (zip)**
3. Распакуйте архив
4. Запустите **`start.bat`** (см. [docker-for-beginners.md](docker-for-beginners.md))

### Как указать версию

Откройте файл `.env` в папке FinTracker блокнотом и найдите строку:

```
FINTRACKER_VERSION=latest
```

Замените на номер скачанного релиза:

```
FINTRACKER_VERSION=v0.0.3
```

| Значение | Когда использовать |
|----------|-------------------|
| `v0.0.3` | Конкретная версия — **рекомендуется** |
| `latest` | Всегда последняя сборка (может измениться без предупреждения) |

### Запуск после скачивания

**Windows:**

```
start.bat
```

**Вручную (если нужно):**

```bash
cp docker/.env.example .env
docker compose -f docker/docker-compose.images.yml pull
docker compose -f docker/docker-compose.images.yml up -d
```

### Обновление

1. Скачайте новый релиз
2. Скопируйте старый `.env` в новую папку (сохранятся пароль и порты)
3. Обновите `FINTRACKER_VERSION` в `.env`
4. Запустите **`start.bat`**

### Если скачивание не работает (`unauthorized`)

Образы на GitHub могут быть недоступны без входа. Запустите сборку на своём ПК:

```powershell
.\scripts\start.ps1 -Build
```

---

## Что входит в релиз

| Компонент | Где хранится | Скачивается |
|-----------|--------------|-------------|
| Сайт и сервер FinTracker | GitHub (образы GHCR) | При `start.bat` или `pull` |
| База PostgreSQL | Docker Hub | Автоматически при первом запуске |
| Ваши данные | На вашем диске (volume Docker) | Не входят в релиз — остаются у вас |

---

## Для maintainer (разработчиков)

### Как выпустить новую версию

1. Убедитесь, что workflow [`.github/workflows/docker-release.yml`](../.github/workflows/docker-release.yml) есть в `main`
2. Создайте и запушьте тег:

```bash
git tag v0.0.4
git push origin v0.0.4
```

3. GitHub Actions соберёт образы и создаст страницу релиза

При push в `main` / `autodeploy` обновляется только тег `latest` (без новой страницы релиза).

### Ручной запуск сборки

**Actions → Docker images & Release → Run workflow** — поле `tag` (например `latest`).

### Обязательно: публичные пакеты на GitHub

По умолчанию образы **приватные** — у пользователей будет ошибка `unauthorized`.

После первой сборки:

1. [github.com/orgs/Code-Brew-Git-Hub/packages](https://github.com/orgs/Code-Brew-Git-Hub/packages)
2. **fintracker-api** → Package settings → **Change visibility** → **Public**
3. То же для **fintracker-frontend**

### Имена образов

| Образ | Назначение |
|-------|------------|
| `ghcr.io/code-brew-git-hub/fintracker-api` | Сервер (API) |
| `ghcr.io/code-brew-git-hub/fintracker-frontend` | Сайт |

### Теги образов

| Событие | Теги |
|---------|------|
| Push в `main` / `autodeploy` | `latest` |
| Push тега `v*` | `v0.0.3`, `latest` |
| Ручной workflow | значение из поля `tag` |

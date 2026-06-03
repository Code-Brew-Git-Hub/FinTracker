# Релизы и Docker-образы (GHCR)

Для self-hosted публикуются готовые образы в [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry):

| Образ | Назначение |
|-------|------------|
| `ghcr.io/<owner>/fintracker-api` | ASP.NET Core API |
| `ghcr.io/<owner>/fintracker-frontend` | nginx + статика |

`<owner>` — владелец репозитория в **нижнем регистре** (например `code-brew-git-hub`).

## Запуск для пользователя

```bash
cp .env.example .env
# в .env: FINTRACKER_VERSION=v1.0.0  (или latest)
docker compose -f docker-compose.images.yml pull
docker compose -f docker-compose.images.yml up -d
```

Сборка из исходников (как раньше): `docker compose up --build`.

## Как выпустить релиз (maintainer)

1. Убедитесь, что workflow [`.github/workflows/docker-release.yml`](../.github/workflows/docker-release.yml) есть в ветке по умолчанию.
2. Создайте и запушьте тег SemVer:

   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

3. GitHub Actions соберёт образы с тегами `v1.0.0` и `latest`, создаст [GitHub Release](https://github.com/Code-Brew-Git-Hub/FinTracker/releases) с инструкцией.

При push в `main` или `autodeploy` обновляется только тег образа `latest` (без GitHub Release).

### Ручной запуск workflow

**Actions → Docker images & Release → Run workflow** — поле `tag` (например `latest` или `v1.0.1-preview`).

### Первый раз: видимость пакетов

После первой успешной сборки в организации **Code-Brew-Git-Hub**:

1. **Packages** → `fintracker-api` / `fintracker-frontend`
2. **Package settings → Change visibility → Public** (для публичного self-hosted без `docker login`).

Или привяжите видимость к публичному репозиторию FinTracker (настройки репозитория → Packages).

## Теги образов

| Событие | Теги на GHCR |
|---------|----------------|
| Push в `main` / `autodeploy` | `latest` |
| Push тега `v*` | `v1.0.0`, `latest` |
| `workflow_dispatch` | значение из поля `tag` |

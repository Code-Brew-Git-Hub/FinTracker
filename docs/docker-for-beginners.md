# Запуск FinTracker с нуля (подробная пошаговая инструкция)

Этот гайд для тех, кто запускает проект с нуля: без опыта Git, Docker и терминала.

## 0) Что получится в итоге

После выполнения инструкции у вас откроется сайт:

- `http://localhost:8080`

## 1) Что нужно установить заранее

### 1.1 Git (чтобы скачать проект)

1. Перейдите на сайт [https://git-scm.com/download/win](https://git-scm.com/download/win)
2. Скачайте установщик и установите Git (нажимайте Next, стандартные настройки подходят).
3. После установки перезапустите компьютер (желательно).

### 1.2 Docker Desktop (чтобы запустить проект)

1. Скачайте Docker Desktop: [https://www.docker.com/products/docker-desktop/](https://www.docker.com/products/docker-desktop/)
2. Установите как обычную программу.
3. Перезагрузите компьютер, если установщик попросит.
4. Запустите Docker Desktop.
5. Дождитесь статуса "Engine running" (обычно внизу окна Docker).

## 2) Создайте папку для проектов

Можно, например, использовать `C:\coding`.

1. Откройте проводник.
2. Создайте папку `C:\coding` (если ее нет).

## 3) Скачайте репозиторий FinTracker

Ниже два варианта. Выберите любой:

- Вариант A (рекомендуется): через Git
- Вариант B: через ZIP-архив (без Git)

### 3.1 Откройте PowerShell

- Нажмите `Win + X` -> `Terminal` или `Windows PowerShell`.

### 3.2 Перейдите в папку `C:\coding`

```powershell
cd C:\coding
```

### 3.3 Клонируйте репозиторий

```powershell
git clone git@github.com:Code-Brew-Git-Hub/FinTracker.git
```

Если у вас не настроен SSH-ключ, используйте HTTPS:

```powershell
git clone https://github.com/Code-Brew-Git-Hub/FinTracker.git
```

### 3.4 Перейдите в папку проекта

```powershell
cd FinTracker
```

## 3B) Альтернатива: скачать проект ZIP-архивом (без Git)

Если не хотите ставить/использовать Git, можно скачать проект как архив.

1. Откройте страницу репозитория: [https://github.com/Code-Brew-Git-Hub/FinTracker](https://github.com/Code-Brew-Git-Hub/FinTracker)
2. Нажмите зеленую кнопку `Code`.
3. Выберите `Download ZIP`.
4. Дождитесь загрузки архива.
5. Распакуйте архив в `C:\coding`.
6. Переименуйте распакованную папку в `FinTracker` (если нужно).

После этого откройте PowerShell и перейдите в папку проекта:

```powershell
cd C:\coding\FinTracker
```

## 4) Проверьте, что все установилось правильно

Выполните команды:

```powershell
docker --version
docker compose version
```

Если команды вывели версии без ошибок - все хорошо.

Если вы выбрали вариант A (через Git), дополнительно можно проверить:

```powershell
git --version
```

## 5) Подготовьте `.env` (рекомендуется)

Скопируйте шаблон:

```powershell
Copy-Item .env.example .env
```

Если файл `.env` уже существует, шаг пропустите.

## 6) Проверьте, что вы в корне проекта

В папке должны быть:

- `docker-compose.yml`
- папка `FinTracker.Backend`
- папка `FinTracker.Frontend`

Проверить можно командой:

```powershell
dir
```

## 7) Первый запуск проекта

Выполните:

```powershell
docker compose up --build
```

Что важно знать:

- первый запуск может занять 5-15 минут
- в терминале будет много текста - это нормально
- не закрывайте это окно терминала, пока пользуетесь проектом

## 8) Проверка результата

Откройте в браузере:

- `http://localhost:8080`

Если сайт открылся - запуск успешен.

## 9) Как остановить проект

1. В терминале, где крутится `docker compose up`, нажмите `Ctrl + C`.
2. Затем выполните:

```powershell
docker compose down
```

## 10) Как запустить второй и последующие разы

Обычно хватает:

```powershell
docker compose up
```

Если после изменений в проекте нужна пересборка:

```powershell
docker compose up --build
```

## 11) Обновить проект перед запуском

Если репозиторий уже скачан через Git и хотите получить последние изменения:

```powershell
cd C:\coding\FinTracker
git pull
docker compose up --build
```

Если вы скачивали проект ZIP-архивом, обновлять нужно повторным скачиванием нового ZIP с GitHub.

## 12) Частые проблемы и решения

### Ошибка при `git clone`

- Проверьте интернет.
- Если использовали SSH и получили ошибку доступа, попробуйте HTTPS-вариант из шага 3.3.

### Docker не запускается

- Убедитесь, что Docker Desktop открыт.
- Дождитесь "Engine running".
- Перезапустите Docker Desktop.
- Если не помогло - перезагрузите компьютер.

### Ошибка "port is already allocated"

Это значит, что нужный порт уже занят другой программой.

Что делать:

1. Закройте программы, которые могут использовать порт `8080`.
2. Либо измените `FRONTEND_PORT` в файле `.env`.
3. Перезапустите проект:

```powershell
docker compose down
docker compose up --build
```

### Нужно "сбросить все" и начать заново

```powershell
docker compose down -v
docker compose up --build
```

## 13) Полезные команды

```powershell
# показать запущенные контейнеры
docker compose ps

# посмотреть логи
docker compose logs

# посмотреть логи frontend
docker compose logs frontend
```

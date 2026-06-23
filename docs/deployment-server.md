# Публикация FinTracker на сервере (только через GitLab)

Деплой **полностью автоматический**: push в GitLab → runner на docker-host собирает и публикует FinTracker.

На docker-host у вас уже есть **Nginx Proxy Manager (npm)** на портах 80/443 и **cadvisor** на 8080 — FinTracker использует порт **8082** и проксируется через npm.

---

## Схема

```
git push (main) → GitLab CI → docker-host
                                  │
                    fintracker-frontend :8082
                                  │
                    Nginx Proxy Manager :443
                                  │
                    https://fintracker.trydov1k.online
```

---

## Один раз

### 1. DNS

| Тип | Имя | Значение |
|-----|-----|----------|
| A | `fintracker` | IP docker-host (`192.168.0.20` или внешний) |

### 2. GitLab CI/CD Variables

| Переменная | Обязательна | Пример |
|------------|-------------|--------|
| `DB_PASSWORD` | да | надёжный пароль |
| `DOMAIN` | нет | `fintracker.trydov1k.online` |
| `PUBLIC_URL` | нет | `https://fintracker.trydov1k.online` |
| `FINTRACKER_HOST_PORT` | нет | `8082` (не 8080 — занят cadvisor) |

### 3. Nginx Proxy Manager (один раз в веб-интерфейсе npm)

**Hosts → Proxy Hosts → Add Proxy Host:**

| Поле | Значение |
|------|----------|
| Domain Names | `fintracker.trydov1k.online` |
| Scheme | `http` |
| Forward Hostname / IP | `192.168.0.20` (IP docker-host) |
| Forward Port | `8082` |
| SSL | Request a new SSL Certificate (Let's Encrypt) |

Сохраните. HTTPS настраивается в npm, не в FinTracker.

---

## Ежедневная работа

```bash
git push origin main
```

Pipeline соберёт образы и поднимет контейнеры. Сайт: **https://fintracker.trydov1k.online**

---

## Устранение неполадок

| Проблема | Решение |
|----------|---------|
| `port is already allocated` | Порт 8080 занят (cadvisor). Используйте `FINTRACKER_HOST_PORT=8082` |
| 502 в npm | В npm: Forward Port **8082** (не 8080). Проверка: `curl http://192.168.0.20:8082` |
| CORS | `PUBLIC_URL` в GitLab Variables = URL в браузере |
| `No space left on device` | `docker system prune -af && docker builder prune -af` на docker-host, затем Retry |
| postgres unhealthy | Смотрите логи job; при нехватке диска — очистка Docker |

Логи:

```bash
docker compose -p fintracker -f docker/docker-compose.yml -f docker/docker-compose.server.yml logs -f
```

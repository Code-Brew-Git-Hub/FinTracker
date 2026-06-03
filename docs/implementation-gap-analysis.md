# Анализ соответствия реализации документации `docs/analitics`

**Дата:** 2026-06-03  
**Источники:** 7 документов в `docs/analitics/`  
**Проверено:** `FinTracker.Backend`, `FinTracker.Frontend`

---

## Общая картина

| Статус | Доля |
|--------|------|
| ✅ Реализовано | ~45% |
| ⚠️ Частично | ~35% |
| ❌ Не реализовано | ~20% |

Backend покрывает базовый сценарий (импорт → CRUD → фильтры → scope/теги → аналитика). Не хватает целых модулей из документации: **валидация**, **нормализация**, **иерархия категорий**, **полноценные позиции**, **расширенные компенсации/переводы**. Frontend реализует основной UI, но не использует часть API (links, positions, validation).

---

## По документам

### 1. `analytic.md` — общая спецификация

| Требование | Статус | Комментарий |
|------------|--------|-------------|
| CSV-импорт | ⚠️ | Только T-Bank и Alfa-Bank с фиксированными заголовками и разделителями (`CsvParser.cs`). Нет пользовательского маппинга колонок |
| Excel/PDF (расширяемость) | ❌ | Только `.csv` |
| Ручной ввод | ✅ | `POST api/transactions`, UI в `upload.js` |
| CRUD + soft delete | ✅ | `TransactionsController`, `IsDeleted` |
| Фильтрация (дата, сумма, категория, тип, теги, scope) | ✅ | `TransactionFilter`, `TransactionRepository.GetFilteredAsync` |
| Поиск по описанию | ⚠️ | Только `Description.Contains`, без комментария/тегов |
| Тип transfer в enum | ❌ | `TransactionType` — только `Expense` / `Income`; переводы через links |
| Поле `source` (csv/manual) | ❌ | Нет в модели `Transaction` |
| Категории + иерархия parent-child | ❌ | `Category` — только `Id`, `Name` |
| Теги (несколько на транзакцию) | ✅ | M2M через `TransactionTag` |
| Scope (группы) | ⚠️ | CRUD есть, но нет `description` (закомментировано в модели) |
| Исключение scope из аналитики | ✅ | `ExcludeScopeIds` в `AnalyticsFilterDto` |
| Связи: компенсации и переводы | ⚠️ | См. отдельные документы ниже |
| Массовое редактирование | ⚠️ | `PATCH api/transactions/bulk` — категория, scope, теги, комментарий; нет bulk soft-delete |
| Нормализация описаний | ❌ | Только trim при парсинге CSV |
| Аналитика (время, категории, теги, scope) | ✅ | `AnalyticsService` + `AnalyticsController` |
| Исключение переводов/компенсаций | ✅ | `ExcludeTransfers` (default true), `ExcludeCompensations` |
| Данные для графиков (JSON API) | ✅ | Backend готов; UI отображает таблицы, не графики (по документу это ок) |
| Механизм уточнения транзакций | ⚠️ | Фильтр + редактирование + bulk в UI (`transactions.js`), но без links/positions |
| Unit/e2e тесты | ❌ | Тестовых проектов нет |
| PostgreSQL + .NET | ✅ | EF Core, миграция `20260520162946_Init` |

---

### 2. `Требования заказчика.md`

| Требование заказчика | Статус |
|----------------------|--------|
| Scope для событий (8 марта) + исключение из агрегации | ✅ Backend + UI |
| Компенсирующие транзакции (долг, частичный возврат, залог) | ⚠️ Базовая связь есть, ролей/сумм/степени компенсации нет |
| Переводы между счетами | ⚠️ Связь через `TransactionLink`, без статусов suggested/confirmed |
| Механизм уточнения (фильтр → правки) | ⚠️ Реализован для категорий/тегов/scope/комментариев |
| Теги на несколько транзакций | ✅ |
| Проверка корректности / дубликаты | ❌ Модуль отсутствует |
| Массив позиций в транзакции | ⚠️ CRUD позиций есть, аналитика по позициям — нет |

---

### 3. `agregation_module.md` — модуль агрегации

| Требование | Статус |
|------------|--------|
| Summary (доходы, расходы, баланс) | ✅ `GetSummaryAsync` |
| По времени (день/неделя/месяц) | ✅ `GetByTimeAsync` + `TimeGrouping` |
| По категориям | ✅ |
| По тегам | ✅ |
| По scope | ✅ `GetByScopeAsync` |
| Фильтры (период, сумма, категория, тип, теги, scope) | ✅ |
| Исключение переводов | ✅ default `ExcludeTransfers = true` |
| Исключение компенсаций | ✅ параметр есть, default false |
| Исключение scope по списку | ✅ `ExcludeScopeIds` |
| Агрегация по позициям (если есть) | ❌ `AnalyticsService` работает только с транзакциями |
| Frontend: параметры исключения | ❌ `analytics.js` не передаёт `ExcludeScopeIds`, `ExcludeCompensations` |

---

### 4. `analytic_compensations.md`

| Требование | Статус | Реализация |
|------------|--------|------------|
| Связь нескольких транзакций | ✅ | `TransactionLink` + `TransactionLinkEntry` |
| Тип Compensation | ✅ | `TransactionLinkType.Compensation` |
| Несколько компенсирующих транзакций | ✅ | `AddTransactionAsync` |
| Роли base/compensation | ❌ | `TransactionLinkEntry` — только ID связи |
| Сумма участия в компенсации | ❌ | Нет поля `amount` |
| Расчёт степени компенсации | ❌ | |
| `CompensationGroup` с title, comment, is_closed | ❌ | Упрощённая модель |
| Признак компенсации у транзакции | ⚠️ | Только через `LinkEntries`, нет явного флага в DTO |
| Исключение из агрегации | ✅ | `ExcludeCompensations` |
| Soft delete связи | ❌ | Hard delete (`LinkService.DeleteAsync`) |
| Запрет участия в нескольких группах | ❌ | Нет проверки |
| UI для компенсаций | ❌ | Frontend не вызывает `/api/links` |

---

### 5. `analytic_transfers_between_accounts.md`

| Требование | Статус |
|------------|--------|
| Связь двух транзакций как перевод | ✅ `TransactionLinkType.Transfer` |
| `TransferLink` с outgoing/incoming | ❌ | Обобщённая модель без направлений |
| Статусы suggested/confirmed/rejected | ❌ |
| Исключение из аналитики | ✅ |
| Ручное связывание | ✅ API `POST api/links` |
| Автодетект | ❌ (в документе явно «не входит») |
| Одна транзакция — одна активная связь | ❌ Нет валидации |
| UI | ❌ |

---

### 6. `analytic_positions_array.md`

| Требование | Статус |
|------------|--------|
| CRUD позиций | ✅ `PositionsController` → `api/transactions/{id}/items` |
| Категория на уровне позиции | ✅ |
| Теги на уровне позиции | ❌ |
| `quantity`, `unit_price`, `comment` | ❌ |
| `has_positions` | ❌ Нет явного флага (есть навигация `Positions`) |
| Soft delete позиций | ❌ Hard delete в `PositionRepository` |
| Позиции в ответе транзакции | ❌ `TransactionDto` без positions |
| `PositionDto` с Id | ❌ Нет `Id` в DTO |
| Аналитика по позициям | ❌ |
| Наследование категории транзакции | ❌ |
| UI позиций | ❌ |

Текущая модель позиции (`FinTracker.Domain/Models/Position.cs`): `Id`, `Name`, `Amount`, `TransactionId`, `CategoryId`.

---

### 7. `analytic_transaction_validation.md`

| Требование | Статус |
|------------|--------|
| `ValidationRule` | ❌ |
| `ValidationIssue` | ❌ |
| Поиск идентичных транзакций | ❌ |
| Поиск дублей после пересекающихся импортов | ❌ |
| Статусы new/confirmed/rejected/resolved | ❌ |
| Подтверждение пользователем → soft delete | ❌ |
| Запуск после импорта | ❌ |
| Настраиваемые правила | ❌ |
| API валидации | ❌ |

Есть только row-level валидация при парсинге CSV (`CsvParser.Validate`).

---

## Что уже работает хорошо

- **Импорт CSV** (2 банка) → `ImportController` / `ImportService`
- **Транзакции**: CRUD, soft delete, ручное создание
- **Фильтрация и пагинация** транзакций
- **Теги, scope, категории** (плоские)
- **Массовое редактирование** (категория, scope, теги, комментарий) — backend + UI
- **Аналитика**: summary, by-category/tag/scope/time с исключением переводов
- **Frontend**: upload, transactions (уточнение), analytics (таблицы)

---

## Критичные пробелы (по приоритету)

1. **Валидация / дубликаты** — целый модуль из двух документов отсутствует
2. **Позиции в аналитике** — API позиций есть, но агрегация их не учитывает
3. **Компенсации/переводы** — упрощённая модель без ролей, сумм, статусов; нет UI
4. **Иерархия категорий** — не реализована
5. **Нормализация описаний** — не реализована
6. **Тесты** — нет unit/e2e проектов
7. **Frontend** не использует `/api/links`, `/api/transactions/{id}/items`, validation API

---

## Рекомендуемый порядок доработки

1. ValidationIssue + duplicate detection
2. Позиции в `AnalyticsService`
3. Расширение `TransactionLink` (роли, суммы, soft delete, статусы переводов)
4. UI для links и positions
5. Иерархия категорий + нормализация
6. Тесты (Parser, Analytics, Validation)

---

## GitHub Issues

Задачи из анализа перенесены в Issues субрепозиториев (2026-06-03):

- [FinTracker.Backend — все open issues](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues)
- [FinTracker.Frontend — все open issues](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues)

### Backend (22 задачи)

| Issue | Задача |
|-------|--------|
| [#12](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/12) | ValidationRule и ValidationIssue |
| [#13](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/13) | Правило поиска полностью идентичных транзакций |
| [#14](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/14) | Запуск валидации после импорта |
| [#15](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/15) | Расширить модель Position по документации |
| [#16](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/16) | Позиции в ответах Transaction API |
| [#17](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/17) | Агрегация по позициям в AnalyticsService |
| [#18](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/18) | TransactionLink: роли и суммы компенсации |
| [#19](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/19) | Метаданные группы компенсации |
| [#20](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/20) | Статусы переводов (suggested/confirmed/rejected) |
| [#21](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/21) | GET api/links — список связей |
| [#22](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/22) | Расширить поиск транзакций |
| [#23](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/23) | Метаданные пагинации в списке транзакций |
| [#24](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/24) | Иерархия категорий (parent_id) — backend |
| [#25](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/25) | Модуль нормализации описаний |
| [#26](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/26) | Поле source у транзакции (csv/manual) |
| [#27](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/27) | Расширяемый CSV: маппинг колонок и разделители |
| [#28](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/28) | Импорт: partial success при ошибках строк |
| [#29](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/29) | Description у Scope — backend |
| [#30](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/30) | Bulk soft-delete и расширение BulkUpdate — backend |
| [#31](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/31) | Unit-тесты: Parser и ImportService |
| [#32](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/32) | Unit-тесты: AnalyticsService и LinkService |
| [#33](https://github.com/Code-Brew-Git-Hub/FinTracker.Backend/issues/33) | Integration/e2e тесты API |

### Frontend (7 задач)

| Issue | Задача |
|-------|--------|
| [#1](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues/1) | UI экрана валидации и подтверждения дублей |
| [#2](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues/2) | UI редактирования позиций транзакции |
| [#3](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues/3) | UI для связывания компенсаций и переводов |
| [#4](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues/4) | Параметры исключения в аналитике |
| [#5](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues/5) | Иерархия категорий — UI |
| [#6](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues/6) | Description у Scope — UI |
| [#7](https://github.com/Code-Brew-Git-Hub/FinTracker.Frontend/issues/7) | Bulk soft-delete — UI |

# Файл с предположительной архитектурой проекта
## FinTracker.Backend 
(Бэкендная часть проекта, без фронтенда)

```
FinTracker.Backend/
│
├── src/
│   ├── Core/
│   │   ├── FinTracker.Domain.csproj/                      // Проект для всех сущностей решения
|   |   |   ├── ParsedTransaction.cs                       // Файл, описывающий сущность распареного csv
|   |   |   └── Transaction.cs                             // Файл, описывающий сущность транзакции, с которой мы будем работать
|   |   |   
│   │   └── FinTracker.Application.csproj/                 // Проект для ...
|   |      └── InterFaces/                                 // Папка с интерфайсами для решения
|   |          └── IFinTrackerDbContext.cs                 // Интерфейс для DbContext
|   |
│   ├── Infrastucture/
│   │   └── FinTracker.Persistence.csproj/                 // Проект для работы с базой данных
|   |       ├── Configurations/                            // Папка с конфигурациями сущностей проекта, которые мы будем отправлять в бд
|   |       |   └── TransactionConfiguration.cs            // Конфигурация сущности транзакция
|   |       ├── DbInitializer.cs                           // Файл, который при запуске решения проверит, существует ли бд (не уверен, что нужен)
|   |       └── FinTrackerDbContext.cs                     // Файл, который создаёт DbContext для бд
|   |
│   └── Presentastion/
│       └── FinTracker.API.csproj/                         // Проект для создания, настройки API
|           ├── Properties/
|           ├── appsettings.json                           // Файл с настройками приложения
|           └── Program.cs                                 // Главный файл, из которого будет запускаться решение
│
└── tests/
    └── FinTracker.Tests.csproj/                           // Проект с Unit тестами
        └── UnitTest1.cs
```

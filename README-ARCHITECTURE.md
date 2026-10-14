# Кінокаталог: шари та зв'язки між ними

Рішення `Kino.sln` побудоване за принципами **Clean Architecture** і складається з чотирьох проєктів.

```
Kino.sln
├── Kino.Domain           Class Library       - сутності та контракти доступу до даних
├── Kino.Application      Class Library       - бізнес-логіка, DTO, інтерфейси сервісів
├── Kino.Infrastructure   Class Library       - EF Core, SQL Server, збереження файлів
└── Kino.Web              ASP.NET Core MVC    - контролери, в'юхи, валідація форм
```

## Зв'язки між проєктами

```
Kino.Web  ──►  Kino.Application  ──►  Kino.Domain
   │                  ▲                    ▲
   └──►  Kino.Infrastructure ──────────────┘
```

| Проєкт | Посилається на | Не має права посилатися на |
|---|---|---|
| **Kino.Domain** | ні на що | будь-який інший проєкт, EF Core, ASP.NET Core |
| **Kino.Application** | Domain | Infrastructure, Web, EF Core, `DbContext`, `IFormFile` |
| **Kino.Infrastructure** | Application, Domain | Web, ASP.NET Core |
| **Kino.Web** | Application; Infrastructure — **лише** для Composition Root у `Program.cs` | `DbContext` у контролерах |

Головне правило: **залежності спрямовані всередину, до Domain**. Внутрішні шари визначають
інтерфейси, а зовнішні їх реалізують (Dependency Inversion).

| Інтерфейс | Де оголошено | Де реалізовано |
|---|---|---|
| `IRepository<T>` | Domain | `Infrastructure/Repositories/Repository.cs` |
| `IPosterStorage` | Application | `Infrastructure/Services/PosterStorage.cs` |
| `IMovieService` | Application | `Application/Services/MovieService.cs` |

## Kino.Domain

| Файл | Що робить |
|---|---|
| `Entities/Movie.cs` | чиста сутність без атрибутів валідації та без `IFormFile` |
| `Interfaces/IRepository.cs` | узагальнений контракт доступу до даних |

NuGet-пакетів немає взагалі.

## Kino.Application

| Файл | Що робить |
|---|---|
| `DTO/MovieDto.cs` | об'єкт передачі даних між рівнями |
| `Interfaces/IMovieService.cs` | контракт бізнес-логіки |
| `Interfaces/IPosterStorage.cs` | абстракція сховища постерів (приймає `Stream`, а не `IFormFile`) |
| `Services/MovieService.cs` | бізнес-правила, робота з постерами, логування |
| `Mapping/MovieMapper.cs` | `Movie` ↔ `MovieDto` |
| `DependencyInjection.cs` | `AddApplication()` |

Пакети: лише `Microsoft.Extensions.*.Abstractions`.

## Kino.Infrastructure

| Файл | Що робить |
|---|---|
| `Persistence/AppDbContext.cs` | контекст, конфігурація таблиці, початкові дані (10 фільмів) |
| `Repositories/Repository.cs` | реалізація `IRepository<T>` на EF Core |
| `Services/PosterStorage.cs` | реалізація `IPosterStorage`: зберігає файли в `wwwroot/images/uploads` |
| `DependencyInjection.cs` | `AddInfrastructure(configuration, webRootPath)` та `InitializeDatabase()` |

Пакети: `Microsoft.EntityFrameworkCore.SqlServer`. Шлях до `wwwroot` передається рядком,
тому Infrastructure не залежить від `IWebHostEnvironment` і ASP.NET Core.

## Kino.Web

| Файл | Що робить |
|---|---|
| `Program.cs` | **Composition Root** — реєструє Application та Infrastructure |
| `Controllers/MoviesController.cs` | тонкий контролер, знає лише `IMovieService` |
| `ViewModels/*` | моделі форм з атрибутами валідації та `IFormFile`, маппінг у `MovieDto` |
| `Validation/*` | кастомні атрибути валідації |
| `Localization/*`, `Resources/*` | JSON-локалізація UA / EN / FR |
| `Views/*`, `wwwroot/*` | розмітка, стилі, постери |

## Що змінилося порівняно з версією PL / BLL / DAL

1. **Напрямок залежностей розвернуто.** Було `PL → BLL → DAL`: бізнес-логіка залежала від
   бази даних. Тепер `Application → Domain`, а `Infrastructure` сама залежить від внутрішніх шарів.
2. **З'явився Domain.** Сутність `Movie` та `IRepository<T>` перенесено з DAL у Domain.
3. **DAL став Infrastructure.** Сюди ж переїхала реалізація `PosterStorage` з рівня представлення.
4. **Application більше не підключає DAL.** Реєстрація сервісів відбувається в одному місці —
   `Program.cs` проєкту Web (Composition Root).

## Запуск

1. Відкрити `Kino.sln`.
2. Перевірити рядок підключення в `Kino.Web/appsettings.json`.
3. Стартовий проєкт — `Kino.Web`, далі F5 (або `dotnet run --project Kino.Web`).

База створюється автоматично під час старту разом із початковими даними.

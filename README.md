# Магазин винтажной посуды

Курсовой проект по дисциплине **«Кроссплатформенная среда исполнения программного обеспечения»**.
Веб-приложение для каталогизации и продажи винтажной посуды
(тарелки, чашки, сервизы, вазы) на ASP.NET Core 8 + Blazor Server.

---

## Содержание

1. [О проекте](#о-проекте)
2. [Стек технологий](#стек-технологий)
3. [Требования](#требования)
4. [Быстрый старт (без Docker)](#быстрый-старт-без-docker)
5. [Запуск через Docker](#запуск-через-docker)
6. [Учётные данные по умолчанию](#учётные-данные-по-умолчанию)
7. [Структура проекта](#структура-проекта)
8. [Базовые операции](#базовые-операции)
9. [Команды разработчика](#команды-разработчика)
10. [Что куда коммитить (git workflow)](#что-куда-коммитить-git-workflow)
11. [FAQ и частые проблемы](#faq-и-частые-проблемы)

---

## О проекте

Приложение реализует электронный каталог магазина винтажной посуды:
- учёт предметов с привязкой к заводу-изготовителю, категории и материалу;
- хранение фото, тегов, подробных характеристик (происхождение, состояние, дефекты);
- статусы «В наличии», «Продано», «Архив»;
- поиск по названию, заводу, году, статусу;
- два отчёта: «Что в наличии» и «Что продано» с агрегатами по сумме;
- авторизация (логин, регистрация, роли Admin / Manager).

Проект демонстрирует:
- **Entity Framework Core (Code First)** с миграциями и Fluent API,
- все три типа отношений: **1:1**, **1:N**, **N:N**,
- **Dependency Injection**, репозитории, сервисный слой,
- **Blazor Server**: формы, валидация, модальные окна,
- **FluentValidation** в EditForm,
- **ASP.NET Core Identity** с ролями,
- **Docker** + multi-stage build + healthcheck,
- **Unit-тесты** (xUnit) с покрытием бизнес-логики >70%.

---

## Стек технологий

| Слой | Технология |
| --- | --- |
| Платформа | .NET 8 |
| Веб-фреймворк | ASP.NET Core 8 |
| UI | Blazor Server (Razor Components, InteractiveServer rendering) |
| ORM | Entity Framework Core 8 (Code First, миграции) |
| СУБД | SQLite |
| Аутентификация | ASP.NET Core Identity + cookies |
| Валидация | FluentValidation 11 + Blazored.FluentValidation |
| Health checks | AspNetCore.HealthChecks.Sqlite |
| Тесты | xUnit, FluentAssertions, Moq, Microsoft.Data.Sqlite (in-memory) |
| Контейнеризация | Docker (multi-stage), docker-compose |

---

## Требования

Для **локального запуска без Docker**:
- .NET 8 SDK ([скачать](https://dotnet.microsoft.com/download/dotnet/8.0)) или `brew install dotnet@8` на macOS;
- Git.

Для **запуска через Docker**:
- Docker Desktop (или другой совместимый рантайм с поддержкой Compose v2).

---

## Быстрый старт (без Docker)

```bash
# 1. Клонировать репозиторий
git clone https://github.com/<username>/vintage-posuda.git
cd vintage-posuda

# 2. Восстановить зависимости NuGet
dotnet restore

# 3. (Опционально) Применить миграции вручную.
#    Если этот шаг пропустить — приложение применит миграции
#    автоматически при первом запуске.
dotnet ef database update --project src/VintagePosuda.Web

# 4. Запустить веб-сервер
dotnet run --project src/VintagePosuda.Web
```

После запуска приложение доступно по адресу, который выведет в консоль (обычно
`http://localhost:5000` или `https://localhost:5001`).

При первом запуске автоматически:
- создаётся файл базы `app.db` рядом с `Program.cs`;
- применяются миграции;
- сидится 8 категорий, 6 материалов, 6 тегов, 6 заводов и 3 демо-предмета;
- создаются роли `Admin`, `Manager` и админ-аккаунт из секции `AdminUser`.

---

## Запуск через Docker

```bash
# Собрать образ и поднять контейнер
docker compose up --build

# Проверить healthcheck
curl http://localhost:8080/health
# → Healthy

# Остановить
docker compose down
```

Особенности:
- образ собирается в **multi-stage** Dockerfile (sdk → aspnet, итоговый размер ~210 МБ);
- база SQLite хранится в **named volume** `app-data` (переживает `docker compose down/up`);
- `HEALTHCHECK` опрашивает `/health` каждые 30 секунд; `docker compose ps` покажет статус
  `healthy`, когда приложение готово.

После старта приложение доступно на `http://localhost:8080`.

---

## Учётные данные по умолчанию

Данные первого администратора задаются в секции `AdminUser` файла
`appsettings.json` или через переменные окружения `AdminUser__Email`,
`AdminUser__Password`, `AdminUser__FullName`.

| Логин | Пароль | Роль |
| --- | --- | --- |
| `admin@vintage.local` | `Admin123` | `Admin` |

Через страницу регистрации (`/Account/Register`) можно создать пользователя
с ролью `Manager` — он сможет добавлять и редактировать предметы, но не сможет
удалять их физически.

---

## Структура проекта

```
VintagePosuda/
├── src/
│   └── VintagePosuda.Web/          # основное приложение
│       ├── Components/             # Razor-компоненты Blazor
│       │   ├── Layout/             #   MainLayout, NavMenu
│       │   ├── Pages/              #   Catalog, ItemDetailsPage, ItemForm, Reports, Error
│       │   └── Shared/             #   StatusBadge, ConfirmDialog, SellItemDialog, ReportTable
│       ├── Data/                   # EF Core
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/     #   все Fluent API IEntityTypeConfiguration<T>
│       │   ├── Migrations/         #   автогенерируемые EF-миграции
│       │   └── DbInitializer.cs    #   сидинг ролей, админа и справочников
│       ├── Extensions/             # DI extension-методы
│       ├── Models/                 # сущности предметной области + ApplicationUser
│       ├── Pages/                  # Razor Pages для Identity (Login, Register, Layout)
│       ├── Repositories/           # IRepository<T>, IItemRepository
│       ├── Services/               # IItemService, IReportService и т.п.
│       ├── Validators/             # FluentValidation
│       ├── Dockerfile              # multi-stage build
│       ├── appsettings.json
│       └── Program.cs              # composition root: DI, pipeline, миграции
├── tests/
│   └── VintagePosuda.Tests/        # xUnit (36 тестов)
├── docs/
│   └── ПОЯСНИТЕЛЬНАЯ_ЗАПИСКА.md   # документ для сдачи
├── docker-compose.yml
├── .editorconfig                   # стиль кода (StyleCop-совместимый)
├── .dockerignore
├── .gitignore
└── VintagePosuda.sln
```

---

## Базовые операции

| Операция (из ТЗ) | Где реализована |
| --- | --- |
| Добавление нового предмета | `Components/Pages/ItemForm.razor` (`/items/new`) → `IItemService.CreateAsync` |
| Редактирование карточки | `Components/Pages/ItemForm.razor` (`/items/{id}/edit`) → `IItemService.UpdateAsync` |
| Удаление | `Components/Pages/ItemDetailsPage.razor` → кнопка «Удалить» (только для роли Admin) → `IItemService.DeleteAsync` |
| Архивация (продано) | Та же страница: «Отметить проданным» → `IItemService.MarkAsSoldAsync`; «Архивировать» → `IItemService.ArchiveAsync` |
| Поиск по названию, заводу, году | `Components/Pages/Catalog.razor` (главная) → `IItemService.SearchAsync(SearchQuery)` |
| Отчёт «Что в наличии» / «Что продано» | `Components/Pages/Reports.razor` (`/reports`) → `IReportService` |

---

## Команды разработчика

### EF Core миграции

```bash
# Создать новую миграцию (имя — в PascalCase)
dotnet ef migrations add ИмяМиграции --project src/VintagePosuda.Web --output-dir Data/Migrations

# Применить ожидающие миграции
dotnet ef database update --project src/VintagePosuda.Web

# Откатить до конкретной миграции
dotnet ef database update ИмяМиграции --project src/VintagePosuda.Web

# Удалить последнюю миграцию (если ещё не применена)
dotnet ef migrations remove --project src/VintagePosuda.Web
```

### Тесты и покрытие

```bash
# Запустить все тесты
dotnet test

# Запустить тесты с измерением покрытия
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage"

# Файл cobertura coverage.cobertura.xml появится в
# tests/VintagePosuda.Tests/TestResults/<guid>/
```

Текущее покрытие бизнес-логики:
- `ItemValidator` — 100 %
- `ItemRepository` — 100 %
- `ReportService` — 100 %
- `ItemService` — 87.5 %

Итоговое line coverage по `coverlet.runsettings`: 98.03 %.

### Docker

```bash
# Сборка образа
docker compose build

# Запуск (фоном)
docker compose up -d

# Логи
docker compose logs -f app

# Остановка и удаление контейнеров (volume сохраняется)
docker compose down

# Полное удаление, включая БД
docker compose down -v
```

---

## Что куда коммитить (git workflow)

Проект использует упрощённый GitFlow:

- `main` — стабильные релизы;
- `dev` — интеграция новых фич;
- `feature/*` — отдельные фичи (рекомендуется одна ветка на одно изменение).

Типовая последовательность:

```bash
# Начать работу над новой фичей
git checkout dev
git pull
git checkout -b feature/моя-фича

# ...правки...
git add -A
git commit -m "Краткое описание"

# Влить в dev (через PR или локально)
git checkout dev
git merge --no-ff feature/моя-фича

# Релиз: dev → main
git checkout main
git merge --no-ff dev
git tag v1.0
```

## FAQ и частые проблемы

### Не находится команда `dotnet`

Перезапустите терминал после установки SDK. На macOS (Homebrew) `dotnet@8`
является keg-only formula — может потребоваться экспорт переменных:

```bash
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
```

Чтобы это применялось всегда, можно добавить в `~/.zshrc`.

### Ошибка «Specified framework 'Microsoft.NETCore.App', version '8.0.0' was not found»

Установлен только Runtime, но нужен SDK.
Проверьте: `dotnet --list-sdks` — должен быть 8.x.

### `dotnet ef` не найден

```bash
dotnet tool install --global dotnet-ef --version 8.0.10
export PATH="$HOME/.dotnet/tools:$PATH"
```

### Контейнер падает с «unable to open database file»

Контейнер не может писать в `/app/data`. Проверьте, что в `docker-compose.yml`
volume смонтирован с правом на запись. Самый надёжный вариант — named volume
(уже используется по умолчанию).

### Хочу сбросить базу и начать заново

```bash
# локально
rm src/VintagePosuda.Web/app.db

# в Docker
docker compose down -v   # -v удаляет volume с базой
docker compose up --build
```

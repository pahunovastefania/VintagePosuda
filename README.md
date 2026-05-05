# Магазин винтажной посуды

Курсовой проект по дисциплине **«Кроссплатформенная среда исполнения программного обеспечения»**.
Веб-приложение на ASP.NET Core 8 и Blazor Server для интернет-магазина винтажной посуды с отдельной админской частью.

## О проекте

Приложение состоит из двух частей:

- публичный интернет-магазин для покупателей;
- защищённая админ-панель для ведения каталога, продаж и отчётов.

Покупатель видит витрину товаров, может искать предметы, фильтровать каталог, открывать карточку товара и оставлять заявку на покупку. Административные разделы скрыты от обычного пользователя и доступны только после входа.

## Возможности

- публичная витрина магазина на `/`;
- карточки товаров с фотографиями, ценой, категорией, производителем и описанием;
- фильтрация по категории и производителю;
- поиск по названию, описанию, категории, производителю и тегам;
- сортировка по новизне, цене и году выпуска;
- публичная страница товара `/shop/items/{id}`;
- форма заявки на покупку с валидацией;
- админ-каталог `/admin/catalog`;
- добавление и редактирование предметов;
- отметка товара как проданного;
- архивирование и восстановление товара;
- удаление товара только для роли `Admin`;
- отчёты `/admin/reports`: товары в наличии и проданные товары;
- авторизация через ASP.NET Core Identity;
- сидирование тестовых данных и фотографий товаров.

## Стек технологий

| Часть | Технология |
| --- | --- |
| Платформа | .NET 8 |
| Backend | ASP.NET Core 8 |
| UI | Blazor Server |
| ORM | Entity Framework Core 8 |
| База данных | SQLite |
| Авторизация | ASP.NET Core Identity |
| Валидация | FluentValidation, DataAnnotations |
| Тесты | xUnit, FluentAssertions, Microsoft.Data.Sqlite |
| Контейнеризация | Docker, docker-compose |

## Быстрый старт без Docker

```bash
git clone https://github.com/pahunovastefania/VintagePosuda.git
cd VintagePosuda

dotnet restore
dotnet run --project src/VintagePosuda.Web --launch-profile http
```

После запуска приложение доступно по адресу:

```text
http://localhost:5087
```

При первом старте приложение автоматически:

- применяет EF Core миграции;
- создаёт SQLite-базу `app.db`;
- создаёт роли `Admin` и `Manager`;
- создаёт администратора из настроек;
- добавляет категории, материалы, теги, производителей и тестовые товары;
- добавляет фотографии товаров из открытой коллекции The Metropolitan Museum of Art.

## Запуск через Docker

```bash
docker compose up --build
```

После запуска:

```text
http://localhost:8080
```

Проверка состояния:

```bash
curl http://localhost:8080/health
```

Остановка:

```bash
docker compose down
```

Полный сброс контейнера вместе с базой:

```bash
docker compose down -v
```

## Учётные данные администратора

Первый администратор создаётся при первом запуске из секции `AdminUser`.

- **Локальная разработка** — значения берутся из `appsettings.Development.json` (по умолчанию `admin@vintage.local` / `Admin123`).
- **Production / Docker** — задаются переменными окружения `ADMIN_EMAIL`, `ADMIN_PASSWORD`, `ADMIN_FULL_NAME`. См. `.env.example`. Без них контейнер не стартует.

При входе после 5 неудачных попыток подряд аккаунт блокируется на 15 минут (политика Identity).

## Роли и доступ

| Роль | Откуда | Что может |
| --- | --- | --- |
| `Admin` | Сидируется при первом запуске | Всё, включая удаление товаров и создание менеджеров через `/Account/Register` |
| `Manager` | Создаётся администратором через «Создать менеджера» | Управление каталогом, заявками, отчётами; не может удалять товары |
| Аноним | По умолчанию | Витрина, карточки товара, заявка на покупку |

Публичная регистрация закрыта: страница `/Account/Register` доступна только пользователям с ролью `Admin`.

## Основные маршруты

| Маршрут | Назначение | Доступ |
| --- | --- | --- |
| `/` | Публичная витрина магазина | Все пользователи |
| `/shop/items/{id}` | Публичная карточка товара | Все пользователи |
| `/Account/Login` | Вход | Все пользователи |
| `/admin/catalog` | Управление каталогом | Авторизованные пользователи |
| `/items/new` | Добавление товара | Авторизованные пользователи |
| `/items/{id}` | Админская карточка товара | Авторизованные пользователи |
| `/items/{id}/edit` | Редактирование товара | Авторизованные пользователи |
| `/admin/reports` | Отчёты | Авторизованные пользователи |
| `/admin/purchase-requests` | Заявки покупателей | Авторизованные пользователи |
| `/Account/Register` | Создание нового менеджера | Только `Admin` |
| `/health` | Healthcheck | Все пользователи |

## Структура проекта

```text
VintagePosuda/
├── src/
│   └── VintagePosuda.Web/
│       ├── Components/
│       │   ├── Layout/              # MainLayout, NavMenu
│       │   ├── Pages/               # магазин, админ-каталог, товар, отчёты, формы
│       │   └── Shared/              # общие UI-компоненты и модальные окна
│       ├── Data/                    # DbContext, миграции, конфигурации, сиды
│       ├── Models/                  # доменные сущности
│       ├── Pages/                   # Razor Pages для Login/Register
│       ├── Repositories/            # репозитории
│       ├── Services/                # бизнес-логика
│       ├── Validators/              # FluentValidation
│       ├── wwwroot/                 # CSS, логотип, favicon
│       ├── Dockerfile
│       └── Program.cs
├── tests/
│   └── VintagePosuda.Tests/         # автоматические тесты
├── docs/
├── docker-compose.yml
└── VintagePosuda.sln
```

## Модель данных

В проекте используются связи:

- `Manufacturer` → `Item`: один ко многим;
- `Category` → `Item`: один ко многим;
- `Material` → `Item`: один ко многим;
- `Item` → `ItemDetails`: один к одному;
- `Item` → `ItemPhoto`: один ко многим;
- `Item` ↔ `Tag`: многие ко многим через `ItemTag`;
- `ApplicationUser`: пользователь Identity.

## Команды разработчика

Запуск тестов:

```bash
dotnet test VintagePosuda.sln
```

Создание миграции:

```bash
dotnet ef migrations add ИмяМиграции --project src/VintagePosuda.Web --output-dir Data/Migrations
```

Применение миграций:

```bash
dotnet ef database update --project src/VintagePosuda.Web
```

Сборка Docker-образа:

```bash
docker compose build
```

Запуск контейнера в фоне:

```bash
docker compose up -d
```

Логи:

```bash
docker compose logs -f app
```

## Тесты

В проекте есть тесты для:

- валидации карточки товара;
- поиска и загрузки связанных данных в репозитории;
- сервисного слоя товаров;
- отчётов по товарам в наличии и проданным товарам.

Текущая проверка:

```text
42 теста, 0 ошибок
```

## Источники изображений

Фотографии тестовых товаров взяты из открытой коллекции The Metropolitan Museum of Art. Изображения public domain доступны в рамках Open Access / CC0.

Источник: https://www.metmuseum.org/hubs/open-access

## Частые проблемы

### Не находится команда `dotnet`

Установите .NET 8 SDK и перезапустите терминал.

Проверка:

```bash
dotnet --list-sdks
```

### `dotnet ef` не найден

```bash
dotnet tool install --global dotnet-ef --version 8.0.10
export PATH="$HOME/.dotnet/tools:$PATH"
```

### Нужно сбросить локальную базу

```bash
rm src/VintagePosuda.Web/app.db
dotnet run --project src/VintagePosuda.Web --launch-profile http
```

### Нужно сбросить базу в Docker

```bash
docker compose down -v
docker compose up --build
```

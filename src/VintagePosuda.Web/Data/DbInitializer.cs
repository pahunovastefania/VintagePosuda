using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Data;

/// <summary>
/// Initial database seed for roles, the first administrator and demo catalog data.
/// </summary>
public static class DbInitializer
{
    /// <summary>Administrator role name.</summary>
    public const string AdminRole = "Admin";

    /// <summary>Catalog manager role name.</summary>
    public const string ManagerRole = "Manager";

    /// <summary>
    /// Applies all initial seed steps. The method is idempotent and can be called on every app start.
    /// </summary>
    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var adminUser = configuration.GetSection(AdminUserOptions.SectionName).Get<AdminUserOptions>()
            ?? throw new InvalidOperationException("Admin user settings are missing.");

        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager, adminUser);
        await SeedCategoriesAsync(db);
        await SeedMaterialsAsync(db);
        await SeedTagsAsync(db);
        await SeedManufacturersAsync(db);
        await SeedItemsAsync(db);
        await SeedItemPhotosAsync(db);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (string? role in new[] { AdminRole, ManagerRole })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, AdminUserOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Email) || string.IsNullOrWhiteSpace(options.Password))
        {
            throw new InvalidOperationException("Admin email and password must be configured.");
        }

        if (await userManager.FindByEmailAsync(options.Email) is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = options.Email,
            Email = options.Email,
            EmailConfirmed = true,
            FullName = options.FullName,
        };

        var result = await userManager.CreateAsync(admin, options.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, AdminRole);
        }
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext db)
    {
        string[] names = new[]
        {
            "Тарелка",
            "Чашка",
            "Чайная пара",
            "Сервиз",
            "Ваза",
            "Блюдо",
            "Сахарница",
            "Чайник",
            "Молочник",
            "Конфетница",
            "Салатник",
        };

        await AddMissingAsync(
            db,
            db.Categories,
            names,
            x => x.Name,
            name => new Category { Name = name });
    }

    private static async Task SeedMaterialsAsync(ApplicationDbContext db)
    {
        string[] names = new[]
        {
            "Фарфор",
            "Фаянс",
            "Керамика",
            "Стекло",
            "Хрусталь",
            "Майолика",
            "Костяной фарфор",
        };

        await AddMissingAsync(
            db,
            db.Materials,
            names,
            x => x.Name,
            name => new Material { Name = name });
    }

    private static async Task SeedTagsAsync(ApplicationDbContext db)
    {
        string[] names = new[]
        {
            "Винтаж",
            "Антиквариат",
            "Юбилейное",
            "Импорт",
            "СССР",
            "Авторская работа",
            "Ручная роспись",
            "Редкое клеймо",
            "Подарочный набор",
            "Коллекционное",
        };

        await AddMissingAsync(
            db,
            db.Tags,
            names,
            x => x.Name,
            name => new Tag { Name = name });
    }

    private static async Task SeedManufacturersAsync(ApplicationDbContext db)
    {
        var manufacturers = new[]
        {
            new Manufacturer { Name = "Ленинградский фарфоровый завод (ЛФЗ)", Country = "СССР/Россия", FoundedYear = 1744 },
            new Manufacturer { Name = "Дулёвский фарфоровый завод", Country = "Россия", FoundedYear = 1832 },
            new Manufacturer { Name = "Гжельский фарфоровый завод", Country = "Россия", FoundedYear = 1818 },
            new Manufacturer { Name = "Кузнецовский фарфор", Country = "Российская империя", FoundedYear = 1832 },
            new Manufacturer { Name = "Вербилки", Country = "Россия", FoundedYear = 1766 },
            new Manufacturer { Name = "Meissen", Country = "Германия", FoundedYear = 1710 },
            new Manufacturer { Name = "Wedgwood", Country = "Великобритания", FoundedYear = 1759 },
            new Manufacturer { Name = "Royal Copenhagen", Country = "Дания", FoundedYear = 1775 },
            new Manufacturer { Name = "Rosenthal", Country = "Германия", FoundedYear = 1879 },
        };

        var existingNames = await db.Manufacturers
            .Select(x => x.Name)
            .ToListAsync();
        var existing = existingNames.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = manufacturers
            .Where(x => !existing.Contains(x.Name))
            .ToList();

        if (missing.Count > 0)
        {
            db.Manufacturers.AddRange(missing);
            await db.SaveChangesAsync();
        }
    }

    private static async Task SeedItemsAsync(ApplicationDbContext db)
    {
        var manufacturers = await db.Manufacturers.ToDictionaryAsync(x => x.Name);
        var categories = await db.Categories.ToDictionaryAsync(x => x.Name);
        var materials = await db.Materials.ToDictionaryAsync(x => x.Name);
        var tags = await db.Tags.ToDictionaryAsync(x => x.Name);

        await AddItemIfMissingAsync(
            db,
            "Чайная пара \"Кобальтовая сетка\"",
            () => new Item
            {
                Name = "Чайная пара \"Кобальтовая сетка\"",
                Year = 1965,
                Price = 4500m,
                Status = ItemStatus.InStock,
                Description = "Классический рисунок ЛФЗ. В отличном состоянии.",
                ManufacturerId = manufacturers["Ленинградский фарфоровый завод (ЛФЗ)"].Id,
                CategoryId = categories["Чайная пара"].Id,
                MaterialId = materials["Фарфор"].Id,
                Details = new ItemDetails
                {
                    Origin = "Ленинград",
                    Provenance = "Из частной коллекции",
                    Condition = "Excellent",
                },
            },
            tags["СССР"].Id,
            tags["Винтаж"].Id);

        await AddItemIfMissingAsync(
            db,
            "Декоративная тарелка Meissen с цветочным орнаментом",
            () => new Item
            {
                Name = "Декоративная тарелка Meissen с цветочным орнаментом",
                Year = 1890,
                Price = 28000m,
                Status = ItemStatus.InStock,
                Description = "Раритет XIX века, ручная роспись.",
                ManufacturerId = manufacturers["Meissen"].Id,
                CategoryId = categories["Тарелка"].Id,
                MaterialId = materials["Фарфор"].Id,
                Details = new ItemDetails
                {
                    Origin = "Дрезден",
                    Provenance = "Аукцион Sotheby's, 2010",
                    Condition = "Good",
                    Defects = "Едва заметная микротрещина по краю",
                },
            },
            tags["Антиквариат"].Id,
            tags["Ручная роспись"].Id,
            tags["Коллекционное"].Id);

        await AddItemIfMissingAsync(
            db,
            "Ваза Дулёво «Малинка»",
            () => new Item
            {
                Name = "Ваза Дулёво «Малинка»",
                Year = 1970,
                Price = 3200m,
                Status = ItemStatus.Sold,
                SoldAt = DateTime.UtcNow.AddDays(-7),
                Description = "Декоративная ваза 1970-х годов, надглазурная роспись.",
                ManufacturerId = manufacturers["Дулёвский фарфоровый завод"].Id,
                CategoryId = categories["Ваза"].Id,
                MaterialId = materials["Фарфор"].Id,
                Details = new ItemDetails { Condition = "Good" },
            },
            tags["СССР"].Id,
            tags["Винтаж"].Id);

        await AddItemIfMissingAsync(
            db,
            "Сервиз Wedgwood Jasperware на шесть персон",
            () => new Item
            {
                Name = "Сервиз Wedgwood Jasperware на шесть персон",
                Year = 1958,
                Price = 36500m,
                Status = ItemStatus.InStock,
                Description = "Голубой фарфор с белым рельефным декором, комплект без утрат.",
                ManufacturerId = manufacturers["Wedgwood"].Id,
                CategoryId = categories["Сервиз"].Id,
                MaterialId = materials["Костяной фарфор"].Id,
                Details = new ItemDetails
                {
                    Origin = "Стаффордшир",
                    Provenance = "Семейное собрание, привезён в 1990-е годы.",
                    Condition = "Excellent",
                },
            },
            tags["Импорт"].Id,
            tags["Подарочный набор"].Id);

        await AddItemIfMissingAsync(
            db,
            "Сахарница Вербилки с золотой отводкой",
            () => new Item
            {
                Name = "Сахарница Вербилки с золотой отводкой",
                Year = 1948,
                Price = 7200m,
                Status = ItemStatus.InStock,
                Description = "Фарфоровая сахарница с мягким растительным орнаментом.",
                ManufacturerId = manufacturers["Вербилки"].Id,
                CategoryId = categories["Сахарница"].Id,
                MaterialId = materials["Фарфор"].Id,
                Details = new ItemDetails
                {
                    Origin = "Московская область",
                    Condition = "Good",
                    Defects = "Небольшое потемнение позолоты на ручке крышки",
                },
            },
            tags["СССР"].Id,
            tags["Редкое клеймо"].Id);

        await AddItemIfMissingAsync(
            db,
            "Хрустальная конфетница с гранёным бортом",
            () => new Item
            {
                Name = "Хрустальная конфетница с гранёным бортом",
                Year = 1975,
                Price = 2900m,
                Status = ItemStatus.InStock,
                Description = "Тяжёлый хрусталь, глубокая огранка, чистый звон.",
                ManufacturerId = manufacturers["Гжельский фарфоровый завод"].Id,
                CategoryId = categories["Конфетница"].Id,
                MaterialId = materials["Хрусталь"].Id,
                Details = new ItemDetails
                {
                    Origin = "Подмосковье",
                    Condition = "Excellent",
                },
            },
            tags["Винтаж"].Id,
            tags["СССР"].Id);

        await AddItemIfMissingAsync(
            db,
            "Тарелка Кузнецов с кобальтовым бортом",
            () => new Item
            {
                Name = "Тарелка Кузнецов с кобальтовым бортом",
                Year = 1912,
                Price = 18500m,
                Status = ItemStatus.Sold,
                SoldAt = DateTime.UtcNow.AddDays(-21),
                Description = "Дореволюционная тарелка с читаемым клеймом и тонкой золотой линией.",
                ManufacturerId = manufacturers["Кузнецовский фарфор"].Id,
                CategoryId = categories["Тарелка"].Id,
                MaterialId = materials["Фаянс"].Id,
                Details = new ItemDetails
                {
                    Origin = "Тверская губерния",
                    Provenance = "Приобретена у частного коллекционера.",
                    Condition = "Good",
                    Defects = "Мелкая паутинка глазури",
                },
            },
            tags["Антиквариат"].Id,
            tags["Редкое клеймо"].Id);

        await AddItemIfMissingAsync(
            db,
            "Чайник Royal Copenhagen Blue Fluted",
            () => new Item
            {
                Name = "Чайник Royal Copenhagen Blue Fluted",
                Year = 1962,
                Price = 23800m,
                Status = ItemStatus.InStock,
                Description = "Классический сине-белый узор, объём около 900 мл.",
                ManufacturerId = manufacturers["Royal Copenhagen"].Id,
                CategoryId = categories["Чайник"].Id,
                MaterialId = materials["Фарфор"].Id,
                Details = new ItemDetails
                {
                    Origin = "Копенгаген",
                    Condition = "Excellent",
                },
            },
            tags["Импорт"].Id,
            tags["Коллекционное"].Id);

        await AddItemIfMissingAsync(
            db,
            "Салатник Rosenthal с платиновым кантом",
            () => new Item
            {
                Name = "Салатник Rosenthal с платиновым кантом",
                Year = 1984,
                Price = 6400m,
                Status = ItemStatus.Archived,
                Description = "Минималистичная форма, тонкий платиновый кант по борту.",
                ManufacturerId = manufacturers["Rosenthal"].Id,
                CategoryId = categories["Салатник"].Id,
                MaterialId = materials["Фарфор"].Id,
                Details = new ItemDetails
                {
                    Origin = "Германия",
                    Condition = "Fair",
                    Defects = "Есть следы хранения на внутренней поверхности",
                },
            },
            tags["Импорт"].Id,
            tags["Винтаж"].Id);
    }

    private static async Task SeedItemPhotosAsync(ApplicationDbContext db)
    {
        var photos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Чайная пара \"Кобальтовая сетка\""] = "https://images.metmuseum.org/CRDImages/ad/web-large/DP223040.jpg",
            ["Декоративная тарелка Meissen с цветочным орнаментом"] = "https://images.metmuseum.org/CRDImages/ad/web-large/DP261201.jpg",
            ["Ваза Дулёво «Малинка»"] = "https://images.metmuseum.org/CRDImages/as/web-large/DP246426.jpg",
            ["Сервиз Wedgwood Jasperware на шесть персон"] = "https://images.metmuseum.org/CRDImages/ad/web-large/125563.jpg",
            ["Сахарница Вербилки с золотой отводкой"] = "https://images.metmuseum.org/CRDImages/ad/web-large/ADA5346.jpg",
            ["Хрустальная конфетница с гранёным бортом"] = "https://images.metmuseum.org/CRDImages/gr/web-large/DP105810.jpg",
            ["Тарелка Кузнецов с кобальтовым бортом"] = "https://images.metmuseum.org/CRDImages/cl/web-large/DP248473.jpg",
            ["Чайник Royal Copenhagen Blue Fluted"] = "https://images.metmuseum.org/CRDImages/ad/web-large/DP-15482-001.jpg",
            ["Салатник Rosenthal с платиновым кантом"] = "https://images.metmuseum.org/CRDImages/es/web-large/36071.jpg",
        };

        var itemIds = await db.Items
            .Where(item => photos.Keys.Contains(item.Name))
            .Select(item => new { item.Id, item.Name })
            .ToListAsync();

        foreach (var item in itemIds)
        {
            string url = photos[item.Name];
            var existingPhotos = await db.ItemPhotos
                .Where(photo => photo.ItemId == item.Id)
                .ToListAsync();
            var primaryPhoto = existingPhotos.FirstOrDefault(photo => photo.Url == url);

            foreach (var photo in existingPhotos.Where(photo => photo.Url != url))
            {
                photo.IsPrimary = false;
            }

            if (primaryPhoto is null)
            {
                db.ItemPhotos.Add(new ItemPhoto
                {
                    ItemId = item.Id,
                    Url = url,
                    IsPrimary = true,
                });
            }
            else
            {
                primaryPhoto.IsPrimary = true;
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task AddMissingAsync<T>(
        ApplicationDbContext db,
        DbSet<T> set,
        IEnumerable<string> names,
        Func<T, string> getName,
        Func<string, T> create)
        where T : class
    {
        var existingItems = await set.AsNoTracking().ToListAsync();
        var existingNames = existingItems
            .Select(getName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = names
            .Where(name => !existingNames.Contains(name))
            .Select(create)
            .ToList();

        if (missing.Count > 0)
        {
            set.AddRange(missing);
            await db.SaveChangesAsync();
        }
    }

    private static async Task AddItemIfMissingAsync(
        ApplicationDbContext db,
        string name,
        Func<Item> create,
        params int[] tagIds)
    {
        if (await db.Items.AnyAsync(x => x.Name == name))
        {
            return;
        }

        var item = create();
        foreach (int tagId in tagIds.Distinct())
        {
            item.ItemTags.Add(new ItemTag { TagId = tagId });
        }

        db.Items.Add(item);
        await db.SaveChangesAsync();
    }
}

/// <summary>
/// Configuration section for the first administrator account.
/// </summary>
public sealed class AdminUserOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "AdminUser";

    /// <summary>Administrator email and login.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Administrator password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Display name shown in the application.</summary>
    public string FullName { get; set; } = "Администратор";
}

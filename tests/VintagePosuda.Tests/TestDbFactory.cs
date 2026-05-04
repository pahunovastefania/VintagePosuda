namespace VintagePosuda.Tests;
internal static class TestDbFactory
{
    public static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        return connection;
    }
    public static ApplicationDbContext CreateDb(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new ApplicationDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }
    public static void Seed(ApplicationDbContext db)
    {
        var lfz = new Manufacturer { Name = "ЛФЗ", Country = "СССР", FoundedYear = 1744 };
        var meissen = new Manufacturer { Name = "Meissen", Country = "Германия", FoundedYear = 1710 };

        var teaPair = new Category { Name = "Чайная пара" };
        var plate = new Category { Name = "Тарелка" };

        var porcelain = new Material { Name = "Фарфор" };

        var ussr = new Tag { Name = "СССР" };
        var antique = new Tag { Name = "Антиквариат" };

        db.AddRange(lfz, meissen, teaPair, plate, porcelain, ussr, antique);
        db.SaveChanges();

        var item1 = new Item
        {
            Name = "Чайная пара Кобальт",
            Year = 1965,
            Price = 4500m,
            Status = ItemStatus.InStock,
            ManufacturerId = lfz.Id,
            CategoryId = teaPair.Id,
            MaterialId = porcelain.Id,
            Details = new ItemDetails { Condition = "Excellent", Origin = "Ленинград" },
        };
        item1.ItemTags.Add(new ItemTag { TagId = ussr.Id });
        item1.Photos.Add(new ItemPhoto
        {
            Url = "https://example.test/tea-pair.jpg",
            IsPrimary = true,
        });

        var item2 = new Item
        {
            Name = "Тарелка Meissen",
            Year = 1890,
            Price = 28000m,
            Status = ItemStatus.InStock,
            ManufacturerId = meissen.Id,
            CategoryId = plate.Id,
            MaterialId = porcelain.Id,
            Details = new ItemDetails { Condition = "Good" },
        };
        item2.ItemTags.Add(new ItemTag { TagId = antique.Id });
        item2.Photos.Add(new ItemPhoto
        {
            Url = "https://example.test/plate.jpg",
            IsPrimary = true,
        });

        var item3 = new Item
        {
            Name = "Тарелка ЛФЗ юбилейная",
            Year = 1980,
            Price = 5500m,
            Status = ItemStatus.Sold,
            SoldAt = DateTime.UtcNow.AddDays(-3),
            ManufacturerId = lfz.Id,
            CategoryId = plate.Id,
            MaterialId = porcelain.Id,
            Details = new ItemDetails { Condition = "Good" },
        };

        db.Items.AddRange(item1, item2, item3);
        db.SaveChanges();
    }
}

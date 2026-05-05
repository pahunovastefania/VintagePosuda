namespace VintagePosuda.Tests.Services;
public class ItemServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _db;
    private readonly ItemService _sut;

    public ItemServiceTests()
    {
        _connection = TestDbFactory.CreateOpenConnection();
        _db = TestDbFactory.CreateDb(_connection);
        TestDbFactory.Seed(_db);
        _sut = new ItemService(new ItemRepository(_db), _db, new ItemValidator());
    }

    [Fact]
    public async Task CreateAsync_AddsItem_AndReturnsId()
    {
        var manufacturer = await _db.Manufacturers.FirstAsync();
        var category = await _db.Categories.FirstAsync();
        var material = await _db.Materials.FirstAsync();

        var item = new Item
        {
            Name = "Новый предмет",
            Year = 2000,
            Price = 1000m,
            Status = ItemStatus.InStock,
            ManufacturerId = manufacturer.Id,
            CategoryId = category.Id,
            MaterialId = material.Id,
            Details = new ItemDetails { Condition = "Good" },
        };

        int id = await _sut.CreateAsync(item);

        id.Should().BeGreaterThan(0);
        var saved = await _db.Items.FindAsync(id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Новый предмет");
        saved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateAsync_WithTagIds_AssignsTags()
    {
        var manufacturer = await _db.Manufacturers.FirstAsync();
        var category = await _db.Categories.FirstAsync();
        var material = await _db.Materials.FirstAsync();
        var tags = await _db.Tags.Take(2).ToListAsync();

        var item = new Item
        {
            Name = "С тегами",
            Price = 100m,
            ManufacturerId = manufacturer.Id,
            CategoryId = category.Id,
            MaterialId = material.Id,
            Details = new ItemDetails { Condition = "Good" },
        };

        int id = await _sut.CreateAsync(item, tags.Select(t => t.Id));

        var assigned = await _db.ItemTags.Where(x => x.ItemId == id).ToListAsync();
        assigned.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_WithPhoto_SavesPrimaryPhoto()
    {
        var manufacturer = await _db.Manufacturers.FirstAsync();
        var category = await _db.Categories.FirstAsync();
        var material = await _db.Materials.FirstAsync();

        var item = new Item
        {
            Name = "Предмет с фото",
            Price = 100m,
            ManufacturerId = manufacturer.Id,
            CategoryId = category.Id,
            MaterialId = material.Id,
            Details = new ItemDetails { Condition = "Good" },
        };
        item.Photos.Add(new ItemPhoto
        {
            Url = "https://example.test/new-photo.jpg",
            IsPrimary = true,
        });

        int id = await _sut.CreateAsync(item);

        var photo = await _db.ItemPhotos.AsNoTracking().SingleAsync(x => x.ItemId == id);
        photo.Url.Should().Be("https://example.test/new-photo.jpg");
        photo.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_ChangesEditableFields_AndDetails()
    {
        var existing = await _db.Items
            .Include(x => x.Details)
            .FirstAsync(x => x.Name == "Чайная пара Кобальт");
        var initialCreatedAt = existing.CreatedAt;

        existing.Name = "Чайная пара Кобальт (отреставрировано)";
        existing.Price = 5000m;
        existing.Details!.Defects = "Незаметная царапина";

        await _sut.UpdateAsync(existing);

        var fresh = await _db.Items.AsNoTracking()
            .Include(x => x.Details)
            .FirstAsync(x => x.Id == existing.Id);

        fresh.Name.Should().EndWith("(отреставрировано)");
        fresh.Price.Should().Be(5000m);
        fresh.Details!.Defects.Should().Be("Незаметная царапина");
        fresh.CreatedAt.Should().BeCloseTo(initialCreatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_ReplacesPrimaryPhoto()
    {
        var existing = await _db.Items
            .Include(x => x.Photos)
            .FirstAsync(x => x.Name == "Чайная пара Кобальт");

        existing.Photos.Clear();
        existing.Photos.Add(new ItemPhoto
        {
            Url = "https://example.test/updated-photo.jpg",
            IsPrimary = true,
        });

        await _sut.UpdateAsync(existing);

        var photos = await _db.ItemPhotos.AsNoTracking()
            .Where(x => x.ItemId == existing.Id)
            .ToListAsync();
        photos.Should().ContainSingle();
        photos.Single().Url.Should().Be("https://example.test/updated-photo.jpg");
        photos.Single().IsPrimary.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_UnknownItem_Throws()
    {
        var manufacturer = await _db.Manufacturers.FirstAsync();
        var category = await _db.Categories.FirstAsync();
        var material = await _db.Materials.FirstAsync();

        var ghost = new Item
        {
            Id = 99999,
            Name = "Призрак",
            Price = 100m,
            ManufacturerId = manufacturer.Id,
            CategoryId = category.Id,
            MaterialId = material.Id,
            Details = new ItemDetails { Condition = "Good" },
        };

        var act = async () => await _sut.UpdateAsync(ghost);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CreateAsync_WithInvalidItem_ThrowsValidationException()
    {
        var invalid = new Item
        {
            Name = "",
            Price = 0m,
            ManufacturerId = 0,
            CategoryId = 0,
            MaterialId = 0,
        };

        var act = async () => await _sut.CreateAsync(invalid);
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact]
    public async Task UpdateAsync_StaleRowVersion_ThrowsConcurrencyException()
    {
        var manufacturer = await _db.Manufacturers.FirstAsync();
        var category = await _db.Categories.FirstAsync();
        var material = await _db.Materials.FirstAsync();

        var freshItem = new Item
        {
            Name = "Конкурентный предмет",
            Price = 500m,
            ManufacturerId = manufacturer.Id,
            CategoryId = category.Id,
            MaterialId = material.Id,
            Details = new ItemDetails { Condition = "Good" },
        };
        int id = await _sut.CreateAsync(freshItem);

        using var connection2 = TestDbFactory.CreateOpenConnection();
        var staleSnapshot = new Item
        {
            Id = id,
            Name = "Конкурентный предмет (старая версия)",
            Price = 500m,
            ManufacturerId = manufacturer.Id,
            CategoryId = category.Id,
            MaterialId = material.Id,
            RowVersion = Guid.NewGuid(),
            Details = new ItemDetails { Condition = "Good" },
        };

        var act = async () => await _sut.UpdateAsync(staleSnapshot);
        var assertion = await act.Should().ThrowAsync<InvalidOperationException>();
        assertion.Which.Message.Should().Contain("изменён другим пользователем");
    }

    [Fact]
    public async Task MarkAsSoldAsync_SetsStatusAndDate()
    {
        var item = await _db.Items.FirstAsync(x => x.Name == "Чайная пара Кобальт");
        var soldAt = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

        bool ok = await _sut.MarkAsSoldAsync(item.Id, soldAt);

        ok.Should().BeTrue();
        var fresh = await _db.Items.AsNoTracking().FirstAsync(x => x.Id == item.Id);
        fresh.Status.Should().Be(ItemStatus.Sold);
        fresh.SoldAt.Should().Be(soldAt);
    }

    [Fact]
    public async Task MarkAsSoldAsync_UnknownId_ReturnsFalse()
    {
        bool ok = await _sut.MarkAsSoldAsync(99999);
        ok.Should().BeFalse();
    }

    [Fact]
    public async Task ArchiveAsync_SetsStatusToArchived()
    {
        var item = await _db.Items.FirstAsync(x => x.Status == ItemStatus.InStock);

        bool ok = await _sut.ArchiveAsync(item.Id);

        ok.Should().BeTrue();
        var fresh = await _db.Items.AsNoTracking().FirstAsync(x => x.Id == item.Id);
        fresh.Status.Should().Be(ItemStatus.Archived);
    }

    [Fact]
    public async Task RestoreAsync_BringsItemBackInStock_AndClearsSoldAt()
    {
        var item = await _db.Items.FirstAsync(x => x.Status == ItemStatus.Sold);

        bool ok = await _sut.RestoreAsync(item.Id);

        ok.Should().BeTrue();
        var fresh = await _db.Items.AsNoTracking().FirstAsync(x => x.Id == item.Id);
        fresh.Status.Should().Be(ItemStatus.InStock);
        fresh.SoldAt.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem()
    {
        var item = await _db.Items.FirstAsync();
        bool ok = await _sut.DeleteAsync(item.Id);

        ok.Should().BeTrue();
        (await _db.Items.FindAsync(item.Id)).Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_DelegatesToRepository()
    {
        var result = await _sut.SearchAsync(new SearchQuery { NameContains = "Meissen" });
        result.Should().ContainSingle().Which.Manufacturer!.Name.Should().Be("Meissen");
    }
    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

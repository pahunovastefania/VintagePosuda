namespace VintagePosuda.Tests.Repositories;
public class ItemRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _db;
    private readonly ItemRepository _sut;
    public ItemRepositoryTests()
    {
        _connection = TestDbFactory.CreateOpenConnection();
        _db = TestDbFactory.CreateDb(_connection);
        TestDbFactory.Seed(_db);
        _sut = new ItemRepository(_db);
    }

    [Fact]
    public async Task SearchAsync_WithoutFilters_ReturnsAllItems()
    {
        var result = await _sut.SearchAsync(new SearchQuery());
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task SearchAsync_ByNameContains_ReturnsMatching()
    {
        var result = await _sut.SearchAsync(new SearchQuery { NameContains = "Кобальт" });
        result.Should().ContainSingle().Which.Name.Should().Be("Чайная пара Кобальт");
    }

    [Fact]
    public async Task SearchAsync_ByManufacturerContains_FiltersByName()
    {
        var result = await _sut.SearchAsync(new SearchQuery { ManufacturerContains = "ЛФЗ" });
        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.Manufacturer!.Name == "ЛФЗ");
    }

    [Fact]
    public async Task SearchAsync_ByYear_ReturnsExactMatch()
    {
        var result = await _sut.SearchAsync(new SearchQuery { Year = 1965 });
        result.Should().ContainSingle().Which.Year.Should().Be(1965);
    }

    [Fact]
    public async Task SearchAsync_ByStatus_FiltersBySold()
    {
        var result = await _sut.SearchAsync(new SearchQuery { Status = ItemStatus.Sold });
        result.Should().ContainSingle().Which.Status.Should().Be(ItemStatus.Sold);
    }

    [Fact]
    public async Task SearchAsync_CombinedFilters_AppliesAll()
    {
        var result = await _sut.SearchAsync(new SearchQuery
        {
            ManufacturerContains = "ЛФЗ",
            Status = ItemStatus.InStock,
        });
        result.Should().ContainSingle().Which.Name.Should().Be("Чайная пара Кобальт");
    }

    [Fact]
    public async Task GetWithDetailsAsync_LoadsManufacturerCategoryDetailsTags()
    {
        var item = await _db.Items.FirstAsync(x => x.Name == "Чайная пара Кобальт");
        var loaded = await _sut.GetWithDetailsAsync(item.Id);

        loaded.Should().NotBeNull();
        loaded!.Manufacturer.Should().NotBeNull();
        loaded.Category.Should().NotBeNull();
        loaded.Material.Should().NotBeNull();
        loaded.Details.Should().NotBeNull();
        loaded.Details!.Origin.Should().Be("Ленинград");
        loaded.ItemTags.Should().HaveCount(1);
        loaded.ItemTags.First().Tag.Should().NotBeNull();
        loaded.ItemTags.First().Tag!.Name.Should().Be("СССР");
    }

    [Fact]
    public async Task GetWithDetailsAsync_NotFound_ReturnsNull()
    {
        var loaded = await _sut.GetWithDetailsAsync(99999);
        loaded.Should().BeNull();
    }

    [Fact]
    public async Task GetByStatusAsync_FiltersByStatus()
    {
        var sold = await _sut.GetByStatusAsync(ItemStatus.Sold);
        sold.Should().ContainSingle();

        var inStock = await _sut.GetByStatusAsync(ItemStatus.InStock);
        inStock.Should().HaveCount(2);
    }

    [Fact]
    public async Task SetTagsAsync_ReplacesExistingTags()
    {
        var item = await _db.Items.FirstAsync(x => x.Name == "Чайная пара Кобальт");
        var antique = await _db.Tags.FirstAsync(x => x.Name == "Антиквариат");

        await _sut.SetTagsAsync(item.Id, new[] { antique.Id });

        var tags = await _db.ItemTags.Where(x => x.ItemId == item.Id).ToListAsync();
        tags.Should().ContainSingle().Which.TagId.Should().Be(antique.Id);
    }

    [Fact]
    public async Task DeleteAsync_RemovesItem_AndCascadesDetails()
    {
        var item = await _db.Items.FirstAsync(x => x.Name == "Чайная пара Кобальт");

        bool deleted = await _sut.DeleteAsync(item.Id);

        deleted.Should().BeTrue();
        (await _db.Items.FindAsync(item.Id)).Should().BeNull();
        (await _db.ItemDetails.FindAsync(item.Id)).Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_ReturnsFalse()
    {
        bool deleted = await _sut.DeleteAsync(99999);
        deleted.Should().BeFalse();
    }
    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

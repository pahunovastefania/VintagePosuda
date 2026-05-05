namespace VintagePosuda.Tests.Services;

public class LookupServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _db;

    public LookupServiceTests()
    {
        _connection = TestDbFactory.CreateOpenConnection();
        _db = TestDbFactory.CreateDb(_connection);
        TestDbFactory.Seed(_db);
    }

    [Fact]
    public async Task CategoryService_GetAll_ReturnsSortedByName()
    {
        var sut = new CategoryService(_db);

        var categories = await sut.GetAllAsync();

        categories.Should().HaveCountGreaterThan(0);
        categories.Should().BeInAscendingOrder(x => x.Name);
    }

    [Fact]
    public async Task MaterialService_Create_ReturnsId_AndPersists()
    {
        var sut = new MaterialService(_db);
        var material = new Material { Name = "Хрусталь" };

        int id = await sut.CreateAsync(material);

        id.Should().BeGreaterThan(0);
        var saved = await _db.Materials.FindAsync(id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Хрусталь");
    }

    [Fact]
    public async Task ManufacturerService_Update_ChangesName()
    {
        var sut = new ManufacturerService(_db);
        var existing = await _db.Manufacturers.FirstAsync();
        existing.Name = "Обновлённый завод";

        await sut.UpdateAsync(existing);

        var fresh = await _db.Manufacturers.AsNoTracking().FirstAsync(x => x.Id == existing.Id);
        fresh.Name.Should().Be("Обновлённый завод");
    }

    [Fact]
    public async Task TagService_Delete_RemovesEntity()
    {
        var sut = new TagService(_db);
        var tag = new Tag { Name = "ВременныйТег" };
        await _db.Tags.AddAsync(tag);
        await _db.SaveChangesAsync();

        bool deleted = await sut.DeleteAsync(tag.Id);

        deleted.Should().BeTrue();
        (await _db.Tags.FindAsync(tag.Id)).Should().BeNull();
    }

    [Fact]
    public async Task CategoryService_GetById_UnknownId_ReturnsNull()
    {
        var sut = new CategoryService(_db);

        var found = await sut.GetByIdAsync(99999);

        found.Should().BeNull();
    }

    [Fact]
    public async Task TagService_Delete_UnknownId_ReturnsFalse()
    {
        var sut = new TagService(_db);

        bool deleted = await sut.DeleteAsync(99999);

        deleted.Should().BeFalse();
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

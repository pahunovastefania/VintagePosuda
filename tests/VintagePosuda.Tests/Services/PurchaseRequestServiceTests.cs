namespace VintagePosuda.Tests.Services;

public class PurchaseRequestServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ApplicationDbContext _db;
    private readonly PurchaseRequestService _sut;

    public PurchaseRequestServiceTests()
    {
        _connection = TestDbFactory.CreateOpenConnection();
        _db = TestDbFactory.CreateDb(_connection);
        TestDbFactory.Seed(_db);
        _sut = new PurchaseRequestService(_db);
    }

    [Fact]
    public async Task CreateAsync_PersistsRequest_AndTrimsFields()
    {
        var item = await _db.Items.FirstAsync();
        var request = new PurchaseRequest
        {
            ItemId = item.Id,
            BuyerName = "  Иван Петров  ",
            BuyerPhone = "  +79991112233  ",
            BuyerEmail = "  ivan@example.test  ",
            Comment = "  Хочу купить  ",
        };

        int id = await _sut.CreateAsync(request);

        var saved = await _db.PurchaseRequests.AsNoTracking().FirstAsync(x => x.Id == id);
        saved.BuyerName.Should().Be("Иван Петров");
        saved.BuyerPhone.Should().Be("+79991112233");
        saved.BuyerEmail.Should().Be("ivan@example.test");
        saved.Comment.Should().Be("Хочу купить");
        saved.Status.Should().Be(PurchaseRequestStatus.New);
        saved.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CreateAsync_NullablesEmptyStringsToNull()
    {
        var item = await _db.Items.FirstAsync();
        var request = new PurchaseRequest
        {
            ItemId = item.Id,
            BuyerName = "Аноним",
            BuyerPhone = "+70000000000",
            BuyerEmail = "   ",
            Comment = "",
        };

        int id = await _sut.CreateAsync(request);

        var saved = await _db.PurchaseRequests.AsNoTracking().FirstAsync(x => x.Id == id);
        saved.BuyerEmail.Should().BeNull();
        saved.Comment.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_OrdersByCreatedDesc_AndIncludesItem()
    {
        var item = await _db.Items.FirstAsync();
        await _sut.CreateAsync(new PurchaseRequest { ItemId = item.Id, BuyerName = "Первый", BuyerPhone = "+71111111111" });
        await Task.Delay(10);
        await _sut.CreateAsync(new PurchaseRequest { ItemId = item.Id, BuyerName = "Второй", BuyerPhone = "+72222222222" });

        var all = await _sut.GetAllAsync();

        all.Should().HaveCount(2);
        all[0].BuyerName.Should().Be("Второй");
        all[0].Item.Should().NotBeNull();
        all[0].Item!.Name.Should().Be(item.Name);
    }

    [Fact]
    public async Task SetStatusAsync_UpdatesStatus()
    {
        var item = await _db.Items.FirstAsync();
        int id = await _sut.CreateAsync(new PurchaseRequest
        {
            ItemId = item.Id,
            BuyerName = "Покупатель",
            BuyerPhone = "+70000000000",
        });

        bool ok = await _sut.SetStatusAsync(id, PurchaseRequestStatus.Contacted);

        ok.Should().BeTrue();
        var fresh = await _db.PurchaseRequests.AsNoTracking().FirstAsync(x => x.Id == id);
        fresh.Status.Should().Be(PurchaseRequestStatus.Contacted);
    }

    [Fact]
    public async Task SetStatusAsync_UnknownId_ReturnsFalse()
    {
        bool ok = await _sut.SetStatusAsync(99999, PurchaseRequestStatus.Closed);

        ok.Should().BeFalse();
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

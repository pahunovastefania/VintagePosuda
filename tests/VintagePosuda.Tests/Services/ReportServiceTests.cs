namespace VintagePosuda.Tests.Services;
public class ReportServiceTests
{
    [Fact]
    public async Task GetInStockReportAsync_AggregatesPrices()
    {
        var items = new[]
        {
            new Item { Id = 1, Name = "A", Price = 100m, Status = ItemStatus.InStock },
            new Item { Id = 2, Name = "B", Price = 250m, Status = ItemStatus.InStock },
        };
        var repoMock = new Mock<IItemRepository>();
        repoMock
            .Setup(r => r.GetByStatusAsync(ItemStatus.InStock, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        var sut = new ReportService(repoMock.Object);

        var report = await sut.GetInStockReportAsync();

        report.Items.Should().HaveCount(2);
        report.TotalCount.Should().Be(2);
        report.TotalPrice.Should().Be(350m);
    }

    [Fact]
    public async Task GetSoldReportAsync_DelegatesToRepositoryWithSoldStatus()
    {
        var repoMock = new Mock<IItemRepository>();
        repoMock
            .Setup(r => r.GetByStatusAsync(ItemStatus.Sold, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Item>());

        var sut = new ReportService(repoMock.Object);

        var report = await sut.GetSoldReportAsync();

        report.Items.Should().BeEmpty();
        report.TotalCount.Should().Be(0);
        report.TotalPrice.Should().Be(0m);
        repoMock.Verify(r => r.GetByStatusAsync(ItemStatus.Sold, It.IsAny<CancellationToken>()), Times.Once);
    }
}

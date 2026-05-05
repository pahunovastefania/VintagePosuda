using VintagePosuda.Web.Repositories;
using VintagePosuda.Web.Services;

namespace VintagePosuda.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IItemRepository, ItemRepository>();

        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IManufacturerService, ManufacturerService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IMaterialService, MaterialService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IPurchaseRequestService, PurchaseRequestService>();

        return services;
    }
}

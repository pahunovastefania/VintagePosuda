using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

public class ManufacturerService : LookupService<Manufacturer>, IManufacturerService
{
    public ManufacturerService(ApplicationDbContext db) : base(db)
    {
    }
}

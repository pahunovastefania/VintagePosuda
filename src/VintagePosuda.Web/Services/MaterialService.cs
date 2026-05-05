using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

public class MaterialService : LookupService<Material>, IMaterialService
{
    public MaterialService(ApplicationDbContext db) : base(db)
    {
    }
}

using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

public class CategoryService : LookupService<Category>, ICategoryService
{
    public CategoryService(ApplicationDbContext db) : base(db)
    {
    }
}

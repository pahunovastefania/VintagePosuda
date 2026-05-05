using VintagePosuda.Web.Data;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Services;

public class TagService : LookupService<Tag>, ITagService
{
    public TagService(ApplicationDbContext db) : base(db)
    {
    }
}

using Microsoft.AspNetCore.Identity;

namespace VintagePosuda.Web.Models;
public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Pages.Account;
[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [BindProperty]
    public InputModel Input { get; set; } = new();
    public IList<string> Errors { get; set; } = new List<string>();
    public class InputModel
    {
        [Required(ErrorMessage = "Введите ФИО.")]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Введите email.")]
        [EmailAddress(ErrorMessage = "Некорректный email.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Введите пароль.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть не короче 6 символов.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Подтвердите пароль.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    public void OnGet()
    {
    }
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            FullName = Input.FullName,
            EmailConfirmed = true,
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                Errors.Add(error.Description);
            }
            return Page();
        }

        await _userManager.AddToRoleAsync(user, "Manager");
        await _signInManager.SignInAsync(user, isPersistent: false);
        return LocalRedirect("/");
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Models;
using System.ComponentModel.DataAnnotations;

namespace NarrowCasting_V5.Pages.Admin.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signIn, ILogger<LoginModel> logger)
        {
            _signIn = signIn;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vul een e-mailadres in")]
            [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vul een wachtwoord in")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            public bool RememberMe { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid) return Page();

            var result = await _signIn.PasswordSignInAsync(
                Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in.", Input.Email);
                return LocalRedirect(returnUrl ?? "/Admin/Dashboard");
            }

            if (result.IsLockedOut)
            {
                ErrorMessage = "Account is tijdelijk geblokkeerd. Probeer het later opnieuw.";
            }
            else
            {
                ErrorMessage = "Ongeldige inloggegevens. Controleer uw e-mailadres en wachtwoord.";
            }

            return Page();
        }
    }
}

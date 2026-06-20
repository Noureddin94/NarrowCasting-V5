using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Admin.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signIn;

        public LogoutModel(SignInManager<ApplicationUser> signIn) => _signIn = signIn;

        public async Task<IActionResult> OnPostAsync()
        {
            await _signIn.SignOutAsync();
            return RedirectToPage("/Admin/Account/Login");
        }
    }
}

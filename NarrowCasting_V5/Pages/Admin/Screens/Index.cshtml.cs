using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Admin.Screens
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IScreenService _screens;
        private readonly IDepartmentService _departments;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(IScreenService screens, IDepartmentService departments,
                          UserManager<ApplicationUser> userManager)
        {
            _screens = screens;
            _departments = departments;
            _userManager = userManager;
        }

        public IEnumerable<Screen> Screens { get; set; } = [];
        public IEnumerable<Department> Departments { get; set; } = [];

        public async Task OnGetAsync()
        {
            Departments = await _departments.GetAllAsync();

            // AVG: Employee only sees their own department
            if (User.IsInRole("Admin"))
            {
                Screens = await _screens.GetAllAsync();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                Screens = user?.DepartmentId.HasValue == true
                    ? await _screens.GetByDepartmentAsync(user.DepartmentId.Value)
                    : [];
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            await _screens.DeleteAsync(id, userId);
            TempData["Success"] = "Scherm succesvol verwijderd.";
            return RedirectToPage();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Admin.Departments
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IDepartmentService _departments;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(IDepartmentService departments, UserManager<ApplicationUser> userManager)
        {
            _departments = departments;
            _userManager = userManager;
        }

        public IEnumerable<Department> Departments { get; set; } = [];

        public async Task OnGetAsync() =>
            Departments = await _departments.GetAllAsync();

        public async Task<IActionResult> OnPostSaveAsync(Department dept)
        {
            if (!ModelState.IsValid) return Page();
            var userId = _userManager.GetUserId(User)!;

            if (dept.Id == 0)
                await _departments.CreateAsync(dept, userId);
            else
                await _departments.UpdateAsync(dept, userId);

            TempData["Success"] = "Afdeling opgeslagen.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            await _departments.DeleteAsync(id, userId);
            TempData["Success"] = "Afdeling verwijderd.";
            return RedirectToPage();
        }
    }
}

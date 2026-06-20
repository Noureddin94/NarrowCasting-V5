using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Admin.Screens
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IScreenService _screens;
        private readonly IDepartmentService _departments;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditModel(IScreenService screens, IDepartmentService departments,
                         UserManager<ApplicationUser> userManager)
        {
            _screens = screens;
            _departments = departments;
            _userManager = userManager;
        }

        [BindProperty]
        public Screen Screen { get; set; } = new();

        public SelectList DepartmentSelectList { get; set; } = null!;
        public bool IsNew => Screen.Id == 0;

        private async Task LoadDepartmentsAsync()
        {
            var depts = await _departments.GetAllAsync();
            DepartmentSelectList = new SelectList(depts, "Id", "Name");
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadDepartmentsAsync();

            if (id.HasValue)
            {
                var screen = await _screens.GetByIdAsync(id.Value);
                if (screen is null) return NotFound();

                // Employee can only edit their own department's screens
                if (!User.IsInRole("Admin"))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user?.DepartmentId != screen.DepartmentId)
                        return Forbid();
                }

                Screen = screen;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDepartmentsAsync();
            if (!ModelState.IsValid) return Page();

            var userId = _userManager.GetUserId(User)!;

            if (Screen.Id == 0)
                await _screens.CreateAsync(Screen, userId);
            else
                await _screens.UpdateAsync(Screen, userId);

            TempData["Success"] = IsNew ? "Scherm aangemaakt." : "Scherm bijgewerkt.";
            return RedirectToPage("/Admin/Screens/Index");
        }
    }
}

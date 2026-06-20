using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Admin.Announcements
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IDepartmentService _departmentService;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(IAnnouncementService announcementService, IDepartmentService departmentService, UserManager<IdentityUser> userManager)
        {
            _announcementService = announcementService;
            _departmentService = departmentService;
            _userManager = userManager;
        }

        [BindProperty]
        public Announcement Item { get; set; } = new();
        public SelectList DepartmentSelectList { get; set; } = null!;
        public bool IsNew => Item.Id == 0;


        private async Task LoadDepartmentsAsync()
        {
            var depts = await _departmentService.GetAllAsync();
            DepartmentSelectList = new SelectList(depts, "Id", "Name");
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadDepartmentsAsync();
            if (id.HasValue)
            {
                var item = await _announcementService.GetByIdAsync(id.Value);
                if (item is null) return NotFound();
                Item = item;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadDepartmentsAsync();
            if (!ModelState.IsValid) return Page();

            var userId = _userManager.GetUserId(User)!;
            Item.CreatedById = userId;

            if (Item.Id == 0)
                await _announcementService.CreateAsync(Item, userId);
            else
                await _announcementService.UpdateAsync(Item, userId);

            TempData["Success"] = "Aankondiging opgeslagen.";
            return RedirectToPage("/Admin/Announcements/Index");
        }
    }
}

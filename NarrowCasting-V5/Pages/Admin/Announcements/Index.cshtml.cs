using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Admin.Announcements
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAnnouncementService _announcementService;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(IAnnouncementService announcementService, UserManager<IdentityUser> userManager)
        {
            _announcementService = announcementService;
            _userManager = userManager;
        }

        public IEnumerable<Announcement> Announcements { get; set; } = new List<Announcement>();

        public async Task OnGetAsync()
        {
            if (User.IsInRole("Admin"))
            {
                Announcements = await _announcementService.GetAllAsync();
            }
            else
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is ApplicationUser appUser && appUser.DepartmentId.HasValue)
                {
                    Announcements = await _announcementService.GetActiveForDepartmentAsync(appUser.DepartmentId.Value);
                }
                else
                {
                    Announcements = [];
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            await _announcementService.DeleteAsync(id, userId);
            TempData["Success"] = "Aankondiging verwijderd.";
            return RedirectToPage();
        }
    }
}

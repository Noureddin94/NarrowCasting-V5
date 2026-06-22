using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Public
{
    public class DisplayModel : PageModel
    {
        private readonly IScreenService _screens;
        private readonly IAnnouncementService _announcements;

        public DisplayModel(IScreenService screens, IAnnouncementService announcements)
        {
            _screens = screens;
            _announcements = announcements;
        }

        public Screen? Screen { get; set; }
        public IEnumerable<Announcement> Announcements { get; set; } = Enumerable.Empty<Announcement>();

        public async Task<IActionResult> OnGetAsync(int screenId)
        {
            Screen = await _screens.GetByIdAsync(screenId);
            if (Screen is null) return Page();

            // Show department announcements; if internal screen, include internal msgs too
            Announcements = Screen.IsStaffScreen
                ? await _announcements.GetActiveForDepartmentAsync(Screen.DepartmentId)
                : (await _announcements.GetActiveForDepartmentAsync(Screen.DepartmentId))
                           .Where(a => !a.IsInternal);

            return Page();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Public
{
    public class IndexModel : PageModel
    {
        private readonly IAnnouncementService _announcements;
        public IndexModel(IAnnouncementService announcements) => _announcements = announcements;

        public IEnumerable<Announcement> Announcements { get; set; } = Enumerable.Empty<Announcement>();

        public async Task OnGetAsync() =>
            Announcements = (await _announcements.GetActiveAsync()).Where(a => !a.IsInternal).Take(6);
    }
}

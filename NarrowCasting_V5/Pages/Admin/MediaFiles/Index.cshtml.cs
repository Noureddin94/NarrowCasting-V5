using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Pages.Admin.MediaFiles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IMediaFileService _mediaFiles;

        public IndexModel(IMediaFileService mediaFiles)
        {
            _mediaFiles = mediaFiles;
        }

        public IEnumerable<MediaFile> MediaFiles { get; set; } = Enumerable.Empty<MediaFile>();

        public async Task OnGetAsync()
        {
            MediaFiles = await _mediaFiles.GetAllOrderedAsync();
        }
    }
}

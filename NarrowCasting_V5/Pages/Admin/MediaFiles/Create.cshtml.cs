using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;
using System.ComponentModel.DataAnnotations;

namespace NarrowCasting_V5.Pages.Admin.MediaFiles
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IMediaFileService _mediaFiles;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CreateModel> _logger;
        private const long MaxUploadBytes = 10 * 1024 * 1024;

        public CreateModel(
            IMediaFileService mediaFiles,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            ILogger<CreateModel> logger)
        {
            _mediaFiles = mediaFiles;
            _env = env;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        [Required]
        public IFormFile? Upload { get; set; }

        [BindProperty]
        [MaxLength(500)]
        public string? Caption { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Upload is null)
            {
                ModelState.AddModelError("Upload", "Select a file to upload.");
            }

            if (!ModelState.IsValid)
                return Page();

            if (Upload!.Length == 0)
            {
                ModelState.AddModelError("Upload", "The selected file is empty.");
                return Page();
            }

            if (Upload.Length > MaxUploadBytes)
            {
                ModelState.AddModelError("Upload", "The file is too large. Maximum size is 10 MB.");
                return Page();
            }

            var allowedContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/gif",
                "image/webp"
            };
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".webp"
            };
            var ext = Path.GetExtension(Upload.FileName);

            if (!allowedContentTypes.Contains(Upload.ContentType) || !allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("Upload", "Only image files (jpg, png, gif, webp) are allowed.");
                return Page();
            }

            var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            var fileName = Guid.NewGuid().ToString("N") + ext;
            var filePath = Path.Combine(uploadsRoot, fileName);
            var webPath = "/uploads/" + fileName;
            var originalFileName = Path.GetFileName(Upload.FileName);
            var userId = _userManager.GetUserId(User);

            var mf = new MediaFile
            {
                FileName = originalFileName,
                FilePath = webPath,
                MediaType = Enums.MediaType.Image,
                UploadedById = userId,
                Caption = Caption
            };

            try
            {
                Directory.CreateDirectory(uploadsRoot);

                await using (var stream = System.IO.File.Create(filePath))
                {
                    await Upload.CopyToAsync(stream);
                }

                await _mediaFiles.CreateAsync(mf);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload media file {FileName}", originalFileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                ModelState.AddModelError("Upload", "Upload failed. Please try again or choose another image.");
                return Page();
            }

            TempData["Success"] = "Bestand geupload.";
            return RedirectToPage("/Admin/MediaFiles/Index");
        }
    }
}

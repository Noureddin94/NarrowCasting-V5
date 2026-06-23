using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NarrowCasting_V5.Enums;
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
        private const long MaxImageUploadBytes = 10 * 1024 * 1024;
        private const long MaxVideoUploadBytes = 250 * 1024 * 1024;
        private const long MaxAudioUploadBytes = 50 * 1024 * 1024;

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
        [MaxLength(10000)]
        public string? TextContent { get; set; }

        [BindProperty]
        public IFormFile? Upload { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Selecteer een mediatype.")]
        public MediaType SelectedMediaType { get; set; } = MediaType.Image;

        [BindProperty]
        [MaxLength(500)]
        public string? Caption { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string? savedFilePath = null;

            // --- Validation ---
            if (SelectedMediaType != MediaType.Text && Upload is null)
            {
                ModelState.AddModelError("Upload", "Selecteer een bestand om te uploaden.");
            }
            if (SelectedMediaType == MediaType.Text && string.IsNullOrWhiteSpace(TextContent))
            {
                ModelState.AddModelError("TextContent", "Vul tekst in.");
            }

            if (!ModelState.IsValid)
                return Page();

            if (!IsSupportedMediaType(SelectedMediaType))
            {
                ModelState.AddModelError("SelectedMediaType", "Selecteer image, video, audio of text.");
                return Page();
            }

            // --- Handle file-based media ---
            if (SelectedMediaType != MediaType.Text)
            {
            if (Upload!.Length == 0)
            {
                ModelState.AddModelError("Upload", "Het geselecteerde bestand is leeg.");
                return Page();
            }

                var ext = Path.GetExtension(Upload.FileName);
                var rules = GetUploadRules(SelectedMediaType);

                if (Upload.Length > rules.MaxBytes)
            {
                    ModelState.AddModelError("Upload", $"Het bestand is te groot. Maximum is {rules.MaxMegabytes} MB voor {SelectedMediaType.ToString().ToLowerInvariant()}.");
                return Page();
            }

                if (!rules.ContentTypeSet.Contains(Upload.ContentType) || !rules.ExtensionSet.Contains(ext))
            {
                    ModelState.AddModelError("Upload", rules.ErrorMessage);
                    return Page();
                }
            }

            // --- Build the MediaFile entity ---
            var userId = _userManager.GetUserId(User);
            var mf = new MediaFile
            {
                FileName = SelectedMediaType == MediaType.Text ? "Text" : Path.GetFileName(Upload!.FileName),
                FilePath = string.Empty,
                MediaType = SelectedMediaType,
                UploadedById = userId,
                Caption = Caption,
                TextContent = TextContent
            };

            if (SelectedMediaType != MediaType.Text)
            {
                // --- Save the uploaded file ---
                var uploadsRoot = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
                var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(Upload!.FileName);
                var filePath = Path.Combine(uploadsRoot, fileName);
                var webPath = "/uploads/" + fileName;
                
                mf.FilePath = webPath;
                savedFilePath = filePath;

                try
                {
                    Directory.CreateDirectory(uploadsRoot);
                        await using var stream = System.IO.File.Create(filePath);
                        await Upload.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to upload media file {FileName}", mf.FileName);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    ModelState.AddModelError("Upload", "Upload failed. Please try again or choose another file.");
                    return Page();
                }
            }
            else
                {
                // Text does not have a file path � you can leave FilePath null or empty
                mf.FilePath = string.Empty;
                }

            try
            {
                await _mediaFiles.CreateAsync(mf, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create media record");
                if (!string.IsNullOrWhiteSpace(savedFilePath) && System.IO.File.Exists(savedFilePath))
                {
                    System.IO.File.Delete(savedFilePath);
                }

                ModelState.AddModelError("", "Er is iets misgegaan bij het opslaan van het mediabestand.");
                return Page();
            }

            TempData["Success"] = "Mediabestand geupload.";
            return RedirectToPage("/Admin/MediaFiles/Index");
        }

        private static bool IsSupportedMediaType(MediaType mediaType) =>
            mediaType is MediaType.Image or MediaType.Video or MediaType.Audio or MediaType.Text;

        private static UploadRules GetUploadRules(MediaType mediaType)
        {
            return mediaType switch
            {
                MediaType.Image => new UploadRules(
                    MaxImageUploadBytes,
                    new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" },
                    new[] { "image/jpeg", "image/png", "image/gif", "image/webp" },
                    "Alleen afbeeldingen (jpg, png, gif, webp) zijn toegestaan."),
                MediaType.Video => new UploadRules(
                    MaxVideoUploadBytes,
                    new[] { ".mp4", ".webm", ".mov", ".m4v" },
                    new[] { "video/mp4", "video/webm", "video/quicktime", "video/x-m4v" },
                    "Alleen video's (mp4, webm, mov, m4v) zijn toegestaan."),
                MediaType.Audio => new UploadRules(
                    MaxAudioUploadBytes,
                    new[] { ".mp3", ".wav", ".ogg", ".m4a" },
                    new[] { "audio/mpeg", "audio/wav", "audio/x-wav", "audio/ogg", "audio/mp4", "audio/x-m4a" },
                    "Alleen audio (mp3, wav, ogg, m4a) is toegestaan."),
                _ => throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null)
            };
        }

        private sealed record UploadRules(
            long MaxBytes,
            IEnumerable<string> Extensions,
            IEnumerable<string> ContentTypes,
            string ErrorMessage)
        {
            public long MaxMegabytes => MaxBytes / 1024 / 1024;

            public HashSet<string> ExtensionSet => new(Extensions, StringComparer.OrdinalIgnoreCase);
            public HashSet<string> ContentTypeSet => new(ContentTypes, StringComparer.OrdinalIgnoreCase);
        }
    }
}


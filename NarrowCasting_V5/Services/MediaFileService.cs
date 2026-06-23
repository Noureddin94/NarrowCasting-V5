using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Services
{
    public class MediaFileService : IMediaFileService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuditService _audit;

        public MediaFileService(ApplicationDbContext db, IAuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        public async Task<IEnumerable<MediaFile>> GetAllOrderedAsync()
        {
            return await _db.MediaFiles.OrderByDescending(m => m.UploadedAt).ToListAsync();
        }

        public async Task<MediaFile?> GetByIdAsync(int id)
        {
            return await _db.MediaFiles.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAsync(MediaFile file, string? userId = null)
        {
            _db.MediaFiles.Add(file);
            await _db.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _audit.LogAsync("MediaFile", file.Id, "Create", userId);
            }
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id , string userId)
        {
            var media = await _db.MediaFiles
                .Include(m => m.PlaylistItems)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (media is null)
                return (false, "Media not found");

            if (media.PlaylistItems.Any())
            {
                return (false, "Cannot delete media that is used in playlists.");
            }

            _db.MediaFiles.Remove(media);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("MediaFile", id, "Delete", userId);
            return (true, null);
        }
    }
}

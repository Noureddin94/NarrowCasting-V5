using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Services
{
    public class MediaFileService : IMediaFileService
    {
        private readonly ApplicationDbContext _db;

        public MediaFileService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<MediaFile>> GetAllOrderedAsync()
        {
            return await _db.MediaFiles.OrderByDescending(m => m.UploadedAt).ToListAsync();
        }

        public async Task CreateAsync(MediaFile file)
        {
            _db.MediaFiles.Add(file);
            await _db.SaveChangesAsync();
        }
    }
}

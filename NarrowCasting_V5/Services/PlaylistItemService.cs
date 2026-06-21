using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Services
{
    public class PlaylistItemService : IPlaylistItemService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuditService _audit;
        public PlaylistItemService(ApplicationDbContext db, IAuditService audit)
        {
            _db = db;
            _audit = audit;
        }
        public async Task<IEnumerable<PlaylistItem>> GetAllByPlaylistIdAsync(int playlistId)
        {
            return await _db.PlaylistItems
                .Where(pi => pi.PlaylistId == playlistId)
                .OrderBy(pi => pi.Order)
                .ToListAsync();
        }
        public async Task<PlaylistItem?> GetByIdAsync(int id)
        {
            return await _db.PlaylistItems.FindAsync(id);
        }
        public async Task CreateAsync(PlaylistItem playlistItem, string userId)
        {
            _db.PlaylistItems.Add(playlistItem);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("PlaylistItem", playlistItem.Id, "Create", userId);
        }
        public async Task UpdateAsync(PlaylistItem playlistItem, string userId)
        {
            _db.PlaylistItems.Update(playlistItem);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("PlaylistItem", playlistItem.Id, "Update", userId);
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var playlistItem = await _db.PlaylistItems.FindAsync(id);
            if (playlistItem != null)
            {
                _db.PlaylistItems.Remove(playlistItem);
                await _db.SaveChangesAsync();
                await _audit.LogAsync("PlaylistItem", id, "Delete", userId);
            }
        }
    }
}

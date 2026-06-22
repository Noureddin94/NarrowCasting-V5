using Microsoft.EntityFrameworkCore;
using NarrowCasting_V5.Data;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly ApplicationDbContext _db;
        private readonly IAuditService _audit;
        private const int MaxItems = 20;

        public PlaylistService(ApplicationDbContext db, IAuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        public async Task<IEnumerable<Playlist>> GetAllAsync()
        {
            return await _db.Playlists.Include(p => p.Screen!).ThenInclude(s => s.Department)
                                      .Include(p => p.Items)
                                      .OrderByDescending(p => p.CreatedAt)
                                      .ToListAsync();
        }

        public async Task<IEnumerable<Playlist>> GetByDepartmentAsync(int departmentId)
        {
            return await _db.Playlists.Include(p => p.Screen!).ThenInclude(s => s.Department)
                                      .Include(p => p.Items)
                                      .Where(p => p.Screen != null && p.Screen.DepartmentId == departmentId)
                                      .OrderByDescending(p => p.CreatedAt)
                                      .ToListAsync();
        }

        public async Task<Playlist?> GetByIdAsync(int id)
        {
            return await _db.Playlists.Include(p => p.Screen)
                                      .Include(p => p.Items).ThenInclude(i => i.MediaFile)
                                      .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Playlist?> GetByScreenAsync(int screenId)
        {
            return await _db.Playlists.Include(p => p.Items).ThenInclude(i => i.MediaFile)
                                      .Where(p => p.ScreenId == screenId)
                                      .OrderByDescending(p => p.CreatedAt)
                                      .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Playlist playlist, string userId)
        {
            _db.Playlists.Add(playlist);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Playlist", playlist.Id, "Create", userId);
        }

        public async Task UpdateAsync(Playlist playlist, string userId)
        {
            _db.Playlists.Update(playlist);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Playlist", playlist.Id, "Update", userId);
        }

        public async Task AddItemAsync(int playlistId, PlaylistItem item, string userId)
        {
            var count = await _db.PlaylistItems.CountAsync(i => i.PlaylistId == playlistId);
            if (count >= MaxItems)
                throw new InvalidOperationException($"Een playlist mag maximaal {MaxItems} items bevatten.");

            item.PlaylistId = playlistId;
            _db.PlaylistItems.Add(item);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("PlaylistItem", item.Id, "Add", userId);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var playlist = await _db.Playlists.FindAsync(id);
            if (playlist == null)
                throw new InvalidOperationException("Playlist not found.");

            _db.Playlists.Remove(playlist);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Playlist", id, "Delete", userId);
        }

        public async Task RemoveItemAsync(int itemId, string userId)
        {
            var item = await _db.PlaylistItems.FindAsync(itemId);
            if (item == null)
                throw new InvalidOperationException("Playlist item not found.");

            _db.PlaylistItems.Remove(item);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("PlaylistItem", itemId, "Remove", userId);
        }
    }
}

using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    public interface IPlaylistItemService
    {
        Task<IEnumerable<PlaylistItem>> GetAllByPlaylistIdAsync(int playlistId);
        Task<PlaylistItem?> GetByIdAsync(int id);
        Task CreateAsync(PlaylistItem playlistItem, string userId);
        Task UpdateAsync(PlaylistItem playlistItem, string userId);
        Task DeleteAsync(int id, string userId);
    }
}

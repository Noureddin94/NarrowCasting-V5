using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    public interface IMediaFileService
    {
        Task<IEnumerable<MediaFile>> GetAllOrderedAsync();
        Task CreateAsync(MediaFile file);
        Task<(bool Success, string? Error)> DeleteAsync(int id, string userId);
    }
}

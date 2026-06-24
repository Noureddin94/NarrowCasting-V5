using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    public interface IMediaFileService
    {
        Task<IEnumerable<MediaFile>> GetAllOrderedAsync();
        Task<MediaFile?> GetByIdAsync(int id);
        Task CreateAsync(MediaFile file, string? userId = null);
        Task<(bool Success, string? Error)> UpdateAsync(int id, string? caption, string? textContent, IFormFile? newFile, string userId);
        Task<(bool Success, string? Error)> DeleteAsync(int id, string userId);
    }
}

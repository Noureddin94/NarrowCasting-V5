using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    public interface IMediaFileService
    {
        Task<IEnumerable<MediaFile>> GetAllOrderedAsync();
        Task CreateAsync(MediaFile file);
    }
}

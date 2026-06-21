using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    public interface IAnnouncementService
    {
        Task<IEnumerable<Announcement>> GetAllAsync();
        Task<IEnumerable<Announcement>> GetActiveAsync();
        Task<IEnumerable<Announcement>> GetActiveForDepartmentAsync(int departmentId);
        Task<Announcement?> GetByIdAsync(int id);
        Task CreateAsync(Announcement announcement, string userId);
        Task UpdateAsync(Announcement announcement, string userId);
        Task DeleteAsync(int id, string userId);
    }
}

using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task CreateAsync(Department department, string userId);
        Task UpdateAsync(Department department, string userId);
        Task DeleteAsync(int id, string userId);
    }
}

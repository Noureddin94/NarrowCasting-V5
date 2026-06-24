using NarrowCasting_V5.Models;
using static NarrowCasting_V5.Services.DepartmentService;

namespace NarrowCasting_V5.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task CreateAsync(Department department, string userId);
        Task UpdateAsync(Department department, string userId);
        Task<DepartmentDeleteResult> DeleteAsync(int id, string userId);
    }
}

using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    /// <summary>
    /// User context service interface to decouple UserManager from application logic.
    /// Reduces coupling and improves testability.
    /// </summary>
    public interface IUserContextService
    {
        Task<string> GetCurrentUserIdAsync();
        Task<ApplicationUser?> GetCurrentUserAsync();
        Task<int?> GetCurrentUserDepartmentAsync();
        Task<bool> IsUserAdminAsync();
        Task<bool> CanUserAccessDepartmentAsync(int departmentId);
        Task<bool> CanUserAccessScreenAsync(int screenId);
    }
}

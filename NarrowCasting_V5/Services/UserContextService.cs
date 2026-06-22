using Microsoft.AspNetCore.Identity;
using NarrowCasting_V5.Interfaces;
using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Services
{
    /// <summary>
    /// User context service to reduce direct UserManager coupling across the app.
    /// Centralizes user authorization and permission logic.
    /// </summary>
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IScreenService _screenService;

        public UserContextService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            IScreenService screenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _screenService = screenService;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.Id ?? throw new InvalidOperationException("User not authenticated");
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync()
        {
            var httpUser = _httpContextAccessor.HttpContext?.User;
            if (httpUser is null) return null;
            return await _userManager.GetUserAsync(httpUser);
        }

        public async Task<int?> GetCurrentUserDepartmentAsync()
        {
            var user = await GetCurrentUserAsync();
            return user?.DepartmentId;
        }

        public async Task<bool> IsUserAdminAsync()
        {
            var user = await GetCurrentUserAsync();
            return user is not null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> CanUserAccessDepartmentAsync(int departmentId)
        {
            if (await IsUserAdminAsync()) return true;
            var userDeptId = await GetCurrentUserDepartmentAsync();
            return userDeptId == departmentId;
        }

        public async Task<bool> CanUserAccessScreenAsync(int screenId)
        {
            if (await IsUserAdminAsync()) return true;
            var screen = await _screenService.GetByIdAsync(screenId);
            if (screen is null) return false;
            var userDeptId = await GetCurrentUserDepartmentAsync();
            return userDeptId == screen.DepartmentId;
        }
    }
}

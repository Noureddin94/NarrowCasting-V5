using NarrowCasting_V5.Models;

namespace NarrowCasting_V5.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string entity, int entityId, string action, string userId, string? details = null);
        Task<IEnumerable<AuditLog>> GetLogsAsync(string? userId = null, DateTime? from = null, DateTime? to = null);
    }
}
